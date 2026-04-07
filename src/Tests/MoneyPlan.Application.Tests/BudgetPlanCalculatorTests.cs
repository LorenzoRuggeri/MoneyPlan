using MoneyPlan.Model;
using Savings.Model;

namespace MoneyPlan.Application.Tests
{
    /// <summary>
    /// Unit Tests for BudgetPlanCalculator.GroupByBudgetTypes.
    ///
    /// Conventions:
    ///   - Negative Amount  = expense (outflow)
    ///   - Positive Amount  = income  (inflow)
    ///   - periodPattern    = "yyyy-MM" throughout (one period = one month)
    ///
    /// The 50/30/20 rule is used as the reference budget plan:
    ///   Needs   = category 3 (Money Withdrawal) or category 4 (Bank Fees)
    ///   Wants   = category 5 (Clothes and shoes)
    ///   Savings = derived (income - needs - wants - orphans)
    /// </summary>
    public class BudgetPlanCalculatorTests : UnitTestBase
    {
        private const string PeriodPattern = "yyyy-MM";

        // ------------------------------------------------------------------ //
        //  SECTION 1 – Basic classification                                   //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Items with positive Amount must never appear in Needs or Wants,
        /// even when their CategoryID is covered by an existing rule.
        /// A second item with Amount >= 0 on a Needs-ruled category (cat. 3) is
        /// included explicitly to prove that the rule is not applied to inflows.
        /// Savings is expected to contain the full income (3000 + 200 = 3200)
        /// since no expenses are present and the effective saving equals total income.
        /// </summary>
        [Test]
        public void PositiveAmounts_AreClassifiedAsIncome_NotInAnyBudgetType()
        {
            using var ctx = CreateContext();
            ctx.Setup.CreateDefault();

            var items = new List<MaterializedMoneyItem>
            {
                // Generic income
                new() { Amount =  3000m, Date = new DateTime(2024, 1, 15), CategoryID = 1, Note = "" },
                // Positive amount on a Needs-ruled category: must still be ignored by Needs and Wants
                new() { Amount =   200m, Date = new DateTime(2024, 1, 20), CategoryID = 3, Note = "" }
            };

            var result = ctx.ProjectionCalculator.GroupByBudgetTypes(items, PeriodPattern).ToList();

            Assert.That(result.First(x => x.Description == "Needs").Data, Is.Empty);
            Assert.That(result.First(x => x.Description == "Wants").Data, Is.Empty);
            Assert.That(result.First(x => x.Description == "Savings").Data[0].Amount, Is.EqualTo(3200d));
        }

        /// <summary>
        /// An expense matching a Needs rule must land in the Needs bucket,
        /// with its Amount converted to a positive value.
        /// </summary>
        [Test]
        public void Expense_MatchingNeedsRule_IsClassifiedAsNeeds()
        {
            using var ctx = CreateContext();
            ctx.Setup.CreateDefault();   // category 3 => Needs

            var items = new List<MaterializedMoneyItem>
            {
                new() { Amount =  3000m, Date = new DateTime(2024, 1, 1),  CategoryID = 1, Note = "" },
                new() { Amount =  -200m, Date = new DateTime(2024, 1, 10), CategoryID = 3, Note = "" }
            };

            var result = ctx.ProjectionCalculator.GroupByBudgetTypes(items, PeriodPattern).ToList();

            var needs = result.First(x => x.Description == "Needs");
            Assert.That(needs.Data, Has.Length.EqualTo(1));
            Assert.That(needs.Data[0].Amount, Is.EqualTo(200d));
        }

        /// <summary>
        /// An expense matching a Wants rule must land in the Wants bucket,
        /// with its Amount converted to a positive value.
        /// </summary>
        [Test]
        public void Expense_MatchingWantsRule_IsClassifiedAsWants()
        {
            using var ctx = CreateContext();
            ctx.Setup.CreateDefault();   // category 5 => Wants

            var items = new List<MaterializedMoneyItem>
            {
                new() { Amount =  3000m, Date = new DateTime(2024, 1, 1),  CategoryID = 1, Note = "" },
                new() { Amount =  -150m, Date = new DateTime(2024, 1, 20), CategoryID = 5, Note = "" }
            };

            var result = ctx.ProjectionCalculator.GroupByBudgetTypes(items, PeriodPattern).ToList();

            var wants = result.First(x => x.Description == "Wants");
            Assert.That(wants.Data, Has.Length.EqualTo(1));
            Assert.That(wants.Data[0].Amount, Is.EqualTo(150d));
        }

