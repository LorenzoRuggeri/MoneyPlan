using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using Savings.Model;
using MoneyPlan.SPA.Services;
using Radzen.Blazor;

namespace MoneyPlan.SPA.Pages.Settings
{
    public partial class BudgetPlanRuleEdit : ComponentBase
    {

        [Inject]
        public ISavingsApi savingsAPI { get; set; }

        [Inject]
        DialogService dialogService { get; set; }

        [Inject]
        public NotificationService notificationService { get; set; }

        [Parameter]
        public BudgetPlanRule Model { get; set; }

        [Parameter]
        public bool isNew { get; set; }

        public MoneyCategory[] Categories { get; set; }

        RadzenDropDown<int> accountSelector;

        protected override void OnInitialized()
        {
            if (isNew)
            {
                this.Model.Type = BudgetPlanType.Needs;
            }
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                if (!isNew)
                {
                    accountSelector.Disabled = true;
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            Categories = await savingsAPI.GetMoneyCategories();
        }

        bool ValidateData()
        {
            if (Model.CategoryId == default)
            {
                notificationService.Notify(NotificationSeverity.Error, "Attention", "Category is mandatory field");
                return false;
            }
            return true;
        }

        async Task Delete()
        {
            var res = await dialogService.Confirm("Are you sure you want delete?", "Delete budget rule", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });
            if (res.HasValue && res.Value)
            {
                try
                {
                    await savingsAPI.DeleteBudgetPlanRule(Model.Id);
                    this.dialogService.Close(true);
                }
                catch(Refit.ApiException ex)
                {
                    await this.dialogService.Alert(ex.HasContent ? ex.Content : ex.ReasonPhrase, "Error while deleting");
                }
            }
        }


        private async void OnValidSubmit()
        {
            try
            {
                if (!ValidateData()) return;
                if (isNew)
                {
                    await savingsAPI.InsertBudgetPlanRule(Model);
                }
                else
                {
                    await savingsAPI.EditBudgetPlanRule(Model.Id, Model);
                }
                this.dialogService.Close(true);
            }
            catch(Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Attention", ex.Message);
            }

        }

    }
}
