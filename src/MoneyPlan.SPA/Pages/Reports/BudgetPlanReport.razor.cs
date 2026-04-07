using Microsoft.AspNetCore.Components;
using MoneyPlan.Application.Abstractions.Models.Report;
using MoneyPlan.SPA.Services;
using Radzen;
using Radzen.Blazor;
using Refit;

namespace MoneyPlan.SPA.Pages.Reports
{
    public partial class BudgetPlanReport : ComponentBase
    {
        [CascadingParameter(Name = "FilterCategoryGroupByPeriod")]
        public string FilterCategoryGroupByPeriod { get; set; } = "yy/MM";

        [CascadingParameter(Name = "FilterDateFrom")]
        public DateTime FilterDateFrom { get; set; }

        [CascadingParameter(Name = "FilterDateTo")]
        public DateTime FilterDateTo { get; set; }

        [CascadingParameter(Name = "FilterAccount")]
        public int? FilterAccount { get; set; }

        public ReportBudgetPlanType[] Data { get; set; } = Enumerable.Empty<ReportBudgetPlanType>().ToArray();

        protected override async Task OnParametersSetAsync()
        {
            var response = await APIClient.GetBudgetPlanResume(FilterAccount,
                    FilterCategoryGroupByPeriod,
                    FilterDateFrom,
                    FilterDateTo);
            if (response.IsSuccessStatusCode)
            {
                Data = response.Content;
            }
        }


        ReportBudgetPlanType[] BudgetPlanResumeByAmount()
        {
            List<ReportBudgetPlanType> series = Data.GroupBy(x => new { x.Description, x.TotalPercent })
                .Select(x =>
                {
                    return new ReportBudgetPlanType()
                    {
                        Description = x.Key.Description,
                        TotalPercent = x.Key.TotalPercent,
                        Data = x.SelectMany(gp => gp.Data)
                        .Select(x => new ReportPeriodAmountPercent { Amount = Math.Round(Math.Abs(x.Amount)), Period = x.Period, Percent = x.Percent })
                        .OrderBy(x => x.Period)
                        .ToArray()
                    };
                }).ToList();

            return series.ToArray();
        }

        ReportBudgetPlanType[] TotalPieChartData()
        {
            return Data ?? [];
        }

        string FormatAsEUR(object value)
        {
            return value?.ToString();
        }

        string FormatForAmount(object amount)
        {
            return Math.Round((double)amount, 0).ToString();
        }

        string FormatForPercent(object percent)
        {
            return Math.Round((double)percent, 1, MidpointRounding.AwayFromZero).ToString();
        }


        void OnClickPie(SeriesClickEventArgs args)
        {

        }
    }
}
