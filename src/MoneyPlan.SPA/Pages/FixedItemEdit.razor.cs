using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using Savings.Model;
using MoneyPlan.SPA.Services;
using Radzen.Blazor;

namespace MoneyPlan.SPA.Pages
{
    public partial class FixedItemEdit : ComponentBase
    {

        [Inject]
        public ISavingsApi savingsAPI { get; set; }

        [Inject]
        DialogService dialogService { get; set; }

        [Inject]
        public NotificationService notificationService { get; set; }

        [Parameter]
        public FixedMoneyItem fixedItemToEdit { get; set; }

        [Parameter]
        public bool isNew { get; set; }

        [Parameter]
        public int AccountID { get; set; }

        public bool Incoming { get; set; }

        public MoneyCategory[] Categories { get; set; }
        public MoneyAccount[] Accounts { get; private set; }

        InputNumber<decimal?> amountInputNumber;

        RadzenDropDown<int> accountSelector;

        protected override void OnInitialized()
        {
            if (isNew)
            {
                this.fixedItemToEdit.Date = DateTime.UtcNow.Date;
                this.fixedItemToEdit.Amount = null;
                this.fixedItemToEdit.AccountID = AccountID;
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
                
                await Task.Delay(500);
                await amountInputNumber.Element.Value.FocusAsync();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            Categories = await savingsAPI.GetMoneyCategories();
            Accounts = await savingsAPI.GetMoneyAccounts();
            Incoming = fixedItemToEdit.Amount > 0;
        }

        bool ValidateData()
        {
            if (fixedItemToEdit.AccountID == default)
            {
                notificationService.Notify(NotificationSeverity.Error, "Attention", "The Account must be selected");
                return false;
            }
            if (fixedItemToEdit.Amount == null || fixedItemToEdit.Amount == 0)
            {
                notificationService.Notify(NotificationSeverity.Error, "Attention", "The amount must contain a value and be different than 0");
                return false;
            }
            if (fixedItemToEdit.CategoryID == null)
            {
                notificationService.Notify(NotificationSeverity.Error, "Attention", "Category is mandatory field");
                return false;
            }
            return true;
        }

        async Task Delete()
        {
            var res = await dialogService.Confirm("Are you sure you want delete?", "Delete fixed item", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });
            if (res.HasValue && res.Value)
            {
                var deletedItem = await savingsAPI.DeleteFixedMoneyItem(fixedItemToEdit.ID);
                this.dialogService.Close(true);
            }
        }


        private async void OnValidSubmit()
        {
            try
            {
                if (!ValidateData()) return;
                if (Incoming)
                {
                    fixedItemToEdit.Amount = Math.Abs(fixedItemToEdit.Amount.Value);
                }
                else
                {
                    fixedItemToEdit.Amount = -Math.Abs(fixedItemToEdit.Amount.Value);
                }
                if (isNew)
                {
                    await savingsAPI.InsertFixedMoneyItem(fixedItemToEdit);
                }
                else
                {
                    await savingsAPI.EditFixedMoneyItem(fixedItemToEdit.ID, fixedItemToEdit);
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
