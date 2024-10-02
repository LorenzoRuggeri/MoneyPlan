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
    public partial class Dashboard : ComponentBase
    {
        [Inject]
        public ISavingsApi savingsAPI { get; set; }

        [Inject]
        public Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }

        [Inject]
        public DialogService dialogService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitializeList();
        }

        async Task InitializeList()
        {
            await Task.CompletedTask;
        }
    }
}
