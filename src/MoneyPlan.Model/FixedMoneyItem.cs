using MoneyPlan.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Savings.Model
{
    public class FixedMoneyItem : ICloneable<FixedMoneyItem>
    {
        public long ID { get; set; }
        public DateTime Date { get; set; }
        public decimal? Amount { get; set; }
        public string Note { get; set; }

        public long? CategoryID { get; set; }

        public MoneyCategory Category { get; set; }

        public int AccountID { get; set; }

        public virtual MoneyAccount Account { get; set; }        

        public virtual MaterializedMoneyItem MaterializedMoneyItem { get; set; }

        public bool Cash { get; set; }

        /// <summary>
        /// Defines the order in which the <see cref="FixedMoneyItem"/> will be displayed.
        /// </summary>
        public int TimelineWeight { get; set; }

        public FixedMoneyItem Clone()
        {
            return new FixedMoneyItem()
            {
                Account = this.Account,
                AccountID = this.AccountID,
                Amount = this.Amount,
                Cash = this.Cash,
                Category = this.Category,
                CategoryID = this.CategoryID,
                Date = this.Date,
                ID = this.ID,
                Note = this.Note,
                TimelineWeight = this.TimelineWeight,
                MaterializedMoneyItem = this.MaterializedMoneyItem
            };
        }
    }
}
