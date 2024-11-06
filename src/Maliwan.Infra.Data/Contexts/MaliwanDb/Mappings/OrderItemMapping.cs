using Maliwan.Domain.Core.Data;
using Maliwan.Domain.MaliwanContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Mappings;

public class OrderItemMapping : EntityMap<OrderItem>
{
    public override void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("OrderItems");

        builder.HasIndex(e => new { e.IdOrder, e.IdStock, e.DeletedAt });

        builder.Property(e => e.IdOrder)
            .HasColumnOrder(2);

        builder.Property(e => e.IdStock)
            .HasColumnOrder(3);

        builder.Property(e => e.Quantity)
            .HasColumnOrder(4);

        builder.Property(e => e.UnitPrice)
            .HasPrecision(10, 2)
            .HasColumnOrder(5);

        builder.Property(e => e.Discount)
            .HasPrecision(10, 2)
            .HasColumnOrder(5);

        #region Relationships

        builder.HasOne(e => e.Order)
            .WithMany(e => e.OrderItems)
            .HasForeignKey(e => e.IdOrder);

        builder.HasOne(e => e.Stock)
            .WithMany(e => e.OrderItems)
            .HasForeignKey(e => e.IdStock);

        #endregion
    }
}