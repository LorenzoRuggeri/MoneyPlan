using Microsoft.AspNetCore.Components;
using Savings.Model;
using MoneyPlan.SPA.Services;
using System;
using System.Threading.Tasks;
using Radzen;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace MoneyPlan.SPA.Pages
{
    public partial class Projection : ComponentBase
    {
        [Inject]
        public ISavingsApi savingsAPI { get; set; }

        [Inject]
        public Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }


        private MaterializedMoneyItem[] materializedMoneyItems;

        [Inject]
        public DialogService dialogService { get; set; }

        public bool ShowPastItems { get; set; } = false;

        public DateTime? FilterDateTo { get; set; }

        public int? FilterAccount { get; set; } = null;

        public Configuration CurrentConfiguration { get; set; }

        public IEnumerable<MoneyAccount> Accounts { get; set; }

        protected override async Task OnInitializedAsync()
        {
            FilterDateTo = DateTime.Now.Date.AddMonths(6);
            FilterAccount = await localStorage.GetItemAsync<int?>("Search.AccountId") ?? null;
            await InitializeList();
            CurrentConfiguration = (await savingsAPI.GetConfigurations()).FirstOrDefault();
            Accounts = await savingsAPI.GetMoneyAccounts();
        }


        async void PastItems_Changed()
        {
            await InitializeList();
            StateHasChanged();
        }

        async void OnAccountChanged(object accountId)
        {
            await localStorage.SetItemAsync("Search.AccountId", FilterAccount);
            await InitializeList();
            StateHasChanged();
        }

        async void Change(DateTime? value, string name)
        {
            await InitializeList();
            StateHasChanged();
        }

        async Task InitializeList()
        {
            materializedMoneyItems = await savingsAPI.GetSavings(FilterAccount, null, FilterDateTo);

            if (!ShowPastItems)
            {
                var lastBeforeToday = materializedMoneyItems.LastOrDefault(x => x.Date <= DateTime.Now.Date);
                if (lastBeforeToday != null)
                {
                    materializedMoneyItems = materializedMoneyItems[Array.IndexOf(materializedMoneyItems, lastBeforeToday)..];
                }
            }
        }

        async Task AdjustRecurrency(MaterializedMoneyItem item)
        {
            if (item.EndPeriod) return;
            var res = await dialogService.OpenAsync<RecurrencyAdjustment>($"Recurrency Adjustment",
                            new Dictionary<string, object>() { { "materializedItem", item } },
                            new DialogOptions() { Width = "600px", Height = "300px" });
            await InitializeList();
            StateHasChanged();
        }

        async Task AdjustFixedItem(MaterializedMoneyItem item)
        {
            if (item.EndPeriod) return;
            if (!item.FixedMoneyItemID.HasValue) return;

            // We want the original item, not the one that has been projected.
            var itemToEdit = await savingsAPI.GetixedMoneyItem(item.FixedMoneyItemID.Value);

            bool? res = await dialogService.OpenAsync<FixedItemEdit>($"Edit item",
                             new Dictionary<string, object>() { { "fixedItemToEdit", itemToEdit }, { "isNew", false } },
                             new DialogOptions() { Width = "600px" });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }

        async Task SaveMaterializedHistory(DateTime date)
        {
            string dialogMessage = string.Empty;
            if (FilterAccount.HasValue)
            {
                dialogMessage += "Projection will be saved for all accounts. You're viewing only a partial projecton." + "<br>";
            }
            dialogMessage += $"Do you want to save the projection to the history until {date:dd/MM/yyyy}?";
            
            var res = await dialogService.Confirm(dialogMessage, 
                "Save the history", 
                new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });
            if (res.HasValue && res.Value)
            {
                await savingsAPI.PostSavingsToHistory(date);
                await InitializeList();
            }
        }

        async Task AddNew()
        {
            bool? res = await dialogService.OpenAsync<FixedItemEdit>($"Add new",
                         new Dictionary<string, object>() { { "fixedItemToEdit", new Savings.Model.FixedMoneyItem() }, { "isNew", true } },
                         new DialogOptions() { Width = "700px" });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }
    }
}
