using Microsoft.AspNetCore.Components;
using Savings.Model;
using MoneyPlan.SPA.Services;

namespace MoneyPlan.SPA.Pages
{
    public partial class ConfigurationEdit : ComponentBase
    {

        [Inject]
        private ISavingsApi savingsAPI { get; set; }

        private Configuration Configuration { get; set; }

        public MoneyCategory[] Categories { get; set; }

        public DateTime LastMaterializedDate { get; set; }

        public decimal LastMaterializedAmount { get; set; }

        bool ValidateData()
        {

            return true;
        }

        protected override async Task OnInitializedAsync()
        {
            var lastMaterializedItem = await savingsAPI.GetLastMaterializedMoneyItemPeriod();
            LastMaterializedDate = lastMaterializedItem?.Date ?? new DateTime(2000, 1, 1);
            LastMaterializedAmount = lastMaterializedItem?.Projection ?? 0;
            Categories = await savingsAPI.GetMoneyCategories();
            Configuration = (await savingsAPI.GetConfigurations()).First();
        }

        private async void OnValidSubmit()
        {
            try
            {
                if (!ValidateData()) return;
                // TODO: Valutare cosa fare con questa possibile mancata configurazione.
                try
                {
                    await savingsAPI.EditLastMaterializedMoneyItemPeriod(LastMaterializedDate, LastMaterializedAmount);
                } catch
                { 
                }                
                await savingsAPI.PutConfiguration(Configuration.ID, Configuration);
            }
            catch
            {
                throw;
            }

        }

    }

}
