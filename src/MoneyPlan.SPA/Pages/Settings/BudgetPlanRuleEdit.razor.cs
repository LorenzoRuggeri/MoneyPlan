using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using Savings.Model;
using MoneyPlan.SPA.Services;
using Radzen.Blazor;
using MoneyPlan.Model;

namespace MoneyPlan.SPA.Pages.Settings
{
    public partial class BudgetPlanRuleEdit : ComponentBase
    {
        [Inject]
        DialogService dialogService { get; set; }

        [Inject]
        public NotificationService notificationService { get; set; }

        [Parameter]
        public int Id { get; set; }

        [Parameter]
        public int BudgetPlanId { get; set; }

        public BudgetPlanRule Model { get; set; }

        public MoneyCategory[] Categories { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Categories = await APIClient.GetMoneyCategories();
            if (Id == default)
            {
                this.Model = new();
                this.Model.Type = BudgetPlanType.Needs;
            }
            else
            {
                var response = await APIClient.GetBudgetPlanRule(Id);
                if (response.IsSuccessStatusCode)
                {
                    Model = response.Content;
                }
            }
        }

        bool ValidateData()
        {
            if (Model.CategoryId == default)
            {
                notificationService.Notify(NotificationSeverity.Error, "Attention", "Category is mandatory field");
                return false;
            }
            // If we're being add a new item, we must ensure a BudgetPlan is chosen.
            if (Model.Id == default)
            {
                if (BudgetPlanId == default)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Attention", "A new budget rule must be associated to a budget plan");
                    return false;
                }
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
                    await APIClient.DeleteBudgetPlanRule(Model.Id);
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
                if (Model.Id == default)
                {
                    Model.BudgetPlanId = this.BudgetPlanId;
                    await APIClient.InsertBudgetPlanRule(Model);
                }
                else
                {
                    await APIClient.EditBudgetPlanRule(Model.Id, Model);
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
