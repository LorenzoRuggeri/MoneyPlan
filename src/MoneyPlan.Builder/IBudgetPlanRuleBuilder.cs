using MoneyPlan.Model;
using Savings.Model;

namespace MoneyPlan.Builder
{
    // ─── BudgetPlanRule Builder ───────────────────────────────────────────────────

    public interface IBudgetPlanRuleBuilder
    {
        IBudgetPlanRuleBuilder WithId(int id);
        IBudgetPlanRuleBuilder WithCategory(long categoryId);
        IBudgetPlanRuleBuilder WithCategoryFilter(StringFilterType filter, string text);
        IBudgetPlanRuleBuilder WithType(BudgetPlanType type);
        
        [Obsolete("Da non usare")]
        IBudgetPlanRuleBuilder AsIncome();

        IBudgetPlanRuleBuilder WithBudgetPlan(int id);
        BudgetPlanRule Build();

    }
}
