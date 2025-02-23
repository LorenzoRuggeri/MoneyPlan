using Savings.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyPlan.Model.API.Report
{
    public class ReportBudgetPlan
    {
        public string Description { get; set; }

        public double TotalPercent { get; set; }

        public ReportPeriodAmountPercent[] Data { get; set; }
    }

    public class ReportPeriodAmountPercent
    {
        public string Period { get; set; }
        public double Amount { get; set; }

        public double Percent { get; set; }
    }
}