        /// <summary>
        /// An expense that does not match any rule must be treated as an orphan
        /// and appear inside the Wants bucket (orphans are merged with Wants).
        /// </summary>
        [Test]
        public void Expense_WithNoMatchingRule_IsOrphanAndCountedInWants()
        {
            using var ctx = CreateContext();
            ctx.Setup.CreateDefault();

            // CategoryID = 99 has no rule defined
            var items = new List<MaterializedMoneyItem>
            {
                new() { Amount =  3000m, Date = new DateTime(2024, 1, 1),  CategoryID = 1,  Note = "" },
                new() { Amount =  -100m, Date = new DateTime(2024, 1, 5),  CategoryID = 99, Note = "" }
            };

            var result = ctx.ProjectionCalculator.GroupByBudgetTypes(items, PeriodPattern).ToList();

            var wants = result.First(x => x.Description == "Wants");
            Assert.That(wants.Data, Has.Length.EqualTo(1));
            Assert.That(wants.Data[0].Amount, Is.EqualTo(100d));
        }

        // ------------------------------------------------------------------ //
        //  SECTION 2 – Percentages                                            //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Per-period percentage must be computed as:
        ///   round( |category_amount| / income_for_same_period * 100, 2 )
        /// </summary>
        [Test]
        public void PerPeriodPercent_IsRelativeToIncomeOfSamePeriod()
        {
            using var ctx = CreateContext();
            ctx.Setup.CreateDefault();

            var items = new List<MaterializedMoneyItem>
            {
                new() { Amount =  1000m, Date = new DateTime(2024, 1, 1),  CategoryID = 1, Note = "" },
                new() { Amount =  -200m, Date = new DateTime(2024, 1, 10), CategoryID = 3, Note = "" }   // Needs
            };

            var result = ctx.ProjectionCalculator.GroupByBudgetTypes(items, PeriodPattern).ToList();

            var needsPeriod = result.First(x => x.Description == "Needs").Data[0];
            // 200 / 1000 * 100 = 20 %
            Assert.That(needsPeriod.Percent, Is.EqualTo(20d));
        }

        /// <summary>
        /// TotalPercent must be computed as:
        ///   round( |total_category_amount| / total_income * 100, 2 )
        /// across all periods combined.
        /// </summary>
        [Test]
        public void TotalPercent_IsRelativeToTotalIncomeAcrossAllPeriods()
        {
            using var ctx = CreateContext();
            ctx.Setup.CreateDefault();

            var items = new List<MaterializedMoneyItem>
            {
                // January
                new() { Amount =  2000m, Date = new DateTime(2024, 1, 1),  CategoryID = 1, Note = "" },
                new() { Amount =  -400m, Date = new DateTime(2024, 1, 10), CategoryID = 3, Note = "" },
                // February
                new() { Amount =  2000m, Date = new DateTime(2024, 2, 1),  CategoryID = 1, Note = "" },
                new() { Amount =  -600m, Date = new DateTime(2024, 2, 10), CategoryID = 3, Note = "" }
            };

            var result = ctx.ProjectionCalculator.GroupByBudgetTypes(items, PeriodPattern).ToList();

            // total needs = 1000, total income = 4000  => 25 %
            var needs = result.First(x => x.Description == "Needs");
            Assert.That((double)needs.TotalPercent, Is.EqualTo(25d));
        }

