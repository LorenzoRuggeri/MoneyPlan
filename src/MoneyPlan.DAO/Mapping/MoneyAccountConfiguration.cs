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
    internal class MoneyAccountConfiguration : IEntityTypeConfiguration<MoneyAccount>
    {
        public void Configure(EntityTypeBuilder<MoneyAccount> builder)
        {
            builder.ToTable("MoneyAccounts");

            builder.HasKey(x => x.ID);

            builder.HasMany(x => x.FixedMoneyItems)
                .WithOne(x => x.Account)
                .HasForeignKey(x => x.AccountID);
        }
    }
}
