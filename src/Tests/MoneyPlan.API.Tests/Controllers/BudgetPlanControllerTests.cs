using Microsoft.AspNetCore.Mvc;
using MoneyPlan.API.Controllers;
using MoneyPlan.API.Tests._Helpers;
using MoneyPlan.Model;
using Savings.API.Controllers;
using Savings.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyPlan.API.Tests.Controllers
{
    [TestFixture]
    public class BudgetPlanControllerTests : UnitTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Register<BudgetPlanController>();
        }

        [Test]
        public async Task GetBudgetPlans_ReturnsAllPlans()
        {
            using var ctx = CreateContext();
            var db = ctx.DbContext;

            db.BudgetPlans.Add(new BudgetPlan { Id = 1, Name = "Plan A" });
            db.BudgetPlans.Add(new BudgetPlan { Id = 2, Name = "Plan B" });
            await db.SaveChangesAsync();

            var sut = ctx.Resolve<BudgetPlanController>();

            var actionResult = await sut.GetBudgetPlans();

            var value = actionResult.Value;
            Assert.NotNull(value);
            Assert.That(value.Count(), Is.EqualTo(2));
            Assert.IsTrue(value.Any(p => p.Name == "Plan A"));
            Assert.IsTrue(value.Any(p => p.Name == "Plan B"));
        }

        /*
        [Test]
        public async Task CreateBudgetPlanRule_ReturnsNewId_AndPersists()
        {
            using var ctx = CreateContext();
            var db = ctx.DbContext;

            var sut = ctx.Resolve<BudgetPlanController>();

            var rule = new BudgetPlanRule
            {
                
                Name = "Test Rule"
            };

            var actionResult = await sut.CreateBudgetPlanRule(rule);

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedId = Assert.IsType<int>(okResult.Value);
            Assert.True(returnedId > 0);

            var persisted = await db.BudgetPlanRules.FindAsync(returnedId);
            Assert.NotNull(persisted);
            Assert.Equal("Test Rule", persisted.Name);
        }
        */
    }
}