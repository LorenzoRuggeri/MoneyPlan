using Microsoft.AspNetCore.Components;
using Savings.Model;
using MoneyPlan.Model.API;
using MoneyPlan.SPA.Services;
using Radzen.Blazor;
using Radzen;
using System;
using Microsoft.AspNetCore.Components.Forms;

namespace MoneyPlan.SPA.Pages
{
    public partial class Import : ComponentBase
    {

        [Inject]
        private ISavingsApi savingsAPI { get; set; }

        [Inject]
        public DialogService dialogService { get; set; }

        private IBrowserFile fileUpload;

        public int? FilterAccount { get; set; } = null;

        public string FilterImporter { get; set; } = null;

        public IEnumerable<string> Importers { get; set; }

        public IEnumerable<MoneyAccount> Accounts { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Importers = await savingsAPI.GetImporters();
            Accounts = await savingsAPI.GetMoneyAccounts();
        }

        void LoadFiles(InputFileChangeEventArgs args)
        {
            fileUpload = args.File;
        }

        bool ValidateData()
        {
            return true;
        }

        async void OnSubmit()
        {
            try
            {
                if (!ValidateData()) return;

                var stream = fileUpload.OpenReadStream();
                byte[] b = new byte[stream.Length];
                await stream.ReadAsync(b, 0, (int)stream.Length);

                var request = new ImportFileRequest()
                {
                    Account = FilterAccount.Value,
                    Importer = FilterImporter,
                    Content = b 
                };

                try
                {
                    await savingsAPI.ImportFromfile(request);
                    await dialogService.Alert("Import has been successful.", "Import Result");
                }
                catch (Refit.ApiException ex)
                {
                    await dialogService.Alert(ex.Message, ex.ReasonPhrase);
                }
            }
            catch
            {
                throw;
            }

        }

    }

}