        [Test]
        public void PerPeriodPercent_IsRelativeToExpenses_WhenNoIncomeInThatPeriod()
        {
            using var ctx = CreateContext();
            ctx.Setup.CreateDefault();
            
            var items = new List<MaterializedMoneyItem>
            {
                // No income item for January – only an expense
                new() { Amount =  -200m, Date = new DateTime(2024, 1, 10), CategoryID = 3, Note = "" },     // Needs
                // February has income but no expenses
                new() { Amount =  200m, Date = new DateTime(2024, 2, 10), CategoryID = 1, Note = "" },
                // No income item for March but Wants 
                new() { Amount =  -200m, Date = new DateTime(2024, 3, 10), CategoryID = 5, Note = "" },      // Wants
                // No income item for April but Wants and Needs
                new() { Amount =  -200m, Date = new DateTime(2024, 4, 10), CategoryID = 3, Note = "" },     // Needs
                new() { Amount =  -200m, Date = new DateTime(2024, 4, 10), CategoryID = 5, Note = "" },      // Wants
                // Income and savings for May; no expenses
                new() { Amount =  200m, Date = new DateTime(2024, 5, 10), CategoryID = 3, Note = "" },     // Income
                new() { Amount =  -100m, Date = new DateTime(2024, 5, 10), CategoryID = 6, Note = "" },      // Savings
                // No income for June – only planned savings
                new() { Amount = -300m, Date = new DateTime(2024, 6, 10), CategoryID = 6, Note = "" },  // Savings
            };

            var result = ctx.ProjectionCalculator.GroupByBudgetTypes(items, PeriodPattern).ToList();

            var needs = result.First(x => x.Description == "Needs");
            var wants = result.First(x => x.Description == "Wants");
            var savings = result.First(x => x.Description == "Savings");

            var janNeeds = needs.Data.First(d => d.Period == "2024-01");
            var marWants = wants.Data.First(d => d.Period == "2024-03");
            var aprNeeds = needs.Data.First(d => d.Period == "2024-04");
            var aprWants = wants.Data.First(d => d.Period == "2024-04");
            var maySavings = savings.Data.First(d => d.Period == "2024-05");
            var junSavings = savings.Data.First(d => d.Period == "2024-06");

            Assert.Multiple(() =>
            {
                Assert.That(janNeeds.Percent, Is.EqualTo(100d));
                Assert.That(marWants.Percent, Is.EqualTo(100d));
                Assert.That(aprNeeds.Percent, Is.EqualTo(50d));
                Assert.That(aprWants.Percent, Is.EqualTo(50d));
                Assert.That(maySavings.Percent, Is.EqualTo(100d), "May has some problems");                
                Assert.That(junSavings.Percent, Is.EqualTo(100d), "June has some problems");
            });
        }

        // ------------------------------------------------------------------ //
        //  SECTION 3 – Savings calculation                                   //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Core formula:
        ///   effectiveSaving = (income - plannedSavings - spent) + plannedSavings
        ///                   = income - spent
        /// where spent = needs + wants + orphans.
        ///
        /// Simple case: no planned savings items.
        /// </summary>
        [Test]
        public void Savings_EqualsIncomeMinusSpent_WhenNoPlannedSavings()
        {
            using var ctx = CreateContext();
            ctx.Setup.CreateDefault();

            var items = new List<MaterializedMoneyItem>
            {
                new() { Amount =  1000m, Date = new DateTime(2024, 1, 1),  CategoryID = 1, Note = "" },
                new() { Amount =  -300m, Date = new DateTime(2024, 1, 10), CategoryID = 3, Note = "" },  // Needs
                new() { Amount =  -150m, Date = new DateTime(2024, 1, 20), CategoryID = 5, Note = "" }   // Wants
            };

            var result = ctx.ProjectionCalculator.GroupByBudgetTypes(items, PeriodPattern).ToList();

            var savings = result.First(x => x.Description == "Savings");
            Assert.That(savings.Data, Has.Length.EqualTo(1));
            // 1000 - 300 - 150 = 550
            Assert.That(savings.Data[0].Amount, Is.EqualTo(550d));
        }

        /// <summary>
        /// Savings must never be negative: when total expenses exceed income
        /// the effective saving for that period must be clamped to 0.
        /// </summary>
        [Test]
        public void Savings_IsClampedToZero_WhenExpensesExceedIncome()
        {
            using var ctx = CreateContext();
            ctx.Setup.CreateDefault();

            var items = new List<MaterializedMoneyItem>
            {
                new() { Amount =   500m, Date = new DateTime(2024, 1, 1),  CategoryID = 1, Note = "" },
                new() { Amount =  -800m, Date = new DateTime(2024, 1, 10), CategoryID = 3, Note = "" }   // Needs > income
            };

            var result = ctx.ProjectionCalculator.GroupByBudgetTypes(items, PeriodPattern).ToList();

            var savings = result.First(x => x.Description == "Savings");
            var janSavings = savings.Data.FirstOrDefault(d => d.Period == "2024-01");

            Assert.IsNotNull(janSavings);
            Assert.That(janSavings.Amount, Is.GreaterThanOrEqualTo(0d));
        }

