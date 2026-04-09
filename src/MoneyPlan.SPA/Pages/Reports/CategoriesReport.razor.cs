using Microsoft.AspNetCore.Components;
using MoneyPlan.SPA.Components;
using MoneyPlan.SPA.Services;
using Radzen;
using Radzen.Blazor;
using Savings.Model;

namespace MoneyPlan.SPA.Pages.Reports
{
    public partial class CategoriesReport : SkeletonReport
    {
        [Inject]
        public DialogService dialogService { get; set; }

        public ReportCategory[] Source { get; set; } = Enumerable.Empty<ReportCategory>().ToArray();

        protected override async Task OnParametersSetAsync()
        {
            Source = await APIClient.GetCategoryResume(FilterAccount, FilterPeriodPattern, FilterDateFrom, FilterDateTo);
        }

        async Task OpenDetails(long? category, string period)
        {
            var res = await dialogService.OpenAsync<CategoryDetail>($"Report details",
                           new Dictionary<string, object>() {
                               { "account", FilterAccount },
                               { "FilterDateFrom", FilterDateFrom },
                               { "FilterDateTo", FilterDateTo },
                               { "category", (long?)category },
                               { "periodPattern", FilterPeriodPattern },
                               { "period", period }
                           },
                            new DialogOptions() { Width = "800px", Height = "600px" });
        }


        ReportCategory[] FilterStatisticsResume()
        {
            var outgoing = Source
                .SelectMany(x => x.Data)
                .Where(x => x.Amount < 0)
                .GroupBy(x => x.Period)
                .Select(x => new ReportPeriodAmount { Period = x.Key, Amount = Math.Abs(x.Sum(y => y.Amount)) });

            var incoming = Source
               .SelectMany(x => x.Data)
               .Where(x => x.Amount > 0)
               .GroupBy(x => x.Period)
               .Select(x => new ReportPeriodAmount { Period = x.Key, Amount = x.Sum(y => y.Amount) });

            var gain = incoming
                      .Join(outgoing, inc => inc.Period, outg => outg.Period, (incItem, outgItem) => new { incItem, outgItem })
                      .Select(x => new ReportPeriodAmount { Period = x.incItem.Period, Amount = x.incItem.Amount - x.outgItem.Amount });


            return new ReportCategory[]
            {
                new ReportCategory { Category="Outgoing", Data=outgoing.OrderBy(x=>x.Period).ToArray() },
                new ReportCategory { Category="Incoming", Data=incoming.OrderBy(x=>x.Period).ToArray() },
                new ReportCategory { Category="Gain", Data=gain.OrderBy(x=>x.Period).ToArray() }
            };
        }

        string FormatAsAmount(object value)
        {
            return ((double)value).ToString("N2");
        }

        string FormatAsMonth(object value)
        {
            return value?.ToString();
        }

    }
}
