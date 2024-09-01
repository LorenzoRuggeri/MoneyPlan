using Microsoft.AspNetCore.Components;
using Radzen;
using Savings.Model;
using MoneyPlan.SPA.Services;

namespace MoneyPlan.SPA.Pages
{
    public partial class RecurrentItems : ComponentBase
    {

        [Inject]
        public ISavingsApi savingsAPI { get; set; }

        [Inject]
        public DialogService dialogService { get; set; }

        [Inject]
        public NotificationService notificationService { get; set; }

        [Inject]
        public Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }


        private RecurrentMoneyItem[] recurrentMoneyItems;


        [Parameter]
        public RecurrentMoneyItem parentItem { get; set; } = null;

        public bool ShowOnlyActive { get; set; } = true;
        public DateTime? FilterOnlyActiveDateFrom { get; set; }
        public DateTime? FilterOnlyActiveDateTo { get; set; }
        public int? FilterAccount { get; set; } = null;

        public IEnumerable<MoneyAccount> Accounts { get; set; }

        async Task ShowOnlyActiveOnChange()
        {
            await InitializeList();
            StateHasChanged();
        }

        async void FilterDateOnlyActiveChange(DateTime? value, string name)
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

        protected override async Task OnInitializedAsync()
        {
            Accounts = await savingsAPI.GetMoneyAccounts();
            FilterAccount = await localStorage.GetItemAsync<int?>("Search.AccountId") ?? null;
            await InitializeList();
        }

        async Task InitializeList()
        {
            recurrentMoneyItems = await savingsAPI.GetRecurrentMoneyItems(FilterAccount, ShowOnlyActive, FilterOnlyActiveDateFrom, FilterOnlyActiveDateTo);
        }


        async Task AddNew()
        {
            bool? res = await dialogService.OpenAsync<RecurrentItemEdit>($"Add new",
                        new Dictionary<string, object>()
                        { 
                            { "recurrentItemToEdit", new Savings.Model.RecurrentMoneyItem() },
                            { "isNew", true }
                        },
                        new DialogOptions() { Width = "600px" });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }

        async Task Edit(RecurrentMoneyItem item)
        {
            bool? res = await dialogService.OpenAsync<RecurrentItemEdit>($"Edit item",
                             new Dictionary<string, object>()
                             { 
                                 { "recurrentItemToEdit", item }, 
                                 { "isNew", false }
                             },
                             new DialogOptions() { Width = "600px" });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }

    }
}
