using Maliwan.Domain.Core.Data;
using Maliwan.Domain.MaliwanContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Mappings;

public class BrandMapping : EntityIntIdMap<Brand>
{
    public override void Configure(EntityTypeBuilder<Brand> builder)
    {
        base.Configure(builder);

        builder.ToTable("Brands");

        builder.HasIndex(e => new { e.Name, e.DeletedAt });

        builder.HasIndex(e => new { e.Sku, e.DeletedAt });

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnOrder(2);

        builder.Property(e => e.Sku)
            .IsRequired()
            .HasMaxLength(5)
            .HasColumnOrder(3);

        builder.Property(e => e.Active)
            .HasColumnOrder(4);

        builder.Property(e => e.CreatedAt)
            .HasColumnOrder(5);

        builder.Property(e => e.UpdatedAt)
            .HasColumnOrder(6);

        builder.Property(e => e.DeletedAt)
            .HasColumnOrder(7);

        #region Relationships

        builder.HasMany(e => e.Products)
            .WithOne(e => e.Brand)
            .HasForeignKey(e => e.IdBrand);

        #endregion
    }
}