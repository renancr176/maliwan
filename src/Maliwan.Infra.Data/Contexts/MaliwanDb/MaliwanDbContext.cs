using Maliwan.Domain.Core.Data;
using Maliwan.Domain.Core.Messages;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb;

public class MaliwanDbContext : DbContext, IUnitOfWork
{
    private readonly IMediator _mediatorHandler;

    public MaliwanDbContext(DbContextOptions<MaliwanDbContext> options, IMediator mediatorHandler)
        : base(options)
    {
        _mediatorHandler = mediatorHandler;
    }

    public IDbContextTransaction Transaction { get; private set; }

    #region DbSets

    //public DbSet<Object> Objects { get; set; }

    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                options => options.EnableRetryOnFailure());

            //optionsBuilder.UseMySql(config.GetConnectionString("DefaultConnection"),
            //        ServerVersion.AutoDetect(config.GetConnectionString("DefaultConnection")));
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Ignore<Event>();

        #region Mappings

        //builder.ApplyConfiguration(new ObjectMapping());

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