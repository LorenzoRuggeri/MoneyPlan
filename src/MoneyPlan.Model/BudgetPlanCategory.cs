using System;
using System.Collections.Generic;
using System.Text;

namespace Savings.Model
{
    //TODO: definire il modello sul database
    /// <summary>
    /// Defines in which <see cref="BudgetPlanType"/> a <see cref="MoneyCategory" /> falls.
    /// </summary>
    public class BudgetPlanCategory
    {
        public int ID { get; set; }
        public long CategoryID { get; set; }
        public MoneyCategory Category { get; set; }
        public BudgetPlanType BudgetPlan {get;set; }
    }

    public enum BudgetPlanType
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>50%</remarks>
        Needs = 1,

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>30%</remarks>
        Wants = 2,

        /// <summary>
        /// They don't add up to Projection. They could be transfered to another bank account or simply stored via cash.
        /// </summary>
        /// <remarks>20%</remarks>
        Savings = 3
    }
}
