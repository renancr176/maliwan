using Maliwan.Domain.Core.Data;
using Maliwan.Domain.MaliwanContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Mappings;

public class StockMapping : EntityMap<Stock>
{
    public override void Configure(EntityTypeBuilder<Stock> builder)
    {
        base.Configure(builder);

        builder.ToTable("Stocks");

        builder.Property(e => e.IdProduct)
            .HasColumnOrder(2);

        builder.Property(e => e.IdSize)
            .HasColumnOrder(3);

        builder.Property(e => e.IdColor)
            .HasColumnOrder(4);

        builder.Property(e => e.InputQuantity)
            .HasColumnOrder(5);

        builder.Property(e => e.InputDate)
            .HasColumnOrder(6);

        builder.Property(e => e.PurchasePrice)
            .HasPrecision(10, 2)
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

        builder.HasOne(e => e.Product)
            .WithMany(e => e.Stocks)
            .HasForeignKey(e => e.IdProduct);

        builder.HasOne(e => e.Size)
            .WithMany(e => e.Stocks)
            .HasForeignKey(e => e.IdSize)
            .IsRequired(false);

        builder.HasOne(e => e.Color)
            .WithMany(e => e.Stocks)
            .HasForeignKey(e => e.IdColor)
            .IsRequired(false);

        builder.HasMany(e => e.OrderItems)
            .WithOne(e => e.Stock)
            .HasForeignKey(e => e.IdStock);

        #endregion

        #region Dinamic/Ignored Properties

        builder.Ignore(e => e.CurrentQuantity);

        #endregion
    }
}