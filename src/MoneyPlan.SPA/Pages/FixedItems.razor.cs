﻿using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor.Rendering;
using Savings.Model;
using MoneyPlan.SPA.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MoneyPlan.SPA.Pages
{
    public partial class FixedItems : ComponentBase
    {
        [Inject]
        public Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }

        [Inject]
        public ISavingsApi savingsAPI { get; set; }

        [Inject]
        public DialogService dialogService { get; set; }

        public Configuration CurrentConfiguration { get; set; }

        private FixedMoneyItem[] fixedMoneyItems;

        public DateTime? FilterDateFrom { get; set; }

        public DateTime? FilterDateTo { get; set; }

        [Parameter]
        public long? FilterCategory { get; set; }

        public string? FilterNotes { get; set; }

        /// <summary>
        /// Tells to filter for items that need to be fixed because they're invalid.
        /// </summary>
        public bool FilterInvalid { get; set; }

        public MoneyCategory[] Categories { get; set; }

        protected override async Task OnInitializedAsync()
        {
            FilterDateFrom = await localStorage.GetItemAsync<DateTime?>("FixedItems.FilterDateFrom") ?? DateTime.Now.Date.AddMonths(-2);
            FilterDateTo = await localStorage.GetItemAsync<DateTime?>("FixedItems.FilterDateTo") ?? DateTime.Now.Date.AddDays(15);
            Categories = await savingsAPI.GetMoneyCategories();
            CurrentConfiguration = (await savingsAPI.GetConfigurations()).FirstOrDefault();
            await InitializeList();
        }

        async void Change(DateTime? value, string name)
        {
            await InitializeList();
            StateHasChanged();
        }

        async void OnFromDateChanged(DateTime dateTime)
        {
            FilterDateFrom = new DateTime(dateTime.Year, dateTime.Month, 1);
            await localStorage.SetItemAsync("FixedItems.FilterDateFrom", FilterDateFrom);
            await InitializeList();
            StateHasChanged();
        }

        async void OnToDateChanged(DateTime dateTime)
        {
            FilterDateTo = dateTime.EndOfMonth();
            await localStorage.SetItemAsync("FixedItems.FilterDateTo", FilterDateTo);
            await InitializeList();
            StateHasChanged();
        }

        async Task FilterCategoryChanged(long? value)
        {
            FilterCategory = value;

            await InitializeList();
            StateHasChanged();
        }

        async Task FilterNotesChanged(string value)
        {
            FilterNotes = value;

            await InitializeList();
            StateHasChanged();
        }

        async Task ToogleInvalid(bool toogle)
        {
            FilterInvalid = toogle;

            await InitializeList();
            StateHasChanged();
        }


        async Task InitializeList()
        {
            var results = await savingsAPI.GetFixedMoneyItems(FilterDateFrom, FilterDateTo, false, FilterCategory);

            if (FilterInvalid)
            {
                results = FilterByInvalid(results).ToArray();
            }
            if (!string.IsNullOrEmpty(FilterNotes))
            {
                results = FilterByNotes(results).ToArray();
            }

            fixedMoneyItems = results;
        }


        async Task AddNew()
        {
            bool? res = await dialogService.OpenAsync<FixedItemEdit>($"Add new",
                         new Dictionary<string, object>() { { "fixedItemToEdit", new Savings.Model.FixedMoneyItem() }, { "isNew", true } },
                         new DialogOptions() { Width = "600px", Draggable = true });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }


        async Task Edit(FixedMoneyItem item)
        {
            bool? res = await dialogService.OpenAsync<FixedItemEdit>($"Edit item",
                             new Dictionary<string, object>() { { "fixedItemToEdit", item }, { "isNew", false } },
                             new DialogOptions() { Width = "600px" });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }


        private Func<IEnumerable<FixedMoneyItem>, IEnumerable<FixedMoneyItem>> FilterByInvalid = (list) => list.Where(item => !item.CategoryID.HasValue);

        private IEnumerable<FixedMoneyItem> FilterByNotes(IEnumerable<FixedMoneyItem> list)
        {
            return list.Where(item => item.Note.Contains(FilterNotes, StringComparison.OrdinalIgnoreCase));
        }

    }
}