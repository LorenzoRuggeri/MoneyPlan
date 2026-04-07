using MoneyPlan.Model;
using Savings.Model;

namespace MoneyPlan.Application.Tests
{
    public class BudgetPlanServiceTests : UnitTestBase
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetBudgetPlanRules()
        {
            using (var context = CreateContext())
            {
                // ARRANGE
                context.Setup.CreateDefault();

                // ACT
                var rules = context.BudgetPlanService.GetBudgetPlanRules();

                // ASSERT
                Assert.That(rules.Count(), Is.EqualTo(4));
            }
        }
    }
}