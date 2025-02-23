using Savings.DAO.Infrastructure;
using Savings.Model;

namespace Savings.API.Services
{
    public class ReportService
    {
        private readonly SavingsContext context;

        public ReportService(SavingsContext context)
        {
            this.context = context;
        }

        public IEnumerable<ReportDetail> GetDetailsGroupedByCategory(IEnumerable<ReportFullDetail> details, long? category, string period)
        {
            var categories = GetStructuredCategories();

            var partial = details.Select(x => new { Category = CreateGroupCategory(x.CategoryID), ReportFullDetail = x });
            var partial2 = partial.Where(x => x.ReportFullDetail.Period == period && x.Category.HasCategory(category));
            
            var partial3 = partial2.Select(x => new ReportDetail
            {
                Description = x.ReportFullDetail.Description,
                Amount = x.ReportFullDetail.Amount,
                Date = x.ReportFullDetail.Date
            });
            
            return partial3;

            var results = details.GroupBy(x => 
                                categories.Where(y => y.ID == category || y.Related.Any(child => child.ID == category)).FirstOrDefault()
                           )
                .SelectMany(x => x)
                .Where(x => x.Period == period)
                .Select(x => new ReportDetail { Amount = x.Amount, Date = x.Date, Description = x.Description });

            return results;
        }



        //public IEnumerable<(GroupCategory category, IEnumerable<ReportFullDetail> details)> GetFullDetailsGroupedByCategory(IEnumerable<ReportFullDetail> details)
        public IEnumerable<GroupCategoryDetails> GetFullDetailsGroupedByCategory(IEnumerable<ReportFullDetail> details)
        {
            var categories = GetStructuredCategories();

            var results = details.Select(x => new { ReportFullDetail = x, Category = categories.Where(y => y.ID == x.CategoryID || y.Related.Any(child => child.ID == x.CategoryID)).FirstOrDefault() ?? new GroupCategory() { Description = "<Unspecified>" } })
                .GroupBy(x => x.Category)
                .Select(x => new GroupCategoryDetails { GroupCategory = x.Key, Details = x.Select(x => x.ReportFullDetail).ToList() });

            /*
            var first = details.GroupBy(x =>
            {
                var category = categories.Where(y => y.ID == x.CategoryID || y.Related.Any(child => child.ID == x.CategoryID)).FirstOrDefault();
                return category;
            });
            var results = first.Select(y => 
            {
                //return y.Select(x => new AAA { GroupCategory = y.Key, Details = y.ToList() });
                return new AAA { GroupCategory = y.Key, Details = y.ToList() };
            })                
            .ToList();
            */

            return results;
        }

        public class GroupCategoryDetails
        {
            public GroupCategory GroupCategory { get; set; }
            public List<ReportFullDetail> Details { get; set; } = new List<ReportFullDetail>();
        }

        internal GroupCategory CreateGroupCategory(long? categoryId)
        {
            var categories = GetStructuredCategories();
            return categories.Where(y => y.ID == categoryId || y.Related.Any(child => child.ID == categoryId)).FirstOrDefault() ?? new GroupCategory() { Description = "<Unspecified>" };
        }

        internal IEnumerable<GroupCategory> GetStructuredCategories()
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
