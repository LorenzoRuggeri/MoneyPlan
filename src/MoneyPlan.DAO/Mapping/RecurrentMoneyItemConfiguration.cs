using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Savings.Model;
using System.Reflection.Emit;
using System;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity;

namespace Savings.DAO.Mapping
{
    internal class RecurrentMoneyItemConfiguration : IEntityTypeConfiguration<RecurrentMoneyItem>
    {
        public void Configure(EntityTypeBuilder<RecurrentMoneyItem> builder)
        {
            builder.HasKey(x => x.ID);

            builder
                .HasOne(s => s.MoneyAccount)
                .WithMany(m => m.RecurrentMoneyItems)
                .HasForeignKey(e => e.MoneyAccountId);
        }
    }
}