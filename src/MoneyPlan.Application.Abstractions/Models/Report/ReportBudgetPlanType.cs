using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyPlan.Application.Abstractions.Models.Report
{
    /// <summary>
    /// A model that represent items related to a BudgetPlanType.
    /// BudgetPlanType has a brief <see cref="Description"/>.
    /// </summary>
    public class ReportBudgetPlanType
    {
        public string Description { get; set; }

        public double TotalPercent { get; set; }

        public double TotalAmount => Data?.Sum(x => x.Amount) ?? 0;

        /// <summary>
        /// List of data associated to this BudgetPlanType.
        /// </summary>
        public ReportPeriodAmountPercent[] Data { get; set; }
    }

    public class ReportPeriodAmountPercent
    {
        public string Period { get; set; }

        public double Amount { get; set; }

        public double Percent { get; set; }
    }
}
