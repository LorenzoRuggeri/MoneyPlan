using MoneyPlan.Builder;
using MoneyPlan.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MoneyPlan.Application.Tests
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class DbSetup
    {
        private readonly DaoBuilder daoBuilder;

        public DbSetup(DaoBuilder daoBuilder)
        {
            this.daoBuilder = daoBuilder;
        }

        /// <summary>
        /// Create a basic setup, with MoneyCategories and one Budget Plan.
        /// </summary>
        public void CreateDefault()
        {
            daoBuilder.MoneyCategories.WithId(1).WithDescription("Salary").Build();
            daoBuilder.MoneyCategories.WithId(2).WithDescription("Bank")
                .WithChildren([
                    daoBuilder.MoneyCategories.WithId(3).WithDescription("Money Withdrawal").Build(),
                    daoBuilder.MoneyCategories.WithId(4).WithDescription("Bank Fees").Build(),
                    daoBuilder.MoneyCategories.WithId(6).WithDescription("Trading Account Transfer").Build()
                ])
                .Build();
            daoBuilder.MoneyCategories.WithId(5).WithDescription("Clothes and shoes").Build();

            var budgetPlan = daoBuilder.BudgetPlans.WithName("Default")
                .WithPercentages(50, 30, 20)
                //.WithRules(definedRules.Select(x => x.Id))
                .Build();

            daoBuilder.BudgetPlanRules.WithType(Model.BudgetPlanType.Needs).WithCategory(3).WithBudgetPlan(budgetPlan.Id).Build();
            daoBuilder.BudgetPlanRules.WithType(Model.BudgetPlanType.Needs).WithCategory(4).WithBudgetPlan(budgetPlan.Id).Build();
            daoBuilder.BudgetPlanRules.WithType(Model.BudgetPlanType.Wants).WithCategory(5).WithBudgetPlan(budgetPlan.Id).Build();
            daoBuilder.BudgetPlanRules.WithType(Model.BudgetPlanType.Savings).WithCategory(6).WithBudgetPlan(budgetPlan.Id).Build();
        }
    }
}
