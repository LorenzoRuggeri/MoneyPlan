using Microsoft.AspNetCore.Components;
using MoneyPlan.SPA.Services;
using Radzen.Blazor.Rendering;
using Radzen;
using Savings.Model;
using MoneyPlan.Business;

namespace MoneyPlan.SPA.Pages.Settings
{
    public partial class BudgetPlanRules : ComponentBase
    {
        [Inject]
        public Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }

        [Inject]
        public ISavingsApi savingsAPI { get; set; }

        [Inject]
        public DialogService dialogService { get; set; }

        [Parameter]
        public IEnumerable<ItemModel> Data { get; set; } = Enumerable.Empty<ItemModel>();

        public int? FilterBudgetPlan { get; set; } = null;

        public BudgetPlan? SelectedBudgetPlan { get; set; } = null;

        public IEnumerable<BudgetPlan> BudgetPlans { get; set; }

        protected override async Task OnInitializedAsync()
        {
            BudgetPlans = await savingsAPI.GetBudgetPlans();
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
            IEnumerable<ItemModel> anagraphic = (await savingsAPI.GetAnagraphicRules())
                .Select(x => new ItemModel(x));

            IEnumerable<ItemModel> appliedRules = Enumerable.Empty<ItemModel>();

            if (FilterBudgetPlan != null)
            {
                appliedRules = (await savingsAPI.GetBudgetPlanRules(FilterBudgetPlan.Value))
                    .Select(x =>
                    {
                        var item = new ItemModel(x);
                        item.Checked = true;
                        return item;
                    });
            }

            Data = appliedRules.UnionBy(anagraphic, (model) => model.Id)
                .OrderBy(x => x.Category?.Parent?.Description)
                .ThenBy(x => x.Category?.Description)
                .ToList();
        }

        async Task AddNew()
        {
            bool? res = await dialogService.OpenAsync<BudgetPlanRuleEdit>($"Add new",
                         new Dictionary<string, object>() {
                             { "Model", new Savings.Model.BudgetPlanRule() },
                             { "isNew", true }
                         },
                         new DialogOptions() { Width = "600px", Draggable = true });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }

        async Task Edit(BudgetPlanRule item)
        {
            bool? res = await dialogService.OpenAsync<BudgetPlanRuleEdit>($"Edit",
                         new Dictionary<string, object>() {
                             { "Model", item.Clone() },
                             { "isNew", false }
                         },
                         new DialogOptions() { Width = "600px", Draggable = true });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }

        async Task SaveBudgetPlan()
        {
            if (SelectedBudgetPlan == null)
            {
                await dialogService.Alert("No Budget Plan selected");
                return;
            }

            var filteredRules = Data.Where(x => x.Checked).Select(x => new BudgetPlanBudgetRules()
            {
                BudgetPlanRuleId = x.Id,
                BudgetPlanId = FilterBudgetPlan.Value
            }).ToList();
            SelectedBudgetPlan.Rules.Clear();
            SelectedBudgetPlan.Rules.AddRange(filteredRules);

            try
            {
                await savingsAPI.UpdateBudgetPlan(FilterBudgetPlan.Value, SelectedBudgetPlan);
            }
            catch (Exception ex)
            {
                await dialogService.Alert("Error while updating the Budget Plan");
            }
        }


        public class ItemModel : BudgetPlanRule
        {
            public bool Checked { get; set; }

            public ItemModel(BudgetPlanRule decorated)
            {
                this.Type = decorated.Type;
                this.Category = decorated.Category;
                this.CategoryFilter = decorated.CategoryFilter;
                this.CategoryId = decorated.CategoryId;
                this.CategoryText = decorated.CategoryText;
                this.Id = decorated.Id;
                this.Income = decorated.Income;
            }
        }
    }
}
