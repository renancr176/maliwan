using Maliwan.Domain.Core.Data;
using Maliwan.Domain.Core.Messages;
using Maliwan.Domain.Maliwan.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Maliwan.Infra.Data.Contexts.MaliwanDb.Mappings;

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

    public DbSet<Brand> Brands { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Gender> Genders { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderPayment> OrderPayments { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductColor> ProductColors { get; set; }
    public DbSet<ProductSize> ProductSizes { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Subcategory> Subcategories { get; set; }

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

        builder.ApplyConfiguration(new BrandMapping());
        builder.ApplyConfiguration(new CategoryMapping());
        builder.ApplyConfiguration(new CustomerMapping());
        builder.ApplyConfiguration(new GenderMapping());
        builder.ApplyConfiguration(new OrderMapping());
        builder.ApplyConfiguration(new OrderItemMapping());
        builder.ApplyConfiguration(new OrderPaymentMapping());
        builder.ApplyConfiguration(new PaymentMethodMapping());
        builder.ApplyConfiguration(new ProductMapping());
        builder.ApplyConfiguration(new ProductColorMapping());
        builder.ApplyConfiguration(new ProductSizeMapping());
        builder.ApplyConfiguration(new StockMapping());
        builder.ApplyConfiguration(new SubcategoryMapping());

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