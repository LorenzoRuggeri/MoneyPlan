using MoneyPlan.Application.Abstractions.Budgeting;
using MoneyPlan.Application.Abstractions.Models.Report;
using MoneyPlan.Model;
using Savings.Model;
using System.Data;
using System.Linq.Expressions;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoneyPlan.Application.Budgeting
{
    /// <summary>
    /// 
    /// </summary>
    internal class BudgetPlanCalculator : IBudgetPlanCalculator
    {
        private readonly IBudgetPlanService budgetPlanService;

        public BudgetPlanCalculator(IBudgetPlanService budgetPlanService)
        {
            this.budgetPlanService = budgetPlanService;
        }

        public BudgetPlanType GetBudgetTypeFor(IEnumerable<BudgetPlanRule> rules, MaterializedMoneyItem source)
        {
            var selectedRule = rules.FirstOrDefault(rule => filter(rule, source));
            if (selectedRule == null)
                return BudgetPlanType.None;
            return selectedRule.Type ?? BudgetPlanType.None;
        }

        public IEnumerable<ReportBudgetPlanType> GroupByBudgetTypes(IEnumerable<MaterializedMoneyItem> source, string periodPattern)
        {
            Func<MaterializedMoneyItem, MaterializedMoneyItem> AbsAmount = (source) =>
            {
                var clone = (MaterializedMoneyItem)source.Clone();
                clone.Amount = Math.Abs(clone.Amount);
                return clone;
            };

            List<MaterializedMoneyItem> income = new List<MaterializedMoneyItem>();
            List<MaterializedMoneyItem> needs = new List<MaterializedMoneyItem>();
            List<MaterializedMoneyItem> wants = new List<MaterializedMoneyItem>();
            List<MaterializedMoneyItem> savings = new List<MaterializedMoneyItem>();
            List<MaterializedMoneyItem> orphans = new List<MaterializedMoneyItem>(source);

            var orderedRules = budgetPlanService.GetBudgetPlanRules();

            var partialIncome = orphans.Where(item => item.Amount >= 0).ToList();
            income.AddRange(partialIncome);
            orphans = orphans.Except(partialIncome).ToList();

            foreach (var rule in orderedRules)
            {
                var partialNeeds = orphans.Where(item => filter(rule, item) && rule.Type == BudgetPlanType.Needs).ToList();
                needs.AddRange(partialNeeds.Select(AbsAmount));
                orphans = orphans.Except(partialNeeds).ToList();

                var partialWants = orphans.Where(item => filter(rule, item) && rule.Type == BudgetPlanType.Wants).ToList();
                wants.AddRange(partialWants.Select(AbsAmount));
                orphans = orphans.Except(partialWants).ToList();

                var partialSavings = orphans.Where(item => filter(rule, item) && rule.Type == BudgetPlanType.Savings).ToList();
                savings.AddRange(partialSavings.Select(AbsAmount));
                orphans = orphans.Except(partialSavings).ToList();

                // If we correctly distribuited all data, we can exit.
                if (!orphans.Any())
                    break;
            }
            // If we've still orphans (that means no rules were applied), we must ensure their values are positive, otherwise
            // the calculations will fail.
            orphans = orphans.Select(AbsAmount).ToList();


            // Tutto diviso in base ai periodi.
            // Le percentuali totali, per il grafico a torta, ce ne occuperemo solo dopo.
            var incomePeriod = income.GroupBy(x => x.Date.ToString(periodPattern))
                .Select(x => new ReportPeriodAmountPercent() { Period = x.Key, Amount = (double)x.Sum(y => y.Amount) })
                .ToList();
            
            var spentPeriod = needs.Union(wants).Union(orphans).GroupBy(x => x.Date.ToString(periodPattern))
                .Select(x => new ReportPeriodAmountPercent() { Period = x.Key, Amount = (double)x.Sum(y => y.Amount) })
                .ToList();

            var savingsPeriod = savings.GroupBy(x => x.Date.ToString(periodPattern))
                .Select(x => new ReportPeriodAmountPercent() { Period = x.Key, Amount = (double)x.Sum(y => y.Amount) })
                .ToList();

            // Variabile che enumera tutti i Period di interesse per il contesto.
            var groupKeys = incomePeriod.Union(spentPeriod).Union(savingsPeriod)
                .GroupBy(x => x.Period).Select(x => x.Key);

            // Collezione separata che copre tutti i periodi, usata solo per il LeftJoin degli effectiveSavings.
            // incomePeriod rimane invariato e contiene solo i periodi con income reale.
            var allIncomePeriods = groupKeys.Select(period => new ReportPeriodAmountPercent()
            {
                Period = period,
                Amount = incomePeriod.FirstOrDefault(x => x.Period == period)?.Amount ?? 0
            });

            var effectiveSavings = allIncomePeriods.Select(x =>
            {
                var planned = savingsPeriod.FirstOrDefault(s => s.Period == x.Period)?.Amount ?? 0;
                var spent = spentPeriod.FirstOrDefault(s => s.Period == x.Period)?.Amount ?? 0;

                var liquidity = x.Amount - planned;
                var saved = (liquidity > 0 ? liquidity : 0) - spent;        // Nel caso in cui non abbia nessun Income-mensile ma ho spese
                var effectiveSaving = (saved > 0 ? saved : 0) + planned;    // Nel caso in cui abbia risparmiato (usando liquidità) piu' del Income-mensile
                var clampedAmount = effectiveSaving > 0 ? effectiveSaving : 0;

                return new ReportPeriodAmountPercent()
                {
                    Period = x.Period,
                    Amount = clampedAmount,
                };
            }).ToList();    // Materialize it! Otherwise the next results will be wrong!


            // Base di calcolo per il TotalPercent
            var totalBase = needs.Sum(x => x.Amount)
              + wants.Union(orphans).Sum(x => x.Amount)
              + effectiveSavings.Sum(x => (decimal)x.Amount);

            // Base di calcolo per il Relative Percent
            var relativeBase = groupKeys.Select(period =>
            {
                var incomeItems = income.Where(x => x.Date.ToString(periodPattern) == period);
                if (incomeItems.Any())
                    return incomeItems.GroupBy(x => x.Date.ToString(periodPattern)).First();

                return needs.Union(wants).Union(orphans).Union(savings)
                    .Where(x => x.Date.ToString(periodPattern) == period)
                    .GroupBy(x => x.Date.ToString(periodPattern)).First();
            });

            List<ReportBudgetPlanType> list = new List<ReportBudgetPlanType>();
            list.Add(new ReportBudgetPlanType()
            {
                Description = "Needs",
                TotalPercent = FindTotalPercent(needs, x => x.Amount, totalBase),
                Data = needs.GroupBy(x => x.Date.ToString(periodPattern))
                    .Select(x => new ReportPeriodAmountPercent()
                    { 
                        Period = x.Key, Amount = (double)x.Sum(y => y.Amount), Percent = FindRelativePercent(x, relativeBase)
                    }).ToArray()
            });
            list.Add(new ReportBudgetPlanType()
            {
                Description = "Wants",
                TotalPercent = FindTotalPercent(wants.Union(orphans), x => x.Amount, totalBase),
                Data = wants.Union(orphans).GroupBy(x => x.Date.ToString(periodPattern))
                    .Select(x => new ReportPeriodAmountPercent() 
                    { 
                        Period = x.Key, Amount = (double)x.Sum(y => y.Amount), Percent = FindRelativePercent(x, relativeBase)
                    }).ToArray()
            });
            list.Add(new ReportBudgetPlanType()
            {
                Description = "Savings",
                TotalPercent = FindTotalPercent(effectiveSavings, x => x.Amount, totalBase),
                Data = effectiveSavings.Select(x => new ReportPeriodAmountPercent()
                {
                    Period = x.Period, Amount = x.Amount,  Percent = FindRelativePercent(x.Period, x.Amount, relativeBase)
                }).ToArray()
            });

            return list.ToArray();
        }

        private static double FindTotalPercent<T>(IEnumerable<T> source, Func<T, decimal> expression, decimal totalIncome)
        {
            if (totalIncome == 0)
                return 0;
            return Math.Abs(Math.Round((double)(source.Sum(expression) / totalIncome) * 100, 2));
        }

        private static double FindTotalPercent<T>(IEnumerable<T> source, Func<T, double> expression, decimal totalIncome)
        {
            if (totalIncome == 0)
                return 0;
            return Math.Abs(Math.Round((source.Sum(expression) / (double)totalIncome) * 100, 2));
        }

        private double FindTotalPercent(decimal value, decimal totalIncome)
        {
            if (totalIncome == 0)
                return 0;
            return Math.Abs(Math.Round((double)(value / totalIncome) * 100, 2));
        }

        private static double FindRelativePercent(IGrouping<string, MaterializedMoneyItem> partial, IEnumerable<IGrouping<string, MaterializedMoneyItem>> total)
        {
            decimal totalIncome = total.Where(x => x.Key == partial.Key).Select(x => x.Sum(z => z.Amount)).Sum(x => x);
            // It should never happens but we keep for code-safety.
            if (totalIncome == 0)
                return 0;
            decimal partialIncome = partial.Sum(x => x.Amount);

            return Math.Abs(Math.Round((double)(partialIncome / totalIncome) * 100, 2));
        }

        private static double FindRelativePercent(string period, double amount, IEnumerable<IGrouping<string, MaterializedMoneyItem>> total)
        {
            decimal totalAmount = total.Where(x => x.Key == period).Select(x => x.Sum(z => z.Amount)).Sum(x => x);
            // It should never happens but we keep for code-safety.
            if (totalAmount == 0)
                return 0;
            return Math.Abs(Math.Round((double)((decimal)amount / totalAmount) * 100, 2));
        }

        /// <summary>
        /// Given a Budget Plan rule, tells if it fits the rule.
        /// </summary>
        Func<BudgetPlanRule, MaterializedMoneyItem, bool> filter = (rule, moneyItem) =>
        {
            // A positive amount means it's an inflow. And an inflow cannot fall under a Budget Plan Type.
            if (moneyItem.Amount >= 0)
            {
                return false;
            }
            else if (!string.IsNullOrEmpty(rule.CategoryText) && rule.CategoryId == moneyItem.CategoryID)
            {
                if (rule.CategoryFilter == StringFilterType.Contains)
                    return moneyItem.Note.Contains(rule.CategoryText, StringComparison.OrdinalIgnoreCase);
                else if (rule.CategoryFilter == StringFilterType.Equals)
                    return moneyItem.Note.Equals(rule.CategoryText, StringComparison.OrdinalIgnoreCase);
            }
            else if (rule.CategoryId == moneyItem.CategoryID)
            {
                return true;
            }
            return false;
        };

    }
}
