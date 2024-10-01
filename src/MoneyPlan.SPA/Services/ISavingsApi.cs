using MoneyPlan.Model.API;
using MoneyPlan.Model.API.Report;
using Refit;
using Savings.Model;
using System;
using System.Threading.Tasks;

namespace MoneyPlan.SPA.Services
{
    public interface ISavingsApi
    {
        [Get("/api/Savings")]
        Task<MaterializedMoneyItem[]> GetSavings(int? accountId, DateTime? from, DateTime? to, bool onlyInstallment = false);

        [Get("/api/FixedMoneyItems")]
        Task<FixedMoneyItem[]> GetFixedMoneyItems(DateTime? from, DateTime? to, bool excludeWithdrawal, long? filterCategory);

        [Delete("/api/FixedMoneyItems/{id}")]
        Task<FixedMoneyItem> DeleteFixedMoneyItem(long id);

        [Get("/api/FixedMoneyItems/{id}")]
        Task<FixedMoneyItem> GetixedMoneyItem(long id);

        [Post("/api/FixedMoneyItems")]
        Task<FixedMoneyItem> InsertFixedMoneyItem(FixedMoneyItem fixedMoneyItem);

        [Put("/api/FixedMoneyItems/{id}")]
        Task EditFixedMoneyItem(long id, FixedMoneyItem fixedMoneyItem);

        [Get("/api/RecurrentMoneyItems")]
        Task<RecurrentMoneyItem[]> GetRecurrentMoneyItems(int? accountId, bool onlyActive, DateTime? endDateFrom, DateTime? endDateTo);

        [Delete("/api/RecurrentMoneyItems/{id}")]
        Task<RecurrentMoneyItem> DeleteRecurrentMoneyItem(long id);

        [Post("/api/RecurrentMoneyItems")]
        Task<RecurrentMoneyItem> InsertRecurrentMoneyItem(RecurrentMoneyItem fixedMoneyItem);

        [Put("/api/RecurrentMoneyItems/{id}")]
        Task EditRecurrentMoneyItem(long id, RecurrentMoneyItem fixedMoneyItem);

        [Get("/api/Savings/Backup")]
        Task<Stream> GetBackup();

        [Post("/api/Savings/ToHistory")]
        Task PostSavingsToHistory(DateTime date);

        [Patch("/api/MaterializedMoneyItems/LastMaterializedMoneyItemPeriod")]
        Task EditLastMaterializedMoneyItemPeriod(DateTime date,decimal amount);

        [Get("/api/MaterializedMoneyItems/LastMaterializedMoneyItemPeriod")]
        Task<MaterializedMoneyItem> GetLastMaterializedMoneyItemPeriod();

        [Get("/api/MaterializedMoneyItems")]
        Task<MaterializedMoneyItem[]> GetMaterializedMoneyItems(DateTime? from, DateTime? to, bool onlyRecurrent);

        [Delete("/api/MaterializedMoneyItems/ToHistory/{id}")]
        Task DeleteMaterializedMoneyItemToHistory(long id);

        [Get("/api/MoneyCategories")]
        Task<MoneyCategory[]> GetMoneyCategories();

        [Get("/api/Configurations")]
        Task<Configuration[]> GetConfigurations();

        [Put("/api/Configurations/{id}")]
        Task PutConfiguration(long id, Configuration configuration);

        [Get("/api/Report/GetCategoryResume")]
        Task<ReportCategory[]> GetCategoryResume(int? accountId, string periodPattern, DateTime dateFrom, DateTime dateTo);

        [Get("/api/Report/GetCategoryResumeDetail")]
        Task<ReportDetail[]> GetCategoryResumeDetail(int? accountId, string periodPattern, DateTime dateFrom, DateTime dateTo, long? category, string period);

        [Get("/api/Report/GetBudgetPlanResume")]
        Task<ReportBudgetPlan[]> GetBudgetPlanResume(int? accountId, string periodPattern, DateTime dateFrom, DateTime dateTo);

        [Post("/api/Import/ImportFromFile")]
        Task ImportFromfile(ImportFileRequest request);

        [Get("/api/MoneyAccounts")]
        Task<MoneyAccount[]> GetMoneyAccounts();

        [Get("/api/Import/Importers")]
        Task<IEnumerable<string>> GetImporters();

        [Get("/api/BudgetPlan/")]
        Task<IEnumerable<BudgetPlan>> GetBudgetPlans();

        [Put("/api/BudgetPlan/")]
        Task UpdateBudgetPlan(int id, BudgetPlan model);

        [Get("/api/BudgetPlan/Rules")]
        Task<IEnumerable<BudgetPlanRule>> GetAnagraphicRules();

        [Get("/api/BudgetPlan/{budgetPlanId}/Rules")]
        Task<IEnumerable<BudgetPlanRule>> GetBudgetPlanRules(int budgetPlanId);

        [Post("/api/BudgetPlan/Rules")]
        Task<int> InsertBudgetPlanRule(BudgetPlanRule model);

        [Put("/api/BudgetPlan/Rules/{id}")]
        Task EditBudgetPlanRule(int id, BudgetPlanRule model);

        [Delete("/api/BudgetPlan/Rules")]
        Task DeleteBudgetPlanRule(int id);
    }
}
