using MoneyPlan.Model;

namespace MoneyPlan.Builder
{
    // ─── BudgetPlan Builder ───────────────────────────────────────────────────────

    public interface IBudgetPlanBuilder
    {
        IBudgetPlanBuilder WithId(int id);
        IBudgetPlanBuilder WithName(string name);
        IBudgetPlanBuilder WithPercentages(int needs, int wants, int savings);
        IBudgetPlanBuilder WithRule(int ruleId);
        IBudgetPlanBuilder WithRules(IEnumerable<int> ruleIds);
        BudgetPlan Build();
    }
}
