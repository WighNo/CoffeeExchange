using CoffeeExchange.Data.Context.Entities;
using CoffeeExchange.Data.Requests.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeExchange.Data.Context.Extensions;

public static class UserExtensions
{
    public static async Task<User> AddProductToCart(this User user, DataContext dataContext, AddProductToCartRequest request)
    {
        var productInCart = user.Cart.FirstOrDefault(x => x.Product.Id == request.ProductId);

        if (productInCart is not null)
        {
            productInCart.Count += request.Count;
            dataContext.ProductInCarts.Update(productInCart);
            
            await dataContext.SaveChangesAsync();

            return user;
        }

        var source = await dataContext.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId);

        if (source is null)
            return user;

        ProductInCart newProductInCart = new()
        {
            Product = source,
            Count = request.Count
        };

        await dataContext.ProductInCarts.AddAsync(newProductInCart);
        user.Cart.Add(newProductInCart);
        dataContext.Users.Update(user);
        await dataContext.SaveChangesAsync();
        
        return user;
    }
}