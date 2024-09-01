using Microsoft.EntityFrameworkCore;
using Savings.DAO.Infrastructure;
using Savings.API.Services.Abstract;
using Savings.Model;
using Savings.API.Extensions;

namespace Savings.API.Services
{
    public class ProjectionCalculator : IProjectionCalculator
    {
        private readonly SavingsContext context;

        public ProjectionCalculator(SavingsContext context)
        {
            this.context = context;
        }

        public async Task SaveProjectionToHistory(DateTime date)
        {
            var projectionItems = await CalculateAsync(null, null, null, date, false, false);
            await this.context.MaterializedMoneyItems.AddRangeAsync(projectionItems);
            await this.context.SaveChangesAsync();
        }

        internal decimal CalculateCash(List<FixedMoneyItem> itemsNotAccumulate, Configuration config, decimal additionalCashLeft, DateTime periodStart)
        {
            // We don't want to modify cash earnings, so we're deep cloning them to be sure they will be untouched 
            var cashEarned = itemsNotAccumulate.ConvertAll(x => x.Clone())
                .Where(x => x.CategoryID != config.CashWithdrawalCategoryID && x.Cash && x.Amount > 0)
                .ToList();

            // We don't want to modify the cash expenses, so we're deep cloning them to be sure they will be untouched
            var cashSpent = itemsNotAccumulate.ConvertAll(x => x.Clone())
                .Where(x => x.CategoryID != config.CashWithdrawalCategoryID && x.Cash && x.Amount < 0)
                .ToList();

            var cashWithdrawal = itemsNotAccumulate
                .Where(x => x.CategoryID == config.CashWithdrawalCategoryID)
                .OrderBy(x => x.Date).ToList();

            
            //There is additional cash left (ex. from previous month)
            const long cashWithdrawalAdditionalCashLeftID = 99999999999999999;
            if (additionalCashLeft != 0)
            {
                cashEarned.Insert(0, new FixedMoneyItem
                { 
                    ID = cashWithdrawalAdditionalCashLeftID, Cash = true, Amount = additionalCashLeft, Note = "Additional Cash", Date = periodStart
                });
            }

            var lstItemsToRemove = new List<FixedMoneyItem>();
            decimal carryCashExpenses = 0;
            decimal cashLeftToSpend = 0;

            // Before to interact with Withdrawals, we want to use cash left from previous period or cash earned this period.
            foreach (var item in cashEarned)
            {
                // We want to consider only the cash expenses AFTER we got cash credit; otherwise we didn't have the cash to use for them.
                var currentCashItems = cashSpent.Where(x => x.Date >= item.Date).OrderBy(x => x.Date);
                if (currentCashItems.Any()) item.Note += $" (Original amount {item.Amount})";

                foreach (var currentCashExpense in currentCashItems)
                {
                    decimal creditToSubtract = 0;
                    creditToSubtract = Math.Min(Math.Abs(item.Amount.Value), Math.Abs(currentCashExpense.Amount.Value));
                    currentCashExpense.Amount += creditToSubtract;
                    item.Amount -= creditToSubtract;

                    // Remove the zero values, so we don't cycle it.
                    if (currentCashExpense.Amount == 0)
                    {
                        lstItemsToRemove.Add(currentCashExpense);
                    }
                    // We consumed all the cash we had from previous period.
                    if (item.Amount == 0)
                    {
                        break;
                    }
                }
                foreach (var itemToRemove in lstItemsToRemove)
                {
                    cashSpent.Remove(itemToRemove);
                }
                if (item.Amount > 0)
                {
                    cashLeftToSpend += item.Amount.Value;
                }
            }
            lstItemsToRemove.Clear();

            foreach (var cashWithdrawalItem in cashWithdrawal)
            {
                // We want to take in account only the cash spent after we withdraw the money.
                var currentCashItems = cashSpent.Where(x => x.Date >= cashWithdrawalItem.Date).OrderBy(x => x.Date);
                if (currentCashItems.Any()) cashWithdrawalItem.Note += $" (Original amount {cashWithdrawalItem.Amount})";
                
                // We want to parse all cash expenses (from current period).
                foreach (var currentCashItem in currentCashItems)
                {
                    // Always add the amount; the Cash could be negative (we spent) or it also could be positive (we earned it).
                    decimal creditToSubtract = 0;
                    creditToSubtract = Math.Abs(Math.Max(cashWithdrawalItem.Amount.Value, currentCashItem.Amount.Value));
                    currentCashItem.Amount += creditToSubtract;
                    // NOTE: A Cash withdrawal CANNOT NEVER BE a negative amount.
                    cashWithdrawalItem.Amount += creditToSubtract;

                    if (currentCashItem.Amount == 0)
                    {
                        lstItemsToRemove.Add(currentCashItem);
                    }

                    // If the cash covered the withdrawal we're going to set to zero the withdrawal.
                    if (cashWithdrawalItem.Amount >= 0)
                    {
                        carryCashExpenses = cashWithdrawalItem.Amount.Value;
                        cashWithdrawalItem.Amount = 0;                        
                        break;
                    }
                }

                foreach (var item in lstItemsToRemove)
                {
                    cashSpent.Remove(item);
                }
                if (cashWithdrawalItem.Amount < 0)
                {
                    cashLeftToSpend += Math.Abs(cashWithdrawalItem.Amount.Value);
                }
                else
                {
                    cashLeftToSpend += carryCashExpenses;
                }
            }

            // NOTE: these are for debugging purposes. The 'c' must be equals to our function result ('cashLeftToSpend').
            var a = cashEarned.Select(x => x.Amount).Sum();
            var b = cashWithdrawal.Select(x => x.Amount).Sum() ?? 0;
            var c = a + Math.Abs(b);            

            return cashLeftToSpend;
        }

