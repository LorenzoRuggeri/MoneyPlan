using Microsoft.AspNetCore.Components;
using Radzen;
using Savings.Model;
using MoneyPlan.SPA.Services;
using System.ComponentModel;
using System.Linq.Dynamic.Core;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Radzen.Blazor;
using MoneyPlan.Model.API.Report;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using Microsoft.AspNetCore.Components.Web;
using MoneyPlan.SPA.Pages.Reports;

namespace MoneyPlan.SPA.Pages
{
    public partial class ReportsList : ComponentBase
    {

        [Inject]
        private ISavingsApi savingsAPI { get; set; }

        [Inject]
        public DialogService dialogService { get; set; }

        [Inject]
        public Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }

        public string FilterCategoryGroupByPeriod { get; set; } = "yy/MM";

        public DateTime FilterDateFrom { get; set; }

        public DateTime FilterDateTo { get; set; }

        public int? FilterAccount { get; set; }

        public MoneyAccount[] Accounts { get; private set; }

        public bool IsLoading { get; set; } = true;

        public Type ReportType { get; set; }

        async void OnFromDateChanged(DateTime dateTime)
        {
            FilterDateFrom = new DateTime(dateTime.Year, dateTime.Month, 1);
            StateHasChanged();
        }

        async void OnToDateChanged(DateTime dateTime)
        {
            FilterDateTo = dateTime.EndOfMonth();
            StateHasChanged();
        }

        void DateTimeDateChanged(DateTime? value, string name)
        {
            StateHasChanged();
        }

        void FilterGroupByCategoryPeriodChanged(ChangeEventArgs e)
        {
            var selectedString = e.Value.ToString();
            FilterCategoryGroupByPeriod = string.IsNullOrWhiteSpace(selectedString) ? null : selectedString;
            StateHasChanged();
        }

        async void AccountChanged(object model)
        {
            await localStorage.SetItemAsync("Search.AccountId", FilterAccount);
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            var today = DateTime.Now;
            
            FilterDateTo = today.EndOfMonth();
            FilterDateFrom = FilterDateTo.AddYears(-1).StartOfMonth();
            FilterAccount = await localStorage.GetItemAsync<int?>("Search.AccountId") ?? null;
            Accounts = await savingsAPI.GetMoneyAccounts();
            IsLoading = false;
        }

        void OnClickBudgetPlan(MouseEventArgs args)
        {
            ReportType = typeof(BudgetPlanReport);
            StateHasChanged();
        }

        void OnClickCategories(MouseEventArgs args)
        {
            ReportType = typeof(CategoriesReport);
            StateHasChanged();
        }

        void OnClickBackButton(MouseEventArgs args)
        {
            ReportType = null;
            StateHasChanged();
        }

    }

}
