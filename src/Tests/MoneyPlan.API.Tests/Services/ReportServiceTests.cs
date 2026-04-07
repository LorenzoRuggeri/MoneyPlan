using MoneyPlan.API.Tests._Helpers;
using NuGet.ContentModel;
using Savings.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPlan.API.Tests.Services
{
    [TestFixture]
    public class ReportServiceTests : UnitTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Register<ReportService>();
        }

        [Test]
        public void Test1()
        {
            using (var context = CreateContext())
            {
                // ARRANGE

                // ACT
                var sut = context.Resolve<ReportService>();

                var results = sut.GetStructuredCategories();

                // ASSERT
                Assert.That(results, Is.Not.Null.Or.Empty);
                Assert.Fail("Continuare a scriverlo; al momento non mi convince quello che vedo. Non tratta le categorie innestate");
            }                
        }

        [Test]
        public void Test2()
        {
            using (var context = CreateContext())
            {
                // ARRANGE

                context.DbContext.MoneyCategories.Add(new Savings.Model.MoneyCategory() { ID = 1, ParentId = null, Description = "Casa" });
                context.DbContext.MoneyCategories.Add(new Savings.Model.MoneyCategory() { ID = 2, ParentId = 1, Description = "Bollette" });
                context.DbContext.MoneyCategories.Add(new Savings.Model.MoneyCategory() { ID = 3, ParentId = 1, Description = "Manutenzione" });

                context.DbContext.MoneyCategories.Add(new Savings.Model.MoneyCategory() { ID = 4, ParentId = 1, Description = "Sub-affitto" });
                context.DbContext.MoneyCategories.Add(new Savings.Model.MoneyCategory() { ID = 5, ParentId = 4, Description = "Sub-Bollette" });
                context.DbContext.MoneyCategories.Add(new Savings.Model.MoneyCategory() { ID = 6, ParentId = 4, Description = "Sub-Manutenzione" });

                context.DbContext.MoneyCategories.Add(new Savings.Model.MoneyCategory() { ID = 7, ParentId = null, Description = "Divertimento" });
                context.DbContext.MoneyCategories.Add(new Savings.Model.MoneyCategory() { ID = 8, ParentId = 7, Description = "Sport" });
                context.DbContext.MoneyCategories.Add(new Savings.Model.MoneyCategory() { ID = 9, ParentId = 7, Description = "Spettacoli" });
                context.DbContext.SaveChanges();

                // ACT
                var sut = context.Resolve<ReportService>();

                // ASSERT
                Assert.Multiple(() =>
                {
                    Assert.Multiple(() =>
                    {
                        var result = sut.CreateGroupCategory(1);

                        Assert.That(result, Is.Not.Null.Or.Empty);
                        Assert.That(result.ID, Is.EqualTo(1), "The ID retrieved is wrong.");
                        Assert.That(result.Related, Is.Not.Empty.And.Count.EqualTo(3), "Related for Category 1 aren't right.");
                    });

                    Assert.Multiple(() =>
                    {
                        var result = sut.CreateGroupCategory(4);

                        Assert.That(result, Is.Not.Null.Or.Empty);
                        Assert.That(result.ID, Is.EqualTo(4), "The ID retrieved is wrong.");
                        Assert.That(result.Related, Is.Not.Empty.And.Count.EqualTo(2), "Related for Category 4 aren't right.");
                    });

                    Assert.Multiple(() =>
                    {
                        var result = sut.CreateGroupCategory(7);

                        Assert.That(result, Is.Not.Null.Or.Empty);
                        Assert.That(result.ID, Is.EqualTo(7), "The ID retrieved is wrong.");
                        Assert.That(result.Related, Is.Not.Empty.And.Count.EqualTo(2), "Related for Category 7 aren't right.");
                    });
                });
            }
        }
    }
}
