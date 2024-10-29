using Maliwan.Domain.Core.Data;
using Maliwan.Domain.Core.Messages;
using Maliwan.Domain.IdentityContext.Entities;
using Maliwan.Infra.Data.Contexts.IdentityDb.Mappings;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Maliwan.Infra.Data.Contexts.IdentityDb;

public class IdentityDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid, IdentityUserClaim<Guid>
    , IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    , IUnitOfWork
{
    private readonly IMediator _mediatorHandler;

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, IMediator mediatorHandler)
        : base(options)
    {
        _mediatorHandler = mediatorHandler;
    }

    public IDbContextTransaction Transaction { get; private set; }

    #region DbSets

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseSqlServer(config.GetConnectionString("IdentityConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Ignore<Event>();

        #region Mappings

        builder.ApplyConfiguration(new UserMapping());
        builder.ApplyConfiguration(new RefreshTokenMapping());

        #endregion
    }

    public virtual async Task BeginTransaction()
    {
        if (Transaction == null)
            Transaction = await Database.BeginTransactionAsync();
    }

    public virtual async Task CreateSavepoint(string savePointName)
    {
        Transaction.CreateSavepoint(savePointName);
    }

    public virtual async Task RollbackToSavepoint(string savePointName)
    {
        Transaction.RollbackToSavepoint(savePointName);
    }

    public async Task<bool> Commit()
    {
        var sucesso = await base.SaveChangesAsync() > 0;
        if (Transaction != null)
        {
            Transaction.Commit();
            Transaction = null;
        }
        if (sucesso) await _mediatorHandler.PublishEvent(this);

        return sucesso;
    }
}