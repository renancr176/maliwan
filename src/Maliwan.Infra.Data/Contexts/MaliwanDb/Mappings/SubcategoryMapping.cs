using Maliwan.Domain.Core.Data;
using Maliwan.Domain.Maliwan.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Mappings;

public class SubcategoryMapping : EntityIntIdMap<Subcategory>
{
    public override void Configure(EntityTypeBuilder<Subcategory> builder)
    {
        base.Configure(builder);

        builder.ToTable("Subcategories");

        builder.HasIndex(e => new { e.Name, e.DeletedAt });

        builder.HasIndex(e => new { e.Sku, e.DeletedAt });

        builder.Property(e => e.IdCategory)
            .HasColumnOrder(2);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnOrder(3);

        builder.Property(e => e.Sku)
            .IsRequired()
            .HasMaxLength(5)
            .HasColumnOrder(4);

        builder.Property(e => e.Active)
            .HasColumnOrder(5);

        builder.Property(e => e.CreatedAt)
            .HasColumnOrder(6);

        builder.Property(e => e.UpdatedAt)
            .HasColumnOrder(7);

        builder.Property(e => e.DeletedAt)
            .HasColumnOrder(8);

        #region Relationships

        builder.HasOne(e => e.Category)
            .WithMany(e => e.Subcategories)
            .HasForeignKey(e => e.IdCategory);

        builder.HasMany(e => e.Products)
            .WithOne(e => e.Subcategory)
            .HasForeignKey(e => e.IdSubcategory);

        #endregion
    }
}