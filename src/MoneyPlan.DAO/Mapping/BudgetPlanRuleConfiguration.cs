using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyPlan.Model;
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
            
            builder.Property(x => x.BudgetPlanId)
                .IsRequired(true);
            
            builder.HasOne(x => x.BudgetPlan)
                .WithMany(x => x.Rules)
                .HasForeignKey(x => x.BudgetPlanId)
                .HasPrincipalKey(x => x.Id)
                .IsRequired(true);
        }
    }
}
