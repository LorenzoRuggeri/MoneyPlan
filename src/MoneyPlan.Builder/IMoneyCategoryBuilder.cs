using Savings.Model;

namespace MoneyPlan.Builder
{
    // ─── MoneyCategory Builder ────────────────────────────────────────────────────

    public interface IMoneyCategoryBuilder
    {
        IMoneyCategoryBuilder WithId(long id);
        IMoneyCategoryBuilder WithDescription(string description);
        IMoneyCategoryBuilder WithIcon(string icon);
        IMoneyCategoryBuilder WithParent(long parentId);
        IMoneyCategoryBuilder WithChildren(IEnumerable<MoneyCategory> children);
        MoneyCategory Build();
    }
}
