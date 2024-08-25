using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Savings.Model;

namespace MoneyPlan.SPA.Components
{

    public partial class CategorySelector : ComponentBase
    {
        public MoneyCategory ParentCategory 
        {
            get; 
            set;
        }

        [Parameter]
        public long? CategoryId { get; set; }

        [Parameter]
        public EventCallback<long?> CategoryIdChanged { get; set; } = default!;

        [Parameter]
        public IEnumerable<MoneyCategory> Items { get; set; } = Enumerable.Empty<MoneyCategory>();

        /// <summary>
        /// Shows only the Categories that has no children.
        /// </summary>
        [Parameter]
        public bool HideChildren { get; set; }


        [Parameter]
        public EventCallback<long?> OnChanged { get; set; } = default!;

        protected override void OnInitialized()
        {
            //UpdateCategory(Value);
            base.OnInitialized();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            //base.OnAfterRender(firstRender);
            if (firstRender)
            {
                UpdateCategory(CategoryId);
            }
        }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            return base.SetParametersAsync(parameters);
        }

        private IEnumerable<CategoryViewModel> RenderItems
        {
            get
            {                
                if (Items == null)
                    return Array.Empty<CategoryViewModel>();

                // TODO: If we want to hide children we need to provide a flat list.
                //       Otherwise on the View Component the items will not be clickable. 
                //       That need to be fixed, of course. But for now we're going with a flat list.
                if (HideChildren)
                {
                    var aaa = Items.Where(x => (x.Children.Any() && x.ParentId != null) || x.ParentId == null)
                        //.GroupBy(x => x.ParentId)
                        .Select(y => 
                        {
                            return new CategoryViewModel
                            {
                                Parent = y
                            };
                        })
                        .ToList();
                    return aaa;

                    var partial = Items.Where(x => !x.Children.Any())
                        .Select(y =>
                        {
                            var result = new CategoryViewModel
                            {
                                Parent = y
                            };
                            return result;
                        });

                    return partial;
                    //Items.Where(x => x.)
                }
                else
                {
                    var listWithChildren = Items.Where(x => x.ParentId != null)
                        .GroupBy(x => x.ParentId)
                        .Select(y =>
                        {
                            var result = new CategoryViewModel
                            {
                                Parent = Items.First(z => z.ID == y.Key)
                            };
                            result.Children.AddRange(y);
                            return result;
                        });

                    var listWithoutChildren = Items.Where(x => x.ParentId == null)
                        .Where(x => !listWithChildren.Any(processed => processed.Parent.ID == x.ID))
                        .Select(y =>
                        {
                            return new CategoryViewModel
                            {
                                Parent = y
                            };
                        });

                    return listWithChildren.Union(listWithoutChildren);
                }
            }
        }

        private class CategoryViewModel
        {
            public MoneyCategory Parent { get; set; }
            public List<MoneyCategory> Children { get; set; } = new List<MoneyCategory>();
        }

        private void UpdateCategory(long? value)
        {
            if (value == null || Items == null)
                return;
            
            var one = Items?.Where(x => x.ParentId != null) ?? Enumerable.Empty<MoneyCategory>();
            var two = one.Where(x => x.ID == value);
            var three = two.Select(x => x.Parent).FirstOrDefault();
            
            if (three != null)
            {
                ParentCategory = three;
                //this.StateHasChanged();
            }
            //Value = value;
            CategoryIdChanged.InvokeAsync(value);
            OnChanged.InvokeAsync(value);
        }

    }
}