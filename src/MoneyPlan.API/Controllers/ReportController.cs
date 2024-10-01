using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Savings.DAO.Infrastructure;
using Savings.API.Services.Abstract;
using Savings.Model;
using System.Linq.Expressions;
using Savings.API.Services;
using Microsoft.Identity.Client;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MoneyPlan.Model.API.Report;
using MoneyPlan.Business;

namespace Savings.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly SavingsContext _context;
        private readonly IProjectionCalculator calculator;
        private readonly ReportService reportService;

        public ReportController(IProjectionCalculator calculator, ReportService reportService, SavingsContext context)
        {
            _context = context;
            this.calculator = calculator;
            this.reportService = reportService;
        }

        [HttpGet("GetBudgetPlanResume")]
        public async Task<ActionResult<ReportBudgetPlan[]>> GetBudgetPlanResume(int? accountId, string periodPattern, DateTime dateFrom, DateTime dateTo)
        {
            var projectionItems = (await calculator.CalculateAsync(accountId, dateFrom, dateTo, null, false, false))
                .Where(x => x.EndPeriod == false);

            Func<BudgetPlanRule, MaterializedMoneyItem, bool> filter = (rule, moneyItem) =>
            {
                if (moneyItem.Amount >= 0)
                {
                    return false;
                }
                else if (!string.IsNullOrEmpty(rule.CategoryText) && rule.CategoryId == moneyItem.CategoryID)
                {
                    if (rule.CategoryFilter == StringFilterType.Contains)
                        return moneyItem.Note.Contains(rule.CategoryText, StringComparison.OrdinalIgnoreCase);
                    else if (rule.CategoryFilter == StringFilterType.Equals)
                        return moneyItem.Note.Equals(rule.CategoryText, StringComparison.OrdinalIgnoreCase);
                }
                else if (rule.CategoryId == moneyItem.CategoryID)
                {
                    return true;
                }
                return false;
            };

            var budgetPlan = _context.BudgetPlans.FirstOrDefault();
            List<MaterializedMoneyItem> income = new List<MaterializedMoneyItem>();
            List<MaterializedMoneyItem> needs = new List<MaterializedMoneyItem>();
            List<MaterializedMoneyItem> wants = new List<MaterializedMoneyItem>();
            List<MaterializedMoneyItem> savings = new List<MaterializedMoneyItem>();
            List<MaterializedMoneyItem> orphans = new List<MaterializedMoneyItem>(projectionItems);

            // Order by rules, to be sure they properly catch the items they tends to.
            var orderedRules = _context.RelationshipBudgetPlanToRules
                .Include(x => x.BudgetPlanRule)
                .Select(x => x.BudgetPlanRule)
                .OrderBy(x => x.CategoryId)
                //.ThenByDescending(x => x.CategoryFilter)
                .ToList();

            var incomePartial = orphans.Where(item => item.Amount >= 0).ToList();
            income.AddRange(incomePartial);
            orphans = orphans.Except(incomePartial).ToList();

            foreach (var rule in orderedRules)
            {
                var partialNeeds = orphans.Where(item => filter(rule, item) && rule.Type == BudgetPlanType.Needs).ToList();
                needs.AddRange(partialNeeds);
                orphans = orphans.Except(partialNeeds).ToList();

                var partialWants = orphans.Where(item => filter(rule, item) && rule.Type == BudgetPlanType.Wants).ToList();
                wants.AddRange(partialWants);
                orphans = orphans.Except(partialWants).ToList();

                var partialSavings = orphans.Where(item => filter(rule, item) && rule.Type == BudgetPlanType.Savings).ToList();
                savings.AddRange(partialSavings);
                orphans = orphans.Except(partialSavings).ToList();

                // If we correctly distribuited all data, we can exit.
                if (!orphans.Any())
                    break;
            }

            var total = income.Sum(x => x.Amount);
            var spent = needs.Union(wants).Union(savings).Union(orphans).Sum(x => Math.Abs(x.Amount));
            var needsPercent = Math.Round((needs.Sum(x => x.Amount) / total) * 100, 2);
            var wantsPercent = Math.Round((wants.Sum(x => x.Amount) / total) * 100, 2);
            var savingsPercent = Math.Round((savings.Sum(x => x.Amount) / total) * 100, 2);
            var undefinedPercent = Math.Round((orphans.Sum(x => x.Amount) / total) * 100, 2);

            var cashFlow = total - spent;
            var cashFlowPercent = Math.Round((cashFlow / total) * 100, 2);

            var incomePeriodPercents = income.GroupBy(x => x.Date.ToString(periodPattern))
                .Select(x => new ReportPeriodAmountPercent() { Period = x.Key, Amount = (double)x.Sum(y => y.Amount) });

            // Function to retrieve the relative percentage against the income.
            Func<IGrouping<string, MaterializedMoneyItem>, double> FindPercent = (periodGroup) =>
            {
                var found = incomePeriodPercents.FirstOrDefault(inc => inc.Period == periodGroup.Key);
                if (found == null)
                    return 0;
                return Math.Abs(Math.Round(((double)periodGroup.Sum(y => y.Amount) / found.Amount) * 100, 2));
            };

            Func<IEnumerable<ReportPeriodAmountPercent>> CalculateLiquidityForPeriod = () =>
            {
                var spentByPeriod = savings.GroupBy(x => x.Date.ToString(periodPattern))
                    .Select(x => new ReportPeriodAmountPercent()
                    {
                        Period = x.Key,
                        Amount = (double)x.Sum(y => y.Amount)
                    })
               .Union(wants.GroupBy(x => x.Date.ToString(periodPattern))
                    .Select(x => new ReportPeriodAmountPercent()
                    {
                        Period = x.Key,
                        Amount = (double)x.Sum(y => y.Amount)
                    }))
                .Union(needs.GroupBy(x => x.Date.ToString(periodPattern))
                    .Select(x => new ReportPeriodAmountPercent()
                    {
                        Period = x.Key,
                        Amount = (double)x.Sum(y => y.Amount)
                    }))
                .Union(orphans.GroupBy(x => x.Date.ToString(periodPattern))
                    .Select(x => new ReportPeriodAmountPercent()
                    {
                        Period = x.Key,
                        Amount = (double)x.Sum(y => y.Amount)
                    }));

                return spentByPeriod.GroupBy(x => x.Period)
                    .Select(x =>
                    {
                        var incomePartial = incomePeriodPercents.FirstOrDefault(inc => inc.Period == x.Key);
                        var incomeAmount = (incomePartial?.Amount ?? 0);
                        var spentPartial = Math.Abs(x.Sum(item => item.Amount));
                        var spentAmount = incomeAmount - spentPartial;
                        var result = new ReportPeriodAmountPercent()
                        {
                            Period = x.Key,
                            Amount = spentAmount,
                            Percent = Math.Round((spentAmount / incomeAmount) * 100, 2)
                        };
                        return result;
                    })
                    .Where(x => x.Percent > 0);     // Excluding not displayable values.
            };

            List<ReportBudgetPlan> list = new List<ReportBudgetPlan>();
            list.Add(new ReportBudgetPlan()
            {
                Description = "Needs",
                TotalPercent = needsPercent,
                Data = needs.GroupBy(x => x.Date.ToString(periodPattern))
                    .Select(x => new ReportPeriodAmountPercent() { Period = x.Key, Amount = (double)x.Sum(y => y.Amount), Percent = FindPercent(x) }).ToArray()
            });
            list.Add(new ReportBudgetPlan()
            {
                Description = "Wants",
                TotalPercent = wantsPercent,
                Data = wants.GroupBy(x => x.Date.ToString(periodPattern))
                    .Select(x => new ReportPeriodAmountPercent() { Period = x.Key, Amount = (double)x.Sum(y => y.Amount), Percent = FindPercent(x) }).ToArray()
            });
            list.Add(new ReportBudgetPlan()
            {
                Description = "Savings",
                TotalPercent = savingsPercent,
                Data = savings.GroupBy(x => x.Date.ToString(periodPattern))
                    .Select(x => new ReportPeriodAmountPercent() { Period = x.Key, Amount = (double)x.Sum(y => y.Amount), Percent = FindPercent(x) }).ToArray()
            });
            list.Add(new ReportBudgetPlan()
            {
                Description = "T.B.D.",
                TotalPercent = undefinedPercent,
                Data = orphans.GroupBy(x => x.Date.ToString(periodPattern))
                    .Select(x => new ReportPeriodAmountPercent() { Period = x.Key, Amount = (double)x.Sum(y => y.Amount), Percent = FindPercent(x) }).ToArray()
            });

            var liquidityData = CalculateLiquidityForPeriod();
            if (liquidityData.Any())
            {
                list.Add(new ReportBudgetPlan()
                {
                    Description = "Liquidity",
                    TotalPercent = Math.Abs(cashFlowPercent),
                    Data = liquidityData.ToArray()
                });
            }

            return list.ToArray();
        }

        [HttpGet("GetCategoryResumeDetail")]
        public async Task<ActionResult<ReportDetail[]>> GetCategoryResumeDetail(int? accountId, string periodPattern, DateTime dateFrom, DateTime dateTo, long? category, string period)
        {
            IEnumerable<ReportFullDetail> details = await GetCategoryDetailsAsync(accountId, periodPattern, dateFrom, dateTo);

            var res = reportService.GetDetailsGroupedByCategory(details, category, period);

            /*
            var res = details
                .Where(x => x.CategoryID == category && x.Period == period)
                .Select(x => new ReportDetail { Amount = x.Amount, Date = x.Date, Description = x.Description });
            */

            return res.ToArray();
        }

        [HttpGet("GetCategoryResume")]
        public async Task<ActionResult<ReportCategory[]>> GetCategoryResume(int? accountId, string periodPattern, DateTime dateFrom, DateTime dateTo)
        {
            var categories = _context.MoneyCategories.ToList();
            IEnumerable<ReportFullDetail> union = await GetCategoryDetailsAsync(accountId, periodPattern, dateFrom, dateTo);

            // TODO: move this logic to a service that will be used elsewhere; while interacting MoneyCategories this should be the behaviour
            //       we want.
            var test = reportService.GetFullDetailsGroupedByCategory(union)
                .GroupBy(x => x.GroupCategory)
                .Where(x => x.ToList().Any(y => y.Details.Sum(z => z.Amount) != 0));

            var lstStatistics = new List<ReportCategory>();

            foreach (var item in test)
            {
                var cat = test.FirstOrDefault(x => x.Key.ID == item.Key.ID);

                lstStatistics.Add(new ReportCategory
                {
                    CategoryID = item.Key?.ID,
                    Category = item.Key?.Description,
                    //CategoryIcon = category.Key?.Icon,
                    Data = item.SelectMany(x => x.Details)
                        .GroupBy(x => x.Period)
                        .Select(x => new ReportPeriodAmount() { Period = x.Key, Amount = (double)x.Sum(y => y.Amount) })
                        .Where(x => x.Amount != 0)
                        .ToArray()
                });
            }

            return lstStatistics.OrderBy(x => x.Category).ToArray();

            /*
            var categoryAmounts = union.GroupBy(x => x.CategoryID).Where(x => x.Sum(y => y.Amount) != 0);

            var lstStatistics = new List<ReportCategory>();
            foreach (var category in categoryAmounts)
            {
                var cat = categories.FirstOrDefault(x => x.ID == category.Key);
                lstStatistics.Add(new ReportCategory
                {
                    CategoryID = category.Key,
                    Category = cat?.Description,
                    CategoryIcon = cat?.Icon,
                    Data = category
                            .GroupBy(x => x.Period)
                            .Select(x => new ReportPeriodAmount() { Period = x.Key, Amount = (double)x.Sum(y => y.Amount) })
                            .Where(x => x.Amount != 0)
                            .ToArray()
                });
            }
            
            return lstStatistics.OrderBy(x => x.Category).ToArray();
            */
        }

        private async Task<IEnumerable<ReportFullDetail>> GetCategoryDetailsAsync(int? accountId, string periodPattern, DateTime dateFrom, DateTime dateTo)
        {
            var projectionItems = await calculator.CalculateAsync(accountId, null, dateTo, null, false);
            var withdrawalID = _context.Configuration.FirstOrDefault()?.CashWithdrawalCategoryID;

            Expression<Func<MaterializedMoneyItem, bool>> firstLevelPredicate = (x) => x.Date >= dateFrom && x.Date <= dateTo && x.CategoryID != withdrawalID && !x.EndPeriod;

            var projectionItemsFirstLevel = projectionItems
                .Where(firstLevelPredicate.Compile())
                .Select(x => new ReportFullDetail { Type = "ProjL1", ID = x.ID, Date = x.Date, Period = x.Date.ToString(periodPattern), Description = x.Note, CategoryID = x.CategoryID, Amount = x.Amount })
                .ToList();

            var materializedItemsFirstLevel = await _context.MaterializedMoneyItems
                .Include(x => x.Category)
                .Include(x => x.RecurrentMoneyItem)
                .Include(x => x.FixedMoneyItem)
                .Where(x => accountId.HasValue ?
                    (x.FixedMoneyItemID.HasValue ? x.FixedMoneyItem.AccountID == accountId : true ||
                    x.RecurrentMoneyItemID.HasValue ? x.RecurrentMoneyItem.MoneyAccountId == accountId : true) :
                true)
                .Where(firstLevelPredicate)
                .OrderByDescending(x => x.ID)
                .Select(x => new ReportFullDetail { Type = "MaterL1", ID = x.ID, Date = x.Date, Period = x.Date.ToString(periodPattern), Description = x.Note, CategoryID = x.CategoryID, Amount = x.Amount })
                .ToListAsync();

            var union = projectionItemsFirstLevel.Union(materializedItemsFirstLevel);
            return union;
        }
    }
}
