using Microsoft.Extensions.Logging;
using Savings.DAO.Infrastructure;
using Savings.Model;

namespace MoneyPlan.Builder
{
    internal class MoneyCategoryBuilder : IMoneyCategoryBuilder
    {
        private readonly MoneyCategory _entity = new MoneyCategory();
        private readonly SavingsContext _context;
        private readonly ILogger _logger;

        public MoneyCategoryBuilder(SavingsContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public IMoneyCategoryBuilder WithId(long id)
        {
            _entity.ID = id;
            return this;
        }

        public IMoneyCategoryBuilder WithDescription(string description)
        {
            _entity.Description = description;
            return this;
        }

        public IMoneyCategoryBuilder WithIcon(string icon)
        {
            _entity.Icon = icon;
            return this;
        }

        public IMoneyCategoryBuilder WithParent(long parentId)
        {
            _entity.ParentId = parentId;
            return this;
        }

        public IMoneyCategoryBuilder WithChildren(IEnumerable<MoneyCategory> children)
        {
            foreach (var child in children)
                _entity.Children.Add(child);
            return this;
        }

        public MoneyCategory Build()
        {
            _context.MoneyCategories.Add(_entity);
            _context.SaveChanges();
            return _entity;
        }
    }
}
