﻿using Microsoft.AspNetCore.Components;
using Savings.Model;
using MoneyPlan.SPA.Services;

namespace MoneyPlan.SPA.Pages.Reports
{
    public partial class CategoryDetail : ComponentBase
    {

        [Inject]
        public ISavingsApi savingsAPI { get; set; }

        [Parameter]
        public DateTime FilterDateFrom { get; set; }

        [Parameter]
        public DateTime FilterDateTo { get; set; }

        [Parameter]
        public long? category { get; set; } = null;

        [Parameter]
        public string periodPattern { get; set; } = null;

        [Parameter]
        public string period { get; set; } = null;

        [Parameter]
        public int? account { get; set; } = null;

        public ReportDetail[] ReportCategoryDetails { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitializeList();
        }

        async Task InitializeList()
        {
            ReportCategoryDetails = await savingsAPI.GetCategoryResumeDetail(account, periodPattern, FilterDateFrom, FilterDateTo, category, period);
        }

    }
}
