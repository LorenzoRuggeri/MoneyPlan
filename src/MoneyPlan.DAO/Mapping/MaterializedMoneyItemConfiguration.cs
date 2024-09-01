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
    internal class MaterializedMoneyItemConfiguration : IEntityTypeConfiguration<MaterializedMoneyItem>
    {
        public void Configure(EntityTypeBuilder<MaterializedMoneyItem> builder)
        {
            builder.HasKey(x => x.ID);

            builder.HasOne(x => x.RecurrentMoneyItem)
                .WithMany(x => x.MaterializedMoneyItems);

            builder.HasOne(x => x.FixedMoneyItem)
                .WithOne(x => x.MaterializedMoneyItem);
        }
    }
}
