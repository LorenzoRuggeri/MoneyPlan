using MoneyPlan.Application.Abstractions.Models.Report;
using MoneyPlan.Model;
using Savings.Model;

namespace MoneyPlan.Application.Abstractions.Budgeting
{
    public interface IBudgetPlanCalculator
    {
        /// <summary>
        /// Given an item, identify in which BudgetType it falls.
        /// </summary>
        BudgetPlanType GetBudgetTypeFor(IEnumerable<BudgetPlanRule> rules, MaterializedMoneyItem source);

        IEnumerable<ReportBudgetPlanType> GroupByBudgetTypes(IEnumerable<MaterializedMoneyItem> source, string periodPattern);
    }
}