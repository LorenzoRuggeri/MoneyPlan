using Microsoft.Extensions.Logging;
using MoneyPlan.Model;
using Savings.DAO.Infrastructure;

namespace MoneyPlan.Builder
{
    internal class BudgetPlanBuilder : IBudgetPlanBuilder
    {
        private readonly BudgetPlan _entity = new BudgetPlan();
        private readonly SavingsContext _context;
        private readonly ILogger _logger;

        public BudgetPlanBuilder(SavingsContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public IBudgetPlanBuilder WithId(int id)
        {
            _entity.Id = id;
            return this;
        }

        public IBudgetPlanBuilder WithName(string name)
        {
            _entity.Name = name;
            return this;
        }

        public IBudgetPlanBuilder WithPercentages(int needs, int wants, int savings)
        {
            if (needs + wants + savings != 100)
                throw new InvalidOperationException(
                    $"Percentages must sum to 100, got {needs + wants + savings}.");
            _entity.NeedsPercentage = needs;
            _entity.WantsPercentage = wants;
            _entity.SavingsPercentage = savings;
            return this;
        }

        public IBudgetPlanBuilder WithRule(int ruleId)
        {
            _entity.Rules.Add(new BudgetPlanRule
            {
                Id = ruleId
            });
            return this;
        }

        public IBudgetPlanBuilder WithRules(IEnumerable<int> ruleIds)
        {
            foreach (var ruleId in ruleIds)
                WithRule(ruleId);
            return this;
        }

        public BudgetPlan Build()
        {
            /*
            foreach (var join in _entity.Rules)
            {
                join.BudgetPlanId = _entity.Id;
            }
            */
            _context.BudgetPlans.Add(_entity);
            _context.SaveChanges();
            return _entity;
        }
    }
}