        // ------------------------------------------------------------------ //
        //  SECTION 4 – Multi-period isolation                                 //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Each period must be calculated independently.
        /// An expense in January must not affect the percentage of February.
        /// </summary>
        [Test]
        public void MultiPeriod_EachPeriodIsCalculatedIndependently()
        {
            using var ctx = CreateContext();
            ctx.Setup.CreateDefault();

            var items = new List<MaterializedMoneyItem>
            {
                // January: 1000 income, 100 needs => 10 %
                new() { Amount =  1000m, Date = new DateTime(2024, 1, 1),  CategoryID = 1, Note = "" },
                new() { Amount =  -100m, Date = new DateTime(2024, 1, 10), CategoryID = 3, Note = "" },
                // February: 2000 income, 400 needs => 20 %
                new() { Amount =  2000m, Date = new DateTime(2024, 2, 1),  CategoryID = 1, Note = "" },
                new() { Amount =  -400m, Date = new DateTime(2024, 2, 10), CategoryID = 3, Note = "" }
            };

            var result = ctx.ProjectionCalculator.GroupByBudgetTypes(items, PeriodPattern).ToList();

            var needsData = result.First(x => x.Description == "Needs").Data;
            var jan = needsData.First(d => d.Period == "2024-01");
            var feb = needsData.First(d => d.Period == "2024-02");

            Assert.That(jan.Percent, Is.EqualTo(10d));
            Assert.That(feb.Percent, Is.EqualTo(20d));
        }

        // ------------------------------------------------------------------ //
        //  SECTION 5 – Rule priority (first matching rule wins)               //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// An item must be assigned to the FIRST matching rule only.
        /// It must not appear in multiple buckets.
        /// </summary>
        [Test]
        public void Item_IsAssignedToFirstMatchingRuleOnly()
        {
            using var ctx = CreateContext();
            ctx.Setup.CreateDefault();   // category 3 => Needs

            var items = new List<MaterializedMoneyItem>
            {
                new() { Amount =  1000m, Date = new DateTime(2024, 1, 1),  CategoryID = 1, Note = "" },
                new() { Amount =  -200m, Date = new DateTime(2024, 1, 10), CategoryID = 3, Note = "" }
            };

            var result = ctx.ProjectionCalculator.GroupByBudgetTypes(items, PeriodPattern).ToList();

            var needsTotal = result.First(x => x.Description == "Needs").Data.Sum(d => d.Amount);
            var wantsTotal = result.First(x => x.Description == "Wants").Data.Sum(d => d.Amount);

            // The 200 expense must be in Needs only, not duplicated in Wants
            Assert.That(needsTotal, Is.EqualTo(200d));
            Assert.That(wantsTotal, Is.EqualTo(0d));
        }

        // ------------------------------------------------------------------ //
        //  SECTION 6 – Empty input                                            //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// An empty source list must return three categories with no data
        /// and no exceptions.
        /// </summary>
        [Test]
        public void EmptySource_ReturnsThreeCategoriesWithNoData()
        {
            using var ctx = CreateContext();
            ctx.Setup.CreateDefault();

            var result = ctx.ProjectionCalculator
                            .GroupByBudgetTypes(Enumerable.Empty<MaterializedMoneyItem>(), PeriodPattern)
                            .ToList();

            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result.All(x => !x.Data.Any()), Is.True);
        }

        // ------------------------------------------------------------------ //
        //  SECTION 7 – Amount sign normalisation                              //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// All expense amounts stored in Needs / Wants / Savings Data
        /// must be positive, regardless of the sign of the original item.
        /// </summary>
        [Test]
        public void ExpenseAmounts_AreAlwaysPositiveInOutput()
        {
            using var ctx = CreateContext();
            ctx.Setup.CreateDefault();

            var items = new List<MaterializedMoneyItem>
            {
                new() { Amount =  3000m, Date = new DateTime(2024, 1, 1),  CategoryID = 1, Note = "" },
                new() { Amount =  -250m, Date = new DateTime(2024, 1, 5),  CategoryID = 3, Note = "" },  // Needs
                new() { Amount =  -100m, Date = new DateTime(2024, 1, 6),  CategoryID = 5, Note = "" },  // Wants
                new() { Amount =   -80m, Date = new DateTime(2024, 1, 7),  CategoryID = 99, Note = "" }  // Orphan
            };

            var result = ctx.ProjectionCalculator.GroupByBudgetTypes(items, PeriodPattern).ToList();

            foreach (var category in result)
                Assert.That(category.Data.All(d => d.Amount >= 0), Is.True,
                    $"Negative amount found in category '{category.Description}'");
        }

