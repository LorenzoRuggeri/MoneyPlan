using Humanizer;
using MoneyPlan.API.Tests._Helpers;
using Savings.API.Controllers;
using Savings.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPlan.API.Tests.Controllers
{
    [TestFixture]
    public class ReportControllerTests : UnitTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Register<ReportController>();
        }

        [Test]
        public async Task GetBudgetPlanResume()
        {
            using (var context = this.CreateContext())
            {
                // ARRANGE
                var accountEntity = new Savings.Model.MoneyAccount() { ID = 1, Name = "My Account" };
                context.DbContext.MoneyAccounts.Add(accountEntity);

                context.DbContext.FixedMoneyItems.Add(new Savings.Model.FixedMoneyItem()
                {
                    Date = DateTime.Now.AddMonths(-2).AtNoon(),
                    Amount = 2000, Note = "Salario", Account = accountEntity
                });
                context.DbContext.FixedMoneyItems.Add(new Savings.Model.FixedMoneyItem()
                {
                    Date = DateTime.Now.AddMonths(-2).AtNoon(),
                    Amount = 8000, Note = "Bonifico causa civile", Account = accountEntity
                });

                context.DbContext.SaveChanges();

                // ACT
                var sut = context.Resolve<ReportController>();
                var response = await sut.GetBudgetPlanResume(accountEntity.ID, "yyyy", DateTime.Now.AddMonths(-3), DateTime.Now);

                
                // ASSERT
                Assert.That(response, Is.Not.Null);                
                var result = response.Value;
                Assert.That(result.Count(), Is.EqualTo(3));

            }
        }

    }
}
