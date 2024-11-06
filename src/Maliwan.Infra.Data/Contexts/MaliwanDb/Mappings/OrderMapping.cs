using Maliwan.Domain.Core.Data;
using Maliwan.Domain.Maliwan.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Mappings;

public class OrderMapping : EntityIntIdMap<Order>
{
    public override void Configure(EntityTypeBuilder<Order> builder)
    {
        base.Configure(builder);

        builder.ToTable("Orders");

        builder.Property(e => e.IdCustomer)
            .HasColumnOrder(2);

        builder.Property(e => e.SellDate)
            .HasColumnOrder(3);

        builder.Property(e => e.CreatedAt)
            .HasColumnOrder(4);

        builder.Property(e => e.UpdatedAt)
            .HasColumnOrder(5);

        builder.Property(e => e.DeletedAt)
            .HasColumnOrder(6);

        #region Relationships

        builder.HasOne(e => e.Customer)
            .WithMany(e => e.Orders)
            .HasForeignKey(e => e.IdCustomer);

        builder.HasMany(e => e.OrderItems)
            .WithOne(e => e.Order)
            .HasForeignKey(e => e.IdOrder);

        builder.HasMany(e => e.OrderPayments)
            .WithOne(e => e.Order)
            .HasForeignKey(e => e.IdOrder);

        #endregion
    }
}