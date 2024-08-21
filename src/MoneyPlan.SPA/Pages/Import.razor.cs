using Microsoft.AspNetCore.Components;
using Savings.Model;
using MoneyPlan.SPA.Services;

namespace MoneyPlan.SPA.Pages
{
    public partial class Import : ComponentBase
    {

        [Inject]
        private ISavingsApi savingsAPI { get; set; }

        public Configuration Configuration { get; set; } = new Configuration();

        bool ValidateData()
        {
            return true;
        }

        protected override async Task OnInitializedAsync()
        {
        }

        private async void OnValidSubmit()
        {
            try
            {
                if (!ValidateData()) return;
                await savingsAPI.ImportFromfile();
            }
            catch
            {
                throw;
            }

        }

    }

}
