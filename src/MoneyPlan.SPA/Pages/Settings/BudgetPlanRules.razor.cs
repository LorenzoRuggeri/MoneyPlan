using Microsoft.AspNetCore.Components;
using MoneyPlan.SPA.Services;
using Radzen.Blazor.Rendering;
using Radzen;
using MoneyPlan.Business;
using MoneyPlan.Model;
using Savings.Model;
using Microsoft.AspNetCore.Components.Web;
using Radzen.Blazor;

namespace MoneyPlan.SPA.Pages.Settings
{
    public partial class BudgetPlanRules : ComponentBase
    {
        [Inject]
        public Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }

        [Inject]
        public DialogService dialogService { get; set; }

        [Parameter]
        public IEnumerable<MoneyCategory> Data { get; set; } = Enumerable.Empty<MoneyCategory>();

        public IEnumerable<RuleViewModel> Rules { get; set; } = [];

        public int? FilterBudgetPlan { get; set; } = null;

        public BudgetPlan? SelectedBudgetPlan { get; set; } = null;

        public long? SelectedCategory { get; set; } = null;

        public IEnumerable<BudgetPlan> BudgetPlans { get; set; }

        protected override async Task OnInitializedAsync()
        {
            BudgetPlans = await APIClient.GetBudgetPlans();
            await InitializeList();
        }

        async void OnBudgetPlanChanged(int budgetPlanId)
        {
            SelectedBudgetPlan = BudgetPlans.FirstOrDefault(x => x.Id == budgetPlanId);
            await InitializeList();
            StateHasChanged();
        }

        async Task InitializeList()
        {
            IEnumerable<MoneyCategory> moneyCategories = (await APIClient.GetMoneyCategories());

            /*
             * TODO: To enable if I'd want to load at startup. 
            IEnumerable<BudgetPlanRule> appliedRules = Enumerable.Empty<BudgetPlanRule>();

            if (FilterBudgetPlan != null)
            {
                var responseRules = await APIClient.GetBudgetPlanRules(FilterBudgetPlan.Value);
                if (responseRules.IsSuccessStatusCode)
                {
                    appliedRules = responseRules.Content;
                }
            }
            */

            Data = moneyCategories.Where(x => x.ParentId == null).ToList();

            await LoadRules(SelectedCategory);
        }

        async Task LoadRules(long? categoryId)
        {
            if (categoryId == null)
                return;

            var response = await APIClient.GetRulesForCategory(FilterBudgetPlan.Value, (int)categoryId.Value);
            IEnumerable<BudgetPlanRule> appliedRules = [];
            if (response.IsSuccessStatusCode)
            {
                appliedRules = response.Content ?? [];
            }
            Rules = appliedRules.Select(x => new RuleViewModel()
            {
                Id = x.Id,
                Checked = true,
                Description = x.Category.Description,
                Filter = x.CategoryFilter == StringFilterType.None ?
                    "No text filter"
                    : x.CategoryFilter.GetDisplayDescription() + " '" + x.CategoryText + "'",
                Type = x.Type ?? BudgetPlanType.None,
            }).ToList();
        }

        void OnTreeItemExpand(TreeExpandEventArgs args)
        {
            var category = args.Value as MoneyCategory;
            if (category == null)
            {
                return;
            }

            args.Children.Data = category.Children;
            args.Children.TextProperty = "Description";
            args.Children.HasChildren = (category) => (category as MoneyCategory).Children.Any();
        }

        async void OnTreeItemClick(TreeEventArgs args)
        {
            var category = (args.Value as MoneyCategory);

            if (FilterBudgetPlan.HasValue == false)
            {
                await dialogService.Alert("You need to select a Budget Plan otherwise the Rules will be empty.");
                return;
            }

            SelectedCategory = category.ID;

            await LoadRules(SelectedCategory);

            StateHasChanged();
        }

        async Task AddRule()
        {
            bool? res = await dialogService.OpenAsync<BudgetPlanRuleEdit>($"Add new",
                new Dictionary<string, object>() { { "BudgetPlanId", FilterBudgetPlan.Value } },
                new DialogOptions() { Width = "600px", Draggable = true });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }

        async Task OnGridRowClick(DataGridRowMouseEventArgs<RuleViewModel> args)
        {
            var item = args.Data;

            bool? res = await dialogService.OpenAsync<BudgetPlanRuleEdit>($"Edit",
                 new Dictionary<string, object>() { { "Id", item.Id } },
                 new DialogOptions() { Width = "600px", Draggable = true });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }

        void OnGridRowDatabind(RowRenderEventArgs<RuleViewModel> args)
        {
            var item = args.Data;

            args.Attributes.Add("class", "item-action");
        }

        async Task SaveBudgetPlan()
        {
            if (SelectedBudgetPlan == null)
            {
                await dialogService.Alert("No Budget Plan selected");
                return;
            }
            /*
            var filteredRules = Data.Where(x => x.Checked).SelectMany(x => x.Rules)
                .Select(x => new BudgetPlanBudgetRules()
            {
                BudgetPlanRuleId = x.Id,
                BudgetPlanId = FilterBudgetPlan.Value
            }).ToList();
            SelectedBudgetPlan.Rules.Clear();
            SelectedBudgetPlan.Rules.AddRange(filteredRules);

            try
            {
                await APIClient.UpdateBudgetPlan(FilterBudgetPlan.Value, SelectedBudgetPlan);
            }
            catch (Exception ex)
            {
                await dialogService.Alert("Error while updating the Budget Plan");
            }
            */
        }


        public class RuleViewModel
        {
            public int Id { get; set; }

            public bool Checked { get; set; }

            public string Description { get; set; }

            public string Filter { get; set; }

            public BudgetPlanType Type { get; set; }
        }
    }
}
