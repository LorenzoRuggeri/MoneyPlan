using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Savings.Model;

namespace Savings.DAO.Mapping
{
    internal class MoneyCategoryConfiguration : IEntityTypeConfiguration<MoneyCategory>
    {
        public void Configure(EntityTypeBuilder<MoneyCategory> builder)
        {
            builder.HasKey(x => x.ID);

            builder
                .HasOne(s => s.Parent)
                .WithMany(m => m.Children)
                .HasForeignKey(e => e.ParentId);
        }
    }
}