        // ------------------------------------------------------------------ //
        //  SECTION 8 – TotalPercent invariance across PeriodPatterns          //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// When no period has expenses exceeding income, the clamping never triggers
        /// and TotalPercent is identical between monthly and yearly patterns.
        /// This is the base case — it confirms correctness when no deficit period exists.
        /// See TotalPercent_Savings_DiffersBetweenPatterns_WhenExpensesExceedIncomeInSomePeriods
        /// for the case where clamping does occur and patterns legitimately diverge.
        /// </summary>
        [Test]
        public void TotalPercent_IsInvariant_AcrossDifferentPeriodPatterns_WhenNoDeficitPeriodExists()
        {
            using var ctx = CreateContext();
            ctx.Setup.CreateDefault();

            // Two months of data to make the yearly vs monthly distinction meaningful
            var items = new List<MaterializedMoneyItem>
            {
                // January
                new() { Amount =  2000m, Date = new DateTime(2024, 1,  1), CategoryID = 1, Note = "" },
                new() { Amount =  -400m, Date = new DateTime(2024, 1, 10), CategoryID = 3, Note = "" },  // Needs
                new() { Amount =  -200m, Date = new DateTime(2024, 1, 20), CategoryID = 5, Note = "" },  // Wants
                // February
                new() { Amount =  2000m, Date = new DateTime(2024, 2,  1), CategoryID = 1, Note = "" },
                new() { Amount =  -600m, Date = new DateTime(2024, 2, 10), CategoryID = 3, Note = "" },  // Needs
                new() { Amount =  -100m, Date = new DateTime(2024, 2, 20), CategoryID = 5, Note = "" },  // Wants
            };

            var monthly = ctx.ProjectionCalculator.GroupByBudgetTypes(items, "yyyy-MM").ToList();
            var yearly = ctx.ProjectionCalculator.GroupByBudgetTypes(items, "yyyy").ToList();

            foreach (var description in new[] { "Needs", "Wants", "Savings" })
            {
                var monthlyPercent = monthly.First(x => x.Description == description).TotalPercent;
                var yearlyPercent = yearly.First(x => x.Description == description).TotalPercent;

                Assert.That(yearlyPercent, Is.EqualTo(monthlyPercent),
                    $"TotalPercent mismatch for '{description}': monthly={monthlyPercent}, yearly={yearlyPercent}");
            }
        }

        /// <summary>
        /// Per-period Percent IS expected to differ between monthly and yearly patterns,
        /// because each period aggregates a different number of items.
        /// With monthly grouping January and February are two separate periods;
        /// with yearly grouping they collapse into one. This test documents and
        /// asserts that intentional difference.
        /// </summary>
        [Test]
        public void PerPeriodPercent_DiffersBetweenMonthlyAndYearly_AsExpected()
        {
            using var ctx = CreateContext();
            ctx.Setup.CreateDefault();

            var items = new List<MaterializedMoneyItem>
            {
                // January: 2000 income, 400 needs => 20%
                new() { Amount =  2000m, Date = new DateTime(2024, 1,  1), CategoryID = 1, Note = "" },
                new() { Amount =  -400m, Date = new DateTime(2024, 1, 10), CategoryID = 3, Note = "" },
                // February: 2000 income, 600 needs => 30%
                new() { Amount =  2000m, Date = new DateTime(2024, 2,  1), CategoryID = 1, Note = "" },
                new() { Amount =  -600m, Date = new DateTime(2024, 2, 10), CategoryID = 3, Note = "" },
            };

            var monthly = ctx.ProjectionCalculator.GroupByBudgetTypes(items, "yyyy-MM").ToList();
            var yearly = ctx.ProjectionCalculator.GroupByBudgetTypes(items, "yyyy").ToList();

            var monthlyNeeds = monthly.First(x => x.Description == "Needs").Data;
            var yearlyNeeds = yearly.First(x => x.Description == "Needs").Data;

            // Monthly: two periods with different percentages
            Assert.That(monthlyNeeds.First(d => d.Period == "2024-01").Percent, Is.EqualTo(20d));
            Assert.That(monthlyNeeds.First(d => d.Period == "2024-02").Percent, Is.EqualTo(30d));

            // Yearly: one period — 1000 needs / 4000 income = 25%
            Assert.That(yearlyNeeds, Has.Length.EqualTo(1));
            Assert.That(yearlyNeeds[0].Percent, Is.EqualTo(25d));
        }

