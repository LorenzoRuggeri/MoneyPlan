using Savings.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MoneyPlan.Model
{
    /// <summary>
    /// Defines in which <see cref="BudgetPlanType"/> an item (it could be a <see cref="FixedMoneyItem" /> or a <see cref="MaterializedMoneyItem"/> ) falls.<br />
    /// Depending on its Category or on its Notes, for example.<br/>
    /// Each <see cref="BudgetPlanRule"/> will be applied only once, to prevent the same item being added to different <see cref="BudgetPlanType"/>.
    /// </summary>
    public class BudgetPlanRule : ICloneable
    {
        public int Id { get; set; }

        public int? BudgetPlanId { get; set; }

        public virtual BudgetPlan BudgetPlan {get;set;}


        public long? CategoryId { get; set; }

        public virtual MoneyCategory Category { get; set; }

        public StringFilterType CategoryFilter { get; set; }

        /// <summary>
        /// A text to be filtered using <see cref="CategoryFilter"/> condition.
        /// </summary>
        public string CategoryText { get; set; }

        // TODO: Ma questa perche' l'avevo messa null? Davvero una Rule non ha Type? O mi serviva solo per una Migration?
        public BudgetPlanType? Type {get;set; }

        [Obsolete("Proviamo a non utilizzarlo e a considerare che tutto quello di Income va calcolato", false)]
        /// <summary>
        /// If it's true the <see cref="Type"/> should be null.
        /// </summary>
        public bool Income { get; set; }

        public object Clone()
        {
            return new BudgetPlanRule()
            {
                Id = Id,
                Category = Category,
                CategoryFilter = CategoryFilter,
                CategoryId = CategoryId,
                CategoryText = CategoryText,
                Income = Income,
                Type = Type
            };
        }
    }

    public enum StringFilterType
    {
        [Display(Name = "None")]
        None = 0,
        [Display(Name = "Contains")]
        Contains = 1,
        [Display(Name = "Equals to")]
        Equals = 2
    }

    /// <summary>
    ///     <para>A Budget Plan needs to be validated against its Rules to be sure they don't collide.</para>
    ///     <para>A Budget Plan needs to be validated to be sure the Percentage reachs 100%</para>
    /// </summary>
    public class BudgetPlan
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual List<BudgetPlanRule> Rules { get; set; } = new List<BudgetPlanRule>();

        public int NeedsPercentage { get; set; }

        public int WantsPercentage { get; set; }

        public int SavingsPercentage { get; set; }
    }

    public enum BudgetPlanType
    {
        /// <summary>
        /// Used to exclude something from calculation
        /// </summary>
        None = 0,

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
        /// They could be transfered to another bank account or simply stored via cash.
        /// </summary>
        /// <remarks>20%</remarks>
        Savings = 3
    }
}
