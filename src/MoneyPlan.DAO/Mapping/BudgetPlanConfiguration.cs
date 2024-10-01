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
    internal class BudgetPlanConfiguration : IEntityTypeConfiguration<BudgetPlan>
    {
        public void Configure(EntityTypeBuilder<BudgetPlan> builder)
        {
            builder.ToTable("BudgetPlans");

            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.Rules)
                .WithOne(x => x.BudgetPlan)
                .HasForeignKey(x => x.BudgetPlanId);
                /*
                .UsingEntity<BudgetPlanBudgetRules>(j =>
                j.has);
                */
        }
    }
}
