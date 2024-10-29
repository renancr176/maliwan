using Maliwan.Domain.Core.DomainObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Maliwan.Domain.Core.Data;

public abstract class EntityMap<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : Entity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .HasColumnOrder(1);

        builder.Property(entity => entity.CreatedAt)
            .IsRequired();

        builder.Ignore(entity => entity.Notifications);

        builder.HasQueryFilter(entity => !entity.DeletedAt.HasValue);
    }
}

public abstract class EntityIntIdMap<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : EntityIntId
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .UseIdentityColumn()
            .HasColumnOrder(1);

        builder.Property(entity => entity.CreatedAt)
            .IsRequired();

        builder.Ignore(entity => entity.Notifications);

        builder.HasQueryFilter(entity => !entity.DeletedAt.HasValue);
    }
}