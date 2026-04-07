using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Savings.DAO.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPlan.Builder
{
    public class DaoBuilder
    {
        private readonly SavingsContext _context;
        private readonly ILogger _logger;

        public DaoBuilder(SavingsContext context)
            : this(context, NullLogger.Instance)
        {
        }

        public DaoBuilder(SavingsContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public IMoneyCategoryBuilder MoneyCategories
            => new MoneyCategoryBuilder(_context, _logger);

        public IBudgetPlanRuleBuilder BudgetPlanRules
            => new BudgetPlanRuleBuilder(_context, _logger);

        public IBudgetPlanBuilder BudgetPlans
            => new BudgetPlanBuilder(_context, _logger);
    }
}
