using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Savings.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPlan.DAO.Mapping
{
    internal class BudgetPlanBudgetRulesConfiguration : IEntityTypeConfiguration<BudgetPlanBudgetRules>
    {
        public void Configure(EntityTypeBuilder<BudgetPlanBudgetRules> builder)
        {
            builder.ToTable("BudgetPlanBudgetPlanRule");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.BudgetPlanId, x.BudgetPlanRuleId })
                .IsUnique();

            builder.HasOne(x => x.BudgetPlan)
                .WithMany(x => x.Rules)
                .HasForeignKey(x => x.BudgetPlanId);
            //.HasPrincipalKey(x => x.Id);

            builder.HasOne(x => x.BudgetPlanRule)
                .WithMany(x => x.BudgetPlans)
                .HasForeignKey(x => x.BudgetPlanRuleId);
                //.HasPrincipalKey(x => x.ID);

        }
    }
}
