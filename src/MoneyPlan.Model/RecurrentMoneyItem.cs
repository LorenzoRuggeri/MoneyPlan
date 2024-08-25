using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Savings.Model
{
    public class RecurrentMoneyItem
    {
        public long ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Amount { get; set; }
        public int RecurrencyInterval { get; set; }
        public RecurrencyType RecurrencyType { get; set; }
        public OccurrencyType OccurrencyType { get; set; }
        public string Note { get; set; }
        public MoneyType Type { get; set; }
        public MoneyCategory Category { get; set; }
        public long? CategoryID { get; set; }
        public virtual IEnumerable<RecurrentMoneyItem> AssociatedItems { get; set; }
        public int TimelineWeight { get; set; }
        public virtual IEnumerable<RecurrencyAdjustement> Adjustements { get; set; }
        public bool DefaultCredit { get; set; }

        // TODO: Questo e' correlato ad AssociatedItems.
        public long? RecurrentMoneyItemID { get; set; }

        public int? MoneyAccountId { get; set; }

        public virtual MoneyAccount MoneyAccount { get; set; }
    }


    public enum MoneyType
    {
        [Description("Rata")]
        InstallmentPayment,

        Others
    }

    public enum RecurrencyType
    {
        Day,
        Week,
        Month
    }

    public enum OccurrencyType
    {
        [Description("Any day")]
        AnyDay,

        [Description("Occurs only on a working day")]
        WorkingDay
    }
}