        /// <summary>
        /// When expenses exceed income in one period, the saving for that period is
        /// clamped to 0. This means TotalPercent for Savings will differ between
        /// monthly and yearly patterns — that is the CORRECT and EXPECTED behaviour.
        /// The deficit period does not compensate other periods.
        /// Since TotalPercent is computed against totalBase (needs + wants + savings)
        /// rather than totalIncome, ALL three categories will differ between patterns
        /// when clamping changes the totalBase.
        /// </summary>
        [Test]
        public void TotalPercent_Savings_DiffersBetweenPatterns_WhenExpensesExceedIncomeInSomePeriods()
        {
            using var ctx = CreateContext();
            ctx.Setup.CreateDefault();

            var items = new List<MaterializedMoneyItem>
            {
                // January: income 500, needs 900 => expenses exceed income, savings clamped to 0
                new() { Amount =   500m, Date = new DateTime(2024, 1,  1), CategoryID = 1, Note = "" },
                new() { Amount =  -900m, Date = new DateTime(2024, 1, 10), CategoryID = 3, Note = "" },  // Needs
                // February: income 2000, needs 200 => savings 1800
                new() { Amount =  2000m, Date = new DateTime(2024, 2,  1), CategoryID = 1, Note = "" },
                new() { Amount =  -200m, Date = new DateTime(2024, 2, 10), CategoryID = 3, Note = "" },  // Needs
            };

            var monthly = ctx.ProjectionCalculator.GroupByBudgetTypes(items, "yyyy-MM").ToList();
            var yearly = ctx.ProjectionCalculator.GroupByBudgetTypes(items, "yyyy").ToList();

            // Savings differ between patterns because clamping changes totalBase
            var monthlySavings = monthly.First(x => x.Description == "Savings").TotalPercent;
            var yearlySavings = yearly.First(x => x.Description == "Savings").TotalPercent;
            Assert.That(monthlySavings, Is.Not.EqualTo(yearlySavings),
                "Savings TotalPercent should differ between patterns when clamping occurs in some periods");

            // Each pattern must still sum to 100
            Assert.That((double)monthly.Sum(x => x.TotalPercent), Is.EqualTo(100d).Within(0.01));
            Assert.That((double)yearly.Sum(x => x.TotalPercent), Is.EqualTo(100d).Within(0.01));
        }

        // ------------------------------------------------------------------ //
        //  SECTION 9 – TotalPercent sum always equals 100                     //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// The sum of TotalPercent across Needs, Wants and Savings must always
        /// equal 100, regardless of the PeriodPattern and regardless of whether
        /// the user has drawn from prior liquidity in some periods.
        /// This holds because TotalPercent is computed against the total distributed
        /// amount (needs + wants + savings) rather than against totalIncome.
        /// </summary>
        [Test]
        public void TotalPercent_SumOfAllCategories_IsAlways100()
        {
            using var ctx = CreateContext();
            ctx.Setup.CreateDefault();

            var items = new List<MaterializedMoneyItem>
            {
                // January: normal period
                new() { Amount =  2000m, Date = new DateTime(2024, 1,  1), CategoryID = 1, Note = "" },
                new() { Amount =  -400m, Date = new DateTime(2024, 1, 10), CategoryID = 3, Note = "" },  // Needs
                new() { Amount =  -200m, Date = new DateTime(2024, 1, 20), CategoryID = 5, Note = "" },  // Wants
                // February: expenses exceed income — prior liquidity is used
                new() { Amount =   500m, Date = new DateTime(2024, 2,  1), CategoryID = 1, Note = "" },
                new() { Amount =  -900m, Date = new DateTime(2024, 2, 10), CategoryID = 3, Note = "" },  // Needs > income
                new() { Amount =  -200m, Date = new DateTime(2024, 2, 20), CategoryID = 5, Note = "" },  // Wants
            };

            foreach (var pattern in new[] { "yyyy-MM", "yyyy" })
            {
                var result = ctx.ProjectionCalculator.GroupByBudgetTypes(items, pattern).ToList();

                var sum = result.Sum(x => x.TotalPercent);

                Assert.That((double)sum, Is.EqualTo(100d).Within(0.01),
                    $"Sum of TotalPercent is {sum} instead of 100 with pattern '{pattern}'");
            }
        }
    }
}