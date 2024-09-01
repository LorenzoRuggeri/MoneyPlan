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
    internal class FixedMoneyItemConfiguration : IEntityTypeConfiguration<FixedMoneyItem>
    {
        public void Configure(EntityTypeBuilder<FixedMoneyItem> builder)
        {
            builder.HasKey(x => x.ID);

            builder.HasOne(x => x.MaterializedMoneyItem)
                .WithOne(x => x.FixedMoneyItem);
        }
    }
}
