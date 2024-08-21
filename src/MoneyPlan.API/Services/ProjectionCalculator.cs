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
            var projectionItems = await CalculateAsync(null, null, date, false, false);
            await this.context.MaterializedMoneyItems.AddRangeAsync(projectionItems);
            await this.context.SaveChangesAsync();
        }

        private decimal CalculateCash(List<FixedMoneyItem> itemsToAccumulate, List<FixedMoneyItem> itemsNotAccumulate, Configuration config, decimal additionalCashLeft, DateTime periodStart)
        {
            var cashItems = itemsNotAccumulate
                            .Where(x => x.CategoryID != config.CashWithdrawalCategoryID && x.Cash)
                            .Union(itemsToAccumulate
                            .Where(x => x.CategoryID != config.CashWithdrawalCategoryID && x.Cash))
                            .ToList();


            var cashWithdrawal = itemsNotAccumulate
                .Where(x => x.CategoryID == config.CashWithdrawalCategoryID)
                .Union(itemsToAccumulate
                .Where(x => x.CategoryID == config.CashWithdrawalCategoryID))
                .OrderBy(x => x.Date).ToList();

            //There is additional cash left (ex. from previous month)
            const long cashWithdrawalAdditionalCashLeftID = 99999999999999999;
            if (additionalCashLeft != 0)
            {
                cashWithdrawal.Insert(0, new FixedMoneyItem { ID = cashWithdrawalAdditionalCashLeftID, Cash = true, Amount = additionalCashLeft, Note = "Additional Cash", Date = periodStart });
            }


            decimal carryCashExpenses = 0;
            decimal cashLeftToSpend = 0;
            foreach (var cashWithdrawalItem in cashWithdrawal)
            {
                var currentCashItems = cashItems.Where(x => x.Date >= cashWithdrawalItem.Date).OrderBy(x => x.Date);
                if (currentCashItems.Any()) cashWithdrawalItem.Note += $"(Original amount {cashWithdrawalItem.Amount})";
                if (carryCashExpenses > 0)
                {
                    cashWithdrawalItem.Amount += carryCashExpenses;
                    carryCashExpenses = 0;
                }
                var lstItemsToRemove = new List<FixedMoneyItem>();
                foreach (var currentCashItem in currentCashItems)
                {
                    cashWithdrawalItem.Amount -= currentCashItem.Amount;
                    //If it's subctracted from the additional cash (ex. from the prvious month). It has already been subtracted so it must be counted as 0 for this month balance
                    if (cashWithdrawalItem.ID == cashWithdrawalAdditionalCashLeftID)
                    {
                        if (cashWithdrawalItem.Amount >= 0)
                            currentCashItem.Amount = -cashWithdrawalItem.Amount;
                        else
                            currentCashItem.Amount = 0;
                    }
                    lstItemsToRemove.Add(currentCashItem);
                    if (cashWithdrawalItem.Amount >= 0)
                    {
                        carryCashExpenses = cashWithdrawalItem.Amount.Value;
                        cashWithdrawalItem.Amount = 0;
                        break;
                    }
                }
                foreach (var item in lstItemsToRemove)
                {
                    cashItems.Remove(item);
                }
                if (cashWithdrawalItem.Amount < 0)
                {
                    cashLeftToSpend += cashWithdrawalItem.Amount.Value;
                }
            }
            return cashLeftToSpend;
        }

        public async Task<IEnumerable<MaterializedMoneyItem>> CalculateAsync(DateTime? from, DateTime? to, DateTime? stopToDate, bool onlyInstallment = false, bool includeLastEndPeriod = true)
        {
            if (to == null) to = new DateTime(9999, 12, 31);
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

            while ((periodEnd = CalculateNextAccountingPeriod(periodStart, config.EndPeriodRecurrencyType, config.EndPeriodRecurrencyInterval).AddDays(-1)) <= to || periodStart < to)
            {
                int accumulatorStartingIndex = res.Count;
                
                // TODO: Ma a che serve l'AccumulateForBudget? Non mi basta semplicemente prelevare in Cash?
                //       Forse e' come il Salvadanaio in IntesaSanPaolo? Che non viene scritto da nessuna parte che e' finito in un altro 'cassetto'.
                //       EDIT: A riga 197 si fa menzione di un PeriodicBudget che serve a sottrarre l'AccumulateForBudget. Mhhh.
                var fixedItemsNotAccumulate = await context.FixedMoneyItems
                                                    .Include(x => x.Category)
                                                    .Where(x => x.Date >= periodStart && x.Date <= periodEnd && !x.AccumulateForBudget)
                                                    .AsNoTracking().ToListAsync();
                
                var fixedItemsAccumulate = await context.FixedMoneyItems
                                                    .Where(x => x.Date >= periodStart && x.Date <= periodEnd && x.AccumulateForBudget)
                                                    .AsNoTracking().ToListAsync();
                
                // TODO: Investigare sul come trasformare questa query per poter eliminare gli Adjustements.
                var recurrentItems = await context.RecurrentMoneyItems
                                                    .Include(x => x.Adjustements).Include(x => x.AssociatedItems).Include(x => x.Category)
                                                    .Where(x => (x.StartDate <= periodEnd && (!x.EndDate.HasValue || periodStart <= x.EndDate) && x.RecurrentMoneyItemID == null) ||
                                                                (x.Adjustements.Count() > 0 && x.Adjustements.First().RecurrencyNewDate.HasValue && x.Adjustements.First().RecurrencyNewDate.Value >= periodStart && x.Adjustements.First().RecurrencyNewDate.Value <= periodEnd))
                                                    .AsNoTracking().ToListAsync();

                if (onlyInstallment) recurrentItems = recurrentItems.Where(x => x.Type == MoneyType.InstallmentPayment).ToList();

                decimal additionalCash = 0;
                if (!endPeriodCashCarryUsed)
                {
                    endPeriodCashCarryUsed = true;
                    additionalCash = lastEndPeriod?.EndPeriodCashCarry ?? 0;
                }
                var cashLeftToSpend = CalculateCash(fixedItemsAccumulate, fixedItemsNotAccumulate, config, additionalCash, periodStart);

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

                //********************************  Calculation 2: Fixed items to accumulate for budget
                List<MaterializedMoneySubitems> lstSubItemsFixed = new();
                var accumulateMaterializedItem = new MaterializedMoneyItem { Date = periodStart, Note = "🧾 Accumulator", TimelineWeight = 5, IsRecurrent = false, Subitems = lstSubItemsFixed };
                foreach (var accumulateItem in fixedItemsAccumulate)
                {
                    accumulateMaterializedItem.Category = null;
                    accumulateMaterializedItem.Amount += accumulateItem.Amount ?? 0;
                    accumulateMaterializedItem.EndPeriod = false;
                    accumulateMaterializedItem.Type = MoneyType.Others;

                    lstSubItemsFixed.Add(new MaterializedMoneySubitems { Date = accumulateItem.Date, Amount = accumulateItem.Amount ?? 0, CategoryID = accumulateItem.CategoryID, Note = accumulateItem.Note });
                }
                if (fixedItemsAccumulate.Count > 0)
                {
                    accumulateMaterializedItem.Note += ": " + string.Join(',', fixedItemsAccumulate.Select(x => x.Note).ToArray());
                }
                res.Add(accumulateMaterializedItem);

                //********************************  Calculation 3: Recurrent items 
                var accumulatedForBudgetLeft = accumulateMaterializedItem.Amount;
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

                        //Check if is a budget for subtract the accumulated
                        if (recurrentItem.Type == MoneyType.PeriodicBudget)
                        {
                            decimal accumulatorToSubtract = accumulatedForBudgetLeft < currentInstallmentAmount ? currentInstallmentAmount : accumulatedForBudgetLeft;
                            accumulatedForBudgetLeft -= accumulatorToSubtract;
                            currentInstallmentAmount -= accumulatorToSubtract;
                        }

                        List<MaterializedMoneySubitems> lstSubItemsRecurrent = new();
                        //Check if there are child items
                        if (recurrentItem.AssociatedItems != null)
                        {
                            var lstNoteAssociatedItems = new List<string>();
                            var associatedItemsToCalculate = recurrentItem.AssociatedItems.Where(x => x.StartDate <= periodEnd && (!x.EndDate.HasValue || periodStart <= x.EndDate.Value));
                            if (onlyInstallment) associatedItemsToCalculate = associatedItemsToCalculate.Where(x => x.Type == MoneyType.InstallmentPayment);
                            foreach (var associatedItem in associatedItemsToCalculate)
                            {
                                var associatedIteminstallment = CalculateInstallmentInPeriod(associatedItem, installment.original, installment.original);
                                if (associatedIteminstallment.Count() > 0)
                                {
                                    currentInstallmentAmount += associatedItem.Amount;
                                    /*
                                    if (currentAdjustment?.RecurrencyNewAmount == null)
                                    {
                                        currentInstallmentAmount += associatedItem.Amount;
                                    }
                                    */

                                    lstNoteAssociatedItems.Add(associatedItem.Note);
                                    lstSubItemsRecurrent.Add(new MaterializedMoneySubitems { Date = currentInstallmentDate, Amount = associatedItem.Amount, CategoryID = associatedItem.CategoryID, Note = associatedItem.Note });
                                }
                            }
                            if (lstNoteAssociatedItems.Count > 0)
                            {
                                currentInstallmentNote += ": " + string.Join(',', lstNoteAssociatedItems.ToArray());
                            }

                        }

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
                            Subitems = lstSubItemsRecurrent
                        });
                    }
                }

                //********************************  Calculation 4: End Period item

                // TODO: Questo serve ad evitare che alcune Categorie vengano conteggiate.
                //       Ma una volta che avrò la gestione con i MoneyAccounts potrò escludere i singoli MoneyAccount dal conteggio.
                //       Un MoneyAccount sarà ad esempio 'Conto Trading Fineco'; cosi da non dover percepire gli accrediti fatti sul conto
                //       come un guadagno - che non sono in quanto non sono nient'altro che uno spostamento da un MoneyAccount ad un altro MoneyAccount.
                var excludeAction = (MaterializedMoneyItem item) => context.MaterializedMoneyRules
                    .Any(x => (x.Type == ItemType.Category && x.RelatedID == item.CategoryID) && x.Exclude == true);

                var periodIncome = res.GetRange(accumulatorStartingIndex, res.Count - accumulatorStartingIndex)
                    .Where(x => x.Amount > 0)
                    .Where(x => !excludeAction(x))
                    .Sum(x => x.Amount);

                var periodOutcome = res.GetRange(accumulatorStartingIndex, res.Count - accumulatorStartingIndex)
                    .Where(x => x.Amount < 0)
                    .Where(x => !excludeAction(x))
                    .Sum(x => x.Amount);

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
            var adjustment = item.Adjustements?.Where(x => x.RecurrencyDate == currentInstallmentOriginal && x.RecurrencyNewDate.HasValue).FirstOrDefault();
            if (adjustment != null)
            {
                currentInstallmentDate = adjustment.RecurrencyNewDate.Value;
            }
            else
            { 
                currentInstallmentDate = currentInstallmentOriginal;
                // Adjusting date based on Occurency Type.
                if (item.OccurrencyType == OccurrencyType.WorkingDay)
                {
                    currentInstallmentDate = currentInstallmentDate.GetWorkingDay();
                }
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
