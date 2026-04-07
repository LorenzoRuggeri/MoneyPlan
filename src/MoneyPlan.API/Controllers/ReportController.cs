using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Savings.DAO.Infrastructure;
using Savings.API.Services.Abstract;
using Savings.Model;
using System.Linq.Expressions;
using Savings.API.Services;
using Microsoft.Identity.Client;
using MoneyPlan.Business;
using System.Linq;
using System.Numerics;
using MoneyPlan.Model;
using MoneyPlan.Application.Abstractions.Models.Report;
using MoneyPlan.Application.Abstractions.Budgeting;

namespace Savings.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly SavingsContext _context;
        private readonly IBudgetPlanCalculator budgetPlanCalc;
        private readonly IProjectionCalculator calculator;
        private readonly ReportService reportService;

        public ReportController(IProjectionCalculator calculator, ReportService reportService, SavingsContext context, IBudgetPlanCalculator budgetPlanCalc)
        {
            _context = context;
            this.budgetPlanCalc = budgetPlanCalc;
            this.calculator = calculator;
            this.reportService = reportService;
        }

        [HttpGet("GetBudgetPlanResume")]
        public async Task<ActionResult<ReportBudgetPlanType[]>> GetBudgetPlanResume(int? accountId, string periodPattern, DateTime dateFrom, DateTime dateTo)
        {
            var projectionItems = (await calculator.CalculateAsync(accountId, dateFrom, dateTo, null, false, false))
                .Where(x => x.EndPeriod == false);

            var groups = budgetPlanCalc.GroupByBudgetTypes(projectionItems, periodPattern);
            return groups.ToArray();
        }

        [HttpGet("GetCategoryResumeDetail")]
        public async Task<ActionResult<ReportDetail[]>> GetCategoryResumeDetail(int? accountId, string periodPattern, DateTime dateFrom, DateTime dateTo, long? category, string period)
        {
            IEnumerable<ReportCategoryRow> details = await GetCategoryDetailsAsync(accountId, periodPattern, dateFrom, dateTo);

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
            IEnumerable<ReportCategoryRow> union = await GetCategoryDetailsAsync(accountId, periodPattern, dateFrom, dateTo);

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

        private async Task<IEnumerable<ReportCategoryRow>> GetCategoryDetailsAsync(int? accountId, string periodPattern, DateTime dateFrom, DateTime dateTo)
        {
            var projectionItems = await calculator.CalculateAsync(accountId, null, dateTo, null, false);
            var withdrawalID = _context.Configuration.FirstOrDefault()?.CashWithdrawalCategoryID;

            Expression<Func<MaterializedMoneyItem, bool>> firstLevelPredicate = (x) => x.Date >= dateFrom && x.Date <= dateTo && x.CategoryID != withdrawalID && !x.EndPeriod;

            var projectionItemsFirstLevel = projectionItems
                .Where(firstLevelPredicate.Compile())
                .Select(x => new ReportCategoryRow { Type = "ProjL1", ID = x.ID, Date = x.Date, Period = x.Date.ToString(periodPattern), Description = x.Note, CategoryID = x.CategoryID, Amount = x.Amount })
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
                .Select(x => new ReportCategoryRow { Type = "MaterL1", ID = x.ID, Date = x.Date, Period = x.Date.ToString(periodPattern), Description = x.Note, CategoryID = x.CategoryID, Amount = x.Amount })
                .ToListAsync();

            var union = projectionItemsFirstLevel.Union(materializedItemsFirstLevel);
            return union;
        }
    }
}
