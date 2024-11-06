﻿using Maliwan.Domain.Core.Data;
using Maliwan.Domain.Maliwan.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Mappings;

public class CategoryMapping : EntityIntIdMap<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        base.Configure(builder);

        builder.ToTable("Categories");

        builder.HasIndex(e => new { e.Name, e.DeletedAt });

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnOrder(2);

        builder.Property(e => e.Active)
            .HasColumnOrder(3);
        
        builder.Property(e => e.CreatedAt)
            .HasColumnOrder(4);

        builder.Property(e => e.UpdatedAt)
            .HasColumnOrder(5);

        builder.Property(e => e.DeletedAt)
            .HasColumnOrder(6);

        #region Relationships

        builder.HasMany(e => e.SubCategories)
            .WithOne(e => e.Category)
            .HasForeignKey(e => e.IdCategory);

        #endregion
    }
}