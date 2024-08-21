using System;
using System.Collections.Generic;
using System.Text;

namespace Savings.Model
{
    /// <summary>
    /// Rule that tell us how a <see cref="MaterializedMoneyItem"/> should be threaten while persisting it.
    /// </summary>
    public class MaterializedMoneyRule
    {
        public long ID { get; set; }
        public long RelatedID { get; set; }
        public ItemType Type { get; set; }
        /// <summary>
        /// It's displayed but it's not taken in account while suming values.
        /// </summary>
        public bool Exclude { get; set; }
    }

    public enum ItemType
    {
        Category = 1,
        FixedItem = 2
    }
}
