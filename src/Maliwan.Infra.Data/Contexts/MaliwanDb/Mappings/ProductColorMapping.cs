using Maliwan.Domain.Core.Data;
using Maliwan.Domain.Maliwan.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Mappings;

public class ProductColorMapping : EntityIntIdMap<ProductColor>
{
    public override void Configure(EntityTypeBuilder<ProductColor> builder)
    {
        base.Configure(builder);

        builder.ToTable("ProductColors");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnOrder(2);

        builder.Property(e => e.Sku)
            .IsRequired()
            .HasMaxLength(5)
            .HasColumnOrder(3);

        builder.Property(e => e.BgColor)
            .IsRequired()
            .HasMaxLength(7)
            .HasColumnOrder(4);

        builder.Property(e => e.TextColor)
            .IsRequired()
            .HasMaxLength(7)
            .HasColumnOrder(5);

        builder.Property(e => e.CreatedAt)
            .HasColumnOrder(6);

        builder.Property(e => e.UpdatedAt)
            .HasColumnOrder(7);

        builder.Property(e => e.DeletedAt)
            .HasColumnOrder(8);

        #region Relationships

        builder.HasMany(e => e.Stocks)
            .WithOne(e => e.Color)
            .HasForeignKey(e => e.IdColor)
            .IsRequired(false);

        #endregion
    }
}