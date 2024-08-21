using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Savings.Model;
using System.Reflection.Emit;
using System;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity;

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