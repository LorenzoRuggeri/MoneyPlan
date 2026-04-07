using Microsoft.Extensions.Logging;
using MoneyPlan.Model;
using Savings.DAO.Infrastructure;
using Savings.Model;

namespace MoneyPlan.Builder
{
    internal class BudgetPlanRuleBuilder : IBudgetPlanRuleBuilder
    {
        private readonly BudgetPlanRule _entity = new BudgetPlanRule();
        private readonly SavingsContext _context;
        private readonly ILogger _logger;

        public BudgetPlanRuleBuilder(SavingsContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public IBudgetPlanRuleBuilder WithId(int id)
        {
            _entity.Id = id;
            return this;
        }

        // TODO: [Claude] Preferisco sempre passare gli Id ai Builder, di modo da non dovermi legare ad avere l'oggetto in mano.
        public IBudgetPlanRuleBuilder WithCategory(long categoryId)
        {
            _entity.CategoryId = categoryId;
            return this;
        }

        public IBudgetPlanRuleBuilder WithCategoryFilter(StringFilterType filter, string text)
        {
            _entity.CategoryFilter = filter;
            _entity.CategoryText = text;
            return this;
        }

        public IBudgetPlanRuleBuilder WithType(BudgetPlanType type)
        {
            _entity.Type = type;
            return this;
        }

        public IBudgetPlanRuleBuilder AsIncome()
        {
            _entity.Income = true;
            _entity.Type = null;
            return this;
        }

        public IBudgetPlanRuleBuilder WithBudgetPlan(int id)
        {
            _entity.BudgetPlanId = id;
            return this;
        }

        public BudgetPlanRule Build()
        {
            _context.BudgetPlanRules.Add(_entity);
            _context.SaveChanges();
            return _entity;
        }

    }
}
