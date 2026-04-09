using Microsoft.AspNetCore.Components;

namespace MoneyPlan.SPA.Components
{
    public abstract class SkeletonReport : ComponentBase
    {
        [CascadingParameter(Name = "FilterPeriodPattern")]
        public string FilterPeriodPattern { get; set; } = "yy/MM";

        [CascadingParameter(Name = "FilterDateFrom")]
        public DateTime FilterDateFrom { get; set; }

        [CascadingParameter(Name = "FilterDateTo")]
        public DateTime FilterDateTo { get; set; }

        [CascadingParameter(Name = "FilterAccount")]
        public int? FilterAccount { get; set; }
    }
}
