using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Savings.Model;

namespace Savings.DAO.Mapping
{
    internal class RecurrentMoneyItemConfiguration : IEntityTypeConfiguration<RecurrentMoneyItem>
    {
        public void Configure(EntityTypeBuilder<RecurrentMoneyItem> builder)
        {
            builder.HasKey(x => x.ID);

            builder.HasOne(s => s.MoneyAccount)
                .WithMany(m => m.RecurrentMoneyItems)
                .HasForeignKey(e => e.MoneyAccountId);

            builder.HasMany(x => x.MaterializedMoneyItems)
                .WithOne(x => x.RecurrentMoneyItem);
        }
    }
}