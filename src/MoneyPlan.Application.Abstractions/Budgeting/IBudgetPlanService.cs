using MoneyPlan.Model;

namespace MoneyPlan.Application.Abstractions.Budgeting
{
    public interface IBudgetPlanService
    {
        IEnumerable<BudgetPlanRule> GetBudgetPlanRules();
        IEnumerable<BudgetPlanRule> GetRulesForCategory(int budgetPlanId, int categoryId);
    }
}