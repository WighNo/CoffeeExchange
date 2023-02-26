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

    public DbSet<ProductInCart> ProductInCarts => Set<ProductInCart>();

    public DbSet<CoffeeHouse> CoffeeHouses => Set<CoffeeHouse>();
    
    public DbSet<ProductInAssortment> ProductsInAssortments => Set<ProductInAssortment>();

    public DbSet<ProductPriceHistory> ProductSalesHistories => Set<ProductPriceHistory>();
}