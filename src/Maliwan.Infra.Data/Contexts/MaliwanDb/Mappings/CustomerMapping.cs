using Maliwan.Domain.Core.Data;
using Maliwan.Domain.Maliwan.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Mappings;

public class CustomerMapping : EntityMap<Customer>
{
    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);

        builder.ToTable("Customers");

        builder.HasIndex(e => new { e.Document, e.DeletedAt });

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnOrder(2);

        builder.Property(e => e.Document)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnOrder(3);

        builder.Property(e => e.Type)
            .HasColumnOrder(4);

        builder.Property(e => e.CreatedAt)
            .HasColumnOrder(5);

        builder.Property(e => e.UpdatedAt)
            .HasColumnOrder(6);

        builder.Property(e => e.DeletedAt)
            .HasColumnOrder(7);

        #region Relationships

        builder.HasMany(e => e.Orders)
            .WithOne(e => e.Customer)
            .HasForeignKey(e => e.IdCustomer);

        #endregion
    }
}