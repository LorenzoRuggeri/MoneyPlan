using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Savings.Model
{
    public class FixedMoneyItem
    {
        public long ID { get; set; }
        public DateTime Date { get; set; }
        public decimal? Amount { get; set; }
        public string Note { get; set; }

        public long? CategoryID { get; set; }

        public MoneyCategory Category { get; set; }

        public bool Cash { get; set; }

        /// <summary>
        /// Defines the order in which the <see cref="FixedMoneyItem"/> will be displayed.
        /// </summary>
        public int TimelineWeight { get; set; }
    }
}
