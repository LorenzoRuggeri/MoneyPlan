using Savings.API.Extensions;
using Savings.DAO.Infrastructure;
using Savings.Model;

namespace Savings.API.Services
{
    /// <summary>
    /// Interact with MoneyCategory
    /// </summary>
    public class CategoriesService
    {
        private readonly SavingsContext context;

        public CategoriesService(SavingsContext context)
        {
            this.context = context;
        }

        public IEnumerable<GroupCategory> GetGroupCategories()
        {
            return this.GetStructuredCategories();
        }

        /// <summary>
        /// Search for a Category with specified <paramref name="categoryId"/> and retrieve its <see cref="GroupCategory"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GroupCategory GetParentCategory(long categoryId)
        {
            var categories = GetStructuredCategories();
            return categories.Where(y => y.ID == categoryId || y.Related.Any(child => child.ID == categoryId)).FirstOrDefault() ?? new GroupCategory() { Description = "<Unspecified>" };
        }


        /// <summary>
        /// Get all descendants Categories identifiers.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public IEnumerable<long> GetDescendats(long categoryId)
        {
            var self = this.context.MoneyCategories.FirstOrDefault(x => x.ID == categoryId);

            var childrenByParentId = this.context.MoneyCategories.ToLookup(r => r.ParentId);

            return this.context.MoneyCategories.Where(x => x.ParentId == self.ID).Expand(x => childrenByParentId[x.ID])
                .Select(x => x.ID);
        }

        private IEnumerable<GroupCategory> GetStructuredCategories()
        {
            return this.context.MoneyCategories.GroupBy(x => x.ParentId.HasValue ? x.ParentId : x.ID)
                .ToList()
                .Select(x =>
                {
                    var items = x.ToList();
                    if (items.Count > 1)
                    {
                        var singleItem = items.First();
                        return new GroupCategory()
                        {
                            ID = singleItem.ID,
                            Description = singleItem.Description,
                            Related = new List<Category>(items.Select(x => new Category() { ID = x.ID, Description = x.Description }).ToArray())
                        };
                    }
                    else
                    {
                        var singleItem = items.First();
                        return new GroupCategory()
                        {
                            ID = singleItem.ID,
                            Description = singleItem.Description
                        };
                    }
                });
        }
    }
}
