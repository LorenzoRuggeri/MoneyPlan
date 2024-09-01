using System;
using System.Collections.Generic;

namespace Savings.Model
{
    /// <summary>
    /// A projection that has been materialized.
    /// </summary>
    public class MaterializedMoneyItem
    {
        public long ID { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public MoneyType Type { get; set; }

        public long? CategoryID { get; set; }

        public virtual MoneyCategory Category { get; set; }

        public string Note { get; set; }
        public decimal Projection { get; set; }

        /// <summary>
        /// Tell us if it is the summary of the period.
        /// </summary>
        public bool EndPeriod { get; set; }

        public int TimelineWeight { get; set; }
        public bool IsRecurrent { get; set; }
        
        public long? RecurrentMoneyItemID { get; set; }

        public virtual RecurrentMoneyItem RecurrentMoneyItem { get; set; }

        public long? FixedMoneyItemID { get; set; }

        public virtual FixedMoneyItem FixedMoneyItem { get; set; }

        public bool Cash { get; set; }
        public decimal EndPeriodCashCarry { get; set; }

        // TODO: Ma questo serve davvero metterlo sul DB? Investigare per capire se e' possibile farlo a posteriori.
        public IEnumerable<MaterializedMoneySubitems> Subitems { get; set; } = new List<MaterializedMoneySubitems>();
    }

    public class MaterializedMoneySubitems
    {
        public long ID { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public long? CategoryID { get; set; }
        public MoneyCategory Category { get; set; }
        public string Note { get; set; }
    }
}
