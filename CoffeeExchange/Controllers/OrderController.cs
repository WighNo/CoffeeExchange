using CoffeeExchange.Data.Context;
using CoffeeExchange.Data.Context.Entities;
using CoffeeExchange.Data.Requests.Models;
using CoffeeExchange.Data.Response.Errors.BadRequest;
using CoffeeExchange.Data.Response.Errors.NotFound;
using CoffeeExchange.Helpers;
using CoffeeExchange.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeExchange.Controllers;

/// <summary>
/// Контроллер заказов
/// </summary>
[ApiController]
[Route("order-controller")]
public class OrderController : ControllerBase
{
    private readonly DataContext _dataContext;

    /// <summary>
    /// Конструктор класса
    /// </summary>
    /// <param name="dataContext">Провайдер данных</param>
    public OrderController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    /// <summary>
    /// Получить товары в корзине пользователя
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(List<ProductInCart>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UserNotFound), StatusCodes.Status404NotFound)]
    public IActionResult GetUserCart()
    {
        var userId = HttpContext.GetUserIdClaim();
        var user = _dataContext.Users
            .AsNoTracking()
            .Include(u => u.Cart)
            .ThenInclude(cart => cart.Product)
            .FirstOrDefault(x => x.Id == userId);

        if (user is null)
            return new UserNotFound(userId);

        /*var productsInCart = user.Cart;

        var productsInAssortment = _dataContext.ProductsInAssortments
            .AsNoTracking()
            .Where(p => productsInCart.Select(pr => pr.Product).Contains(p.Product))
            .Include(x => x.Product);

        foreach (var product in productsInAssortment)
        {
            var target = productsInCart.First(x => x.Product.Id == product.Product.Id);
            target.IsStock = target.Count <= product.Count;
        }*/
        
        return Ok(user.Cart);
    }
    
    /// <summary>
    /// Оформить покупку
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPost("checkout")]
    [ProducesResponseType(typeof(List<ProductInCart>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UserNotFound), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(UserCartIsEmptyBadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Checkout([FromBody] OrderCheckoutRequest request)
    {
        var userId = HttpContext.GetUserIdClaim();

        var user = await _dataContext.Users
            .Include(u => u.Cart)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
            return new UserNotFound(userId);

        if (user.Cart.Count == 0)
            return new UserCartIsEmptyBadRequest(userId);

        var coffeeHouse = await _dataContext.CoffeeHouses
            .Where(coffeeHouse => coffeeHouse.Id == request.CoffeeHouseId)
            .Include(coffeeHouse => coffeeHouse.Assortment)
            .ThenInclude(productInAssortment => productInAssortment.Product)
            .FirstOrDefaultAsync(x => x.Id == request.CoffeeHouseId);

        if (coffeeHouse is null)
            return new CoffeeHouseNotFound(request.CoffeeHouseId);

        var targetAssortment = coffeeHouse.Assortment
            .Where(productInAssortment => user.Cart
                .Any(productInCart => productInCart.Product.Id == productInAssortment.Product.Id))
            .ToList();

        if (targetAssortment.Count == 0)
            return BadRequest();
        
        foreach (var productInCart in user.Cart)
        {
            var coffeeHouseProduct = targetAssortment.FirstOrDefault(x => x.Product.Id == productInCart.Product.Id);

            if (coffeeHouseProduct is null)
                return BadRequest();

            coffeeHouseProduct.Count -= productInCart.Count;

            if (coffeeHouseProduct.Count < 0)
                return BadRequest();
        }
        
        var cart = user.Cart;
        var cacheCart = new List<ProductInCart>(cart);
        
        cart.Clear();
        cart.Capacity = 0;

        ProductPriceHistoryRecordingService priceHistoryRecordingService = new(coffeeHouse, targetAssortment, cacheCart);
        var histories = priceHistoryRecordingService.CreateRecordsList();
        await _dataContext.ProductSalesHistories.AddRangeAsync(histories);
        
        IPricingService pricingService = new ProductPricingService(cacheCart, coffeeHouse.Assortment);
        pricingService.GeneratePrices();

        _dataContext.ProductInCarts.UpdateRange(cacheCart);
        _dataContext.ProductsInAssortments.UpdateRange(targetAssortment);
        await _dataContext.SaveChangesAsync();
        
        return Ok(cacheCart);
    }
}