        public async Task<IEnumerable<MaterializedMoneyItem>> CalculateAsync(int? accountId, DateTime? from, DateTime? to, DateTime? stopToDate, 
            bool onlyInstallment = false, bool includeLastEndPeriod = true)
        {
            if (to == null) to = new DateTime(DateTime.Now.Year, 12, 31);
            var res = new List<MaterializedMoneyItem>();
            var lastEndPeriod = context.MaterializedMoneyItems.Where(x => x.EndPeriod).OrderByDescending(x => x.Date).FirstOrDefault();
            if (lastEndPeriod != null && includeLastEndPeriod)
                res.Add(lastEndPeriod);

            //var fromDate = lastEndPeriod?.Date ?? throw new Exception("Unable to define the starting time");
            var fromDate = lastEndPeriod?.Date.AddDays(1) ?? (from.HasValue ? from.Value : new DateTime(2000, 1, 1));

            //var periodStart = fromDate.AddDays(1);
            var periodStart = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day);
            var config = context.Configuration.FirstOrDefault() ?? throw new Exception("Unable to find the configuration");
            DateTime periodEnd;
            bool endPeriodCashCarryUsed = false;
            decimal cashLeftToSpend = 0;

            while ((periodEnd = CalculateNextAccountingPeriod(periodStart, config.EndPeriodRecurrencyType, config.EndPeriodRecurrencyInterval).AddDays(-1)) <= to || periodStart < to)
            {
                int accumulatorStartingIndex = res.Count;

                decimal? periodIncome = 0;
                decimal? periodOutcome = 0;

                var fixedItemsNotAccumulate = await context.FixedMoneyItems
                                                    .Include(x => x.Category)
                                                    .Where(x => accountId.HasValue ? x.AccountID == accountId : true)
                                                    .Where(x => x.Date >= periodStart && x.Date <= periodEnd)
                                                    .AsNoTracking().ToListAsync();
                
                // TODO: Investigare sul come trasformare questa query per poter eliminare gli Adjustements.
                var recurrentItems = await context.RecurrentMoneyItems
                                                    .Include(x => x.Category)
                                                    .Where(x => accountId.HasValue ? x.MoneyAccountId == accountId : true)
                                                    .Where(x => x.StartDate <= periodEnd && (!x.EndDate.HasValue || periodStart <= x.EndDate))
                                                    .AsNoTracking().ToListAsync();

                if (onlyInstallment) recurrentItems = recurrentItems.Where(x => x.Type == MoneyType.InstallmentPayment).ToList();

                // Determines how much Cash we've left from previous period.
                decimal additionalCash = 0;
                if (!endPeriodCashCarryUsed)
                {
                    endPeriodCashCarryUsed = true;
                    additionalCash = lastEndPeriod?.EndPeriodCashCarry ?? 0;
                }
                else
                {
                    additionalCash = cashLeftToSpend;
                }

                // Before applying cash modifications, we want to know which is the real amount of spent and income.
                periodIncome += fixedItemsNotAccumulate
                    .Where(x => x.Amount > 0)
                    .Sum(x => x.Amount);

                periodOutcome += fixedItemsNotAccumulate
                    .Where(x => x.Amount < 0)
                    .Sum(x => x.Amount);

                cashLeftToSpend = CalculateCash(fixedItemsNotAccumulate, config, additionalCash, periodStart);

                //********************************  Calculation 1: Fixed items to not accumulate
                foreach (var fixedItem in fixedItemsNotAccumulate)
                {
                    res.Add(new MaterializedMoneyItem
                    {
                        Date = fixedItem.Date,
                        CategoryID = fixedItem?.Category?.ID,
                        Amount = fixedItem.Amount ?? 0,
                        EndPeriod = false,
                        Note = fixedItem.Note,
                        Type = MoneyType.Others,
                        TimelineWeight = fixedItem.TimelineWeight,
                        IsRecurrent = false,
                        FixedMoneyItemID = fixedItem.ID,
                        Cash = fixedItem.Cash
                    });
                }

                //********************************  Calculation 2: Recurrent items 
                var tmpIndex = res.Count;
                foreach (var recurrentItem in recurrentItems)
                {
                    var installments = CalculateInstallmentInPeriod(recurrentItem, periodStart, periodEnd);

                    foreach (var installment in installments)
                    {
                        /*
                        //Check the adjustements
                        var currentAdjustment = recurrentItem.Adjustements?.Where(x => x.RecurrencyDate == installment.currentDate || x.RecurrencyNewDate.HasValue && x.RecurrencyNewDate == installment.currentDate).FirstOrDefault();
                        var currentInstallmentDate = currentAdjustment?.RecurrencyNewDate ?? installment.original;
                        var currentInstallmentAmount = currentAdjustment?.RecurrencyNewAmount ?? recurrentItem.Amount;
                        var currentInstallmentNote = recurrentItem.Note;
                        */

                        var currentInstallmentDate = installment.currentDate;
                        var currentInstallmentAmount = recurrentItem.Amount;
                        var currentInstallmentNote = recurrentItem.Note;

                        res.Add(new MaterializedMoneyItem
                        {
                            Date = currentInstallmentDate,
                            CategoryID = recurrentItem?.Category?.ID,
                            Amount = currentInstallmentAmount,
                            EndPeriod = false,
                            Note = currentInstallmentNote,
                            Type = recurrentItem.Type,
                            TimelineWeight = recurrentItem.TimelineWeight,
                            IsRecurrent = true,
                            RecurrentMoneyItemID = recurrentItem.ID,
                        });
                    }
                }

                // After we applied the Recurrent items we want to add the income\outcome
                periodIncome += res.GetRange(tmpIndex, res.Count - tmpIndex)
                    .Where(x => x.Amount > 0)
                    .Sum(x => x.Amount);

                periodOutcome += res.GetRange(tmpIndex, res.Count - tmpIndex)
                    .Where(x => x.Amount < 0)
                    .Sum(x => x.Amount);

                //********************************  Calculation 3: End Period item

                res.Add(new MaterializedMoneyItem {
                    Amount = res.GetRange(accumulatorStartingIndex, res.Count - accumulatorStartingIndex).Sum(x => x.Amount),
                    Note = $"💵 Cash: {cashLeftToSpend:N2} | Income: {periodIncome} | Outcome: {periodOutcome}", 
                    Date = periodEnd, 
                    EndPeriod = true, 
                    IsRecurrent = false, 
                    EndPeriodCashCarry = cashLeftToSpend
                });

                if (periodEnd == stopToDate) break;
                periodStart = periodEnd.AddDays(1);
            }

