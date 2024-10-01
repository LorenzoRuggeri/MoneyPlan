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
    internal class BudgetPlanRuleConfiguration : IEntityTypeConfiguration<BudgetPlanRule>
    {
        public void Configure(EntityTypeBuilder<BudgetPlanRule> builder)
        {
            builder.ToTable("BudgetPlanRules");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Income)
                .HasDefaultValue(false)
                .IsRequired();

            builder.HasMany(x => x.BudgetPlans)
                .WithOne(x => x.BudgetPlanRule)
                .HasForeignKey(x => x.BudgetPlanRuleId);
        }
    }
}
