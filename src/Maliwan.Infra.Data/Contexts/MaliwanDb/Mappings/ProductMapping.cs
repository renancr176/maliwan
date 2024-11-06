using Maliwan.Domain.Core.Data;
using Maliwan.Domain.Maliwan.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Mappings;

public class ProductMapping : EntityMap<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);

        builder.HasIndex(e => new { e.Name, e.IdBrand, e.IdSubcategory, e.IdGender, e.DeletedAt });

        builder.HasIndex(e => new { e.Sku, e.IdBrand, e.IdSubcategory, e.IdGender, e.DeletedAt });

        builder.Property(e => e.IdBrand)
            .HasColumnOrder(2);

        builder.Property(e => e.IdSubcategory)
            .HasColumnOrder(3);

        builder.Property(e => e.IdGender)
            .IsRequired(false)
            .HasColumnOrder(4);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnOrder(5);

        builder.Property(e => e.UnitPrice)
            .HasPrecision(10, 2)
            .HasColumnOrder(6);

        builder.Property(e => e.Sku)
            .IsRequired()
            .HasMaxLength(5)
            .HasColumnOrder(7);

        builder.Property(e => e.Active)
            .HasColumnOrder(8);

        builder.Property(e => e.CreatedAt)
            .HasColumnOrder(9);

        builder.Property(e => e.UpdatedAt)
            .HasColumnOrder(10);

        builder.Property(e => e.DeletedAt)
            .HasColumnOrder(11);

        #region Relationships

        builder.HasOne(e => e.Brand)
            .WithMany(e => e.Products)
            .HasForeignKey(e => e.IdBrand);

        builder.HasOne(e => e.Subcategory)
            .WithMany(e => e.Products)
            .HasForeignKey(e => e.IdSubcategory);

        builder.HasOne(e => e.Gender)
            .WithMany(e => e.Products)
            .HasForeignKey(e => e.IdGender)
            .IsRequired(false);

        builder.HasMany(e => e.Stocks)
            .WithOne(e => e.Product)
            .HasForeignKey(e => e.IdProduct);

        #endregion
    }
}