using Microsoft.AspNetCore.Components;
using MoneyPlan.Model.API.Report;
using MoneyPlan.SPA.Services;
using Radzen;
using Radzen.Blazor;

namespace MoneyPlan.SPA.Pages.Reports
{
    public partial class BudgetPlanReport : ComponentBase
    {
        [Inject]
        private ISavingsApi ClientAPI { get; set; }

        [CascadingParameter(Name = "FilterCategoryGroupByPeriod")]
        public string FilterCategoryGroupByPeriod { get; set; } = "yy/MM";

        [CascadingParameter(Name = "FilterDateFrom")]
        public DateTime FilterDateFrom { get; set; }

        [CascadingParameter(Name = "FilterDateTo")]
        public DateTime FilterDateTo { get; set; }

        [CascadingParameter(Name = "FilterAccount")]
        public int? FilterAccount { get; set; }

        public ReportBudgetPlan[] Data { get; set; } = Enumerable.Empty<ReportBudgetPlan>().ToArray();

        protected override async Task OnParametersSetAsync()
        {
            Data = await ClientAPI.GetBudgetPlanResume(FilterAccount,
                FilterCategoryGroupByPeriod,
                FilterDateFrom,
                FilterDateTo);
        }


        ReportBudgetPlan[] BudgetPlanResumeByAmount()
        {
            List<ReportBudgetPlan> series = Data.GroupBy(x => new { x.Description, x.TotalPercent })
                .Select(x => new ReportBudgetPlan()
                {
                    Description = x.Key.Description,
                    TotalPercent = x.Key.TotalPercent,
                    Data = x.SelectMany(gp => gp.Data)
                        .Select(x => new ReportPeriodAmountPercent { Amount = Math.Round(Math.Abs(x.Amount)), Period = x.Period, Percent = x.Percent })
                        .OrderBy(x => x.Period)
                        .ToArray()
                }).ToList();

            return series.ToArray();
        }

        ReportPeriodAmountPercent[] BudgetPlanResumeByPercent()
        {
            List<ReportPeriodAmountPercent> series = Data.GroupBy(x => new { x.Description, x.TotalPercent })
                .Select(x => new ReportPeriodAmountPercent()
                {
                    Period = x.Key.Description,
                    Percent = Math.Round((double)Math.Abs(x.Key.TotalPercent), MidpointRounding.AwayFromZero),
                    Amount = Math.Round(x.SelectMany(item => item.Data).Sum(x => x.Amount))
                })
                .ToList();

            return series.ToArray();
        }

        string FormatAsEUR(object value)
        {
            return value?.ToString();
        }

        string FormatAsPercent(object value)
        {
            return value?.ToString() + "%";
        }

        void OnClickPie(SeriesClickEventArgs args)
        {

        }
    }
}