            //Calculate the projection
            var lastProjectionValue = context.MaterializedMoneyItems.Where(x => x.Date <= fromDate)
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.EndPeriod)
                .AsNoTracking().FirstOrDefault()?.Projection ?? 0;

            res = res.OrderBy(x => x.Date).ThenByDescending(x => x.TimelineWeight).ToList();

            foreach (var resItem in res)
            {
                resItem.Projection = lastProjectionValue + (resItem.EndPeriod ? 0 : resItem.Amount);
                lastProjectionValue = resItem.Projection;
            }
            
            //if (from.HasValue) res.RemoveAll(x => x.Date <= from);
            if (from.HasValue) res.RemoveAll(x => x.Date < from);
            return res;
        }

        IEnumerable<(DateTime original, DateTime currentDate)> CalculateInstallmentInPeriod(RecurrentMoneyItem item, DateTime periodStart, DateTime periodEnd)
        {
            var lstInstallmentsDate = new List<(DateTime original, DateTime currentDate)>();
            if (item.StartDate <= periodEnd && (!item.EndDate.HasValue || periodStart <= item.EndDate.Value))
            {
                var currentInstallmentOriginal = item.StartDate;
                var currentInstallmentDate = CalculateActualInstallmentDate(item, currentInstallmentOriginal);
                while (currentInstallmentDate <= periodEnd)
                {
                    if (currentInstallmentDate >= periodStart)
                    {
                        lstInstallmentsDate.Add((currentInstallmentOriginal, currentInstallmentDate));
                    }
                    if (item.RecurrencyInterval == 0) break;
                    currentInstallmentOriginal = CalculateNextReccurrency(currentInstallmentOriginal, item.RecurrencyType, item.RecurrencyInterval, item.OccurrencyType);
                    currentInstallmentDate = CalculateActualInstallmentDate(item, currentInstallmentOriginal);
                }
            }
            return lstInstallmentsDate;
        }

        private static DateTime CalculateActualInstallmentDate(RecurrentMoneyItem item, DateTime currentInstallmentOriginal)
        {
            DateTime currentInstallmentDate;
            // TODO: rimuovere dai parametri 'currentInstallmentOriginal'? E' possibile farlo?
            currentInstallmentDate = currentInstallmentOriginal;
            // Adjusting date based on Occurency Type.
            if (item.OccurrencyType == OccurrencyType.WorkingDay)
            {
                currentInstallmentDate = currentInstallmentDate.GetWorkingDay();
            }
            return currentInstallmentDate;
        }

        DateTime CalculateNextReccurrency(DateTime currentEndPeriod, RecurrencyType recurrType, int recurrIterval, OccurrencyType occurrencyType)
        {
            DateTime nextEndPeriod = currentEndPeriod;

            switch (recurrType)
            {
                case RecurrencyType.Day:
                    nextEndPeriod = currentEndPeriod.AddDays(recurrIterval);
                    break;
                case RecurrencyType.Week:
                    nextEndPeriod = currentEndPeriod.AddDays(recurrIterval * 7);
                    break;
                case RecurrencyType.Month:
                    nextEndPeriod = currentEndPeriod.AddMonths(recurrIterval);
                    break;
            }

            return nextEndPeriod;
        }

        DateTime CalculateNextAccountingPeriod(DateTime currentEndPeriod, RecurrencyType recurrType, int recurrIterval)
        {
            DateTime nextEndPeriod = currentEndPeriod;
            // NOTE: 'recurrIterval' deve essere sempre almeno 1, di modo che aggiungo 0. Contorto ma funziona.

            recurrIterval = recurrIterval - 1;

            switch (recurrType)
            {
                case RecurrencyType.Day:
                    nextEndPeriod = currentEndPeriod.AddDays(recurrIterval);
                    break;
                case RecurrencyType.Week:
                    nextEndPeriod = currentEndPeriod.AddDays(recurrIterval * 7);
                    // TODO: lavorare su questo metodo per farmi dire quando finisce la settimana.
                    /*
                    DateTime StartOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                    DateTime EndOfLastWeek = StartOfWeek.AddDays(-1);
                    */
                    break;
                case RecurrencyType.Month:
                    //nextEndPeriod = currentEndPeriod.AddMonths(recurrIterval);
                    nextEndPeriod = currentEndPeriod.AddMonths(recurrIterval).GetLastDayOfMonth().AddDays(1);
                    break;
            }
            return nextEndPeriod;
        }
    }
}
