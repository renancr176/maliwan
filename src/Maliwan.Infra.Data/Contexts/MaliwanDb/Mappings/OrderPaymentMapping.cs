using Maliwan.Domain.Core.Data;
using Maliwan.Domain.Maliwan.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Mappings;

public class OrderPaymentMapping : EntityMap<OrderPayment>
{
    public override void Configure(EntityTypeBuilder<OrderPayment> builder)
    {
        base.Configure(builder);

        builder.ToTable("OrderPayments");

        builder.Property(e => e.IdOrder)
            .HasColumnOrder(2);
        
        builder.Property(e => e.IdPaymentMethod)
            .HasColumnOrder(3);
        
        builder.Property(e => e.AmountPaid)
            .HasPrecision(10, 2)
            .HasColumnOrder(4);

        builder.Property(e => e.PaymentDate)
            .HasColumnOrder(5);

        builder.Property(e => e.CreatedAt)
            .HasColumnOrder(6);

        builder.Property(e => e.UpdatedAt)
            .HasColumnOrder(7);

        builder.Property(e => e.DeletedAt)
            .HasColumnOrder(8);

        #region Relationships

        builder.HasOne(e => e.Order)
            .WithMany(e => e.OrderPayments)
            .HasForeignKey(e => e.IdOrder);

        builder.HasOne(e => e.PaymentMethod)
            .WithMany(e => e.OrderPayments)
            .HasForeignKey(e => e.IdPaymentMethod);

        #endregion
    }
}