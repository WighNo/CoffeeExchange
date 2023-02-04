using CoffeeExchange.Data.Context.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeExchange.Data.Context;

public class DataContext: DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(options => options.MigrationsAssembly("CoffeeExchange"));
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<DealRecord> DealRecords => Set<DealRecord>();
}