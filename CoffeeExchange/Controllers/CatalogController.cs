using AutoMapper;
using CoffeeExchange.Data.Context;
using CoffeeExchange.Data.Context.Entities;
using CoffeeExchange.Data.Response.Errors.NotFound;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeExchange.Controllers;

/// <summary>
/// Контроллер каталога товаров
/// </summary>
[ApiController]
[Route("catalog")]
public class CatalogController : ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    
    /// <summary>
    /// Конструктор класса
    /// </summary>
    /// <param name="dataContext">Провайдер данных</param>
    /// <param name="mapper">Маппер данных</param>
    public CatalogController(DataContext dataContext, IMapper mapper)
    {
        _dataContext = dataContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Получить все товары
    /// </summary>
    /// <returns></returns>
    [HttpGet("all-products")]
    [ProducesResponseType(typeof(IQueryable<Product>), StatusCodes.Status200OK)]
    public IActionResult GetAllProducts()
    {
        var products = _dataContext.Products
            .AsNoTracking();
        
        return Ok(products);
    }
    
    /// <summary>
    /// Получить конкретный товар
    /// </summary>
    /// <param name="id">ID товара</param>
    /// <returns></returns>
    [HttpGet("product/{id:int}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProductNotFound), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _dataContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            return new ProductNotFound(id);
        
        return Ok(product);
    }

    /// <summary>
    /// Получить все товары в ассортименте кофейни
    /// </summary>
    /// <param name="coffeeHouseId">ID кофейни</param>
    /// <returns></returns>
    [HttpGet("products-in-coffee-house/{coffeeHouseId:int}")]
    [ProducesResponseType(typeof(List<ProductInAssortment>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CoffeeHouseNotFound), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductsInCoffeeHouse(int coffeeHouseId)
    {
        var coffeeHouse = await _dataContext.CoffeeHouses
            .AsNoTracking()
            .Include(coffeeHouse => coffeeHouse.Assortment)
            .ThenInclude(productInAssortment => productInAssortment.Product)
            .FirstOrDefaultAsync(coffeeHouse => coffeeHouse.Id == coffeeHouseId);

        if (coffeeHouse is null)
            return new CoffeeHouseNotFound(coffeeHouseId);

        return Ok(coffeeHouse.Assortment);
    }
}