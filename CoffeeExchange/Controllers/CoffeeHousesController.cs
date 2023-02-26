using AutoMapper;
using CoffeeExchange.Configs;
using CoffeeExchange.Data.Context;
using CoffeeExchange.Data.Context.Entities;
using CoffeeExchange.Data.Context.Extensions;
using CoffeeExchange.Data.Requests.Models;
using CoffeeExchange.Data.Response.Errors.BadRequest;
using CoffeeExchange.Data.Response.Errors.NotFound;
using CoffeeExchange.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeExchange.Controllers;

/// <summary>
/// Контроллер
/// </summary>
[ApiController]
[Route("coffee-house")]
public class CoffeeHousesController : ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    /// <summary>
    /// Конструктор класса
    /// </summary>
    /// <param name="dataContext">Провайдер данных</param>
    /// <param name="mapper">Маппер данных</param>
    public CoffeeHousesController(DataContext dataContext, IMapper mapper)
    {
        _mapper = mapper;
        _dataContext = dataContext;
    }

    /// <summary>
    /// Получить все кофейни
    /// </summary>
    /// <returns></returns>
    [HttpGet("all")]
    [ProducesResponseType(typeof(List<CoffeeHouse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        var coffeeHouses = await _dataContext.CoffeeHouses
            .AsNoTracking()
            .ToListAsync();

        return Ok(coffeeHouses);
    }

    /// <summary>
    /// Получить кофейню по ID
    /// </summary>
    /// <param name="id">ID Кофейни</param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CoffeeHouse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CoffeeHouseNotFound), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUnit(int id)
    {
        var coffeeHouse = await _dataContext.CoffeeHouses
            .AsNoTracking()
            .FirstOrDefaultAsync(coffeeHouse => coffeeHouse.Id == id);

        if (coffeeHouse is null)
            return new CoffeeHouseNotFound(id);

        return Ok(coffeeHouse);
    }

    /// <summary>
    /// Добавить товар в ассортимент кофейни
    /// </summary>
    /// <param name="coffeeHouseId">ID кофейни</param>
    /// <param name="request">Параметры</param>
    /// <returns></returns>
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost("{coffeeHouseId:int}/add-to-assortment")]
    [ProducesResponseType(typeof(List<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProductAlreadyInStock), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProductNotFound), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CoffeeHouseNotFound), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddToAssortment(int coffeeHouseId, [FromBody] AddProductToCoffeeHouseAssortmentRequest request)
    {
        var coffeeHouse = await _dataContext.CoffeeHouses
            .Include(coffeeHouse => coffeeHouse.Assortment)
            .FirstOrDefaultAsync(coffeeHouse => coffeeHouse.Id == coffeeHouseId);

        if (coffeeHouse is null)
            return new CoffeeHouseNotFound(coffeeHouseId);
        
        var product = await _dataContext.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId);

        if (product is null)
            return new ProductNotFound(request.ProductId);

        if (coffeeHouse.Assortment.Any(productInAssortment => productInAssortment.Product == product) == true)
            return new ProductAlreadyInStock(request.ProductId, coffeeHouseId);

        ProductInAssortment productInAssortment = new()
        {
            Product = product,
            Count = request.InitialCount,
        };

        coffeeHouse.Assortment.Add(productInAssortment);
        _dataContext.CoffeeHouses.Update(coffeeHouse);
        await _dataContext.SaveChangesAsync();

        return Ok(coffeeHouse.Assortment);
    }

    /// <summary>
    /// Сбросить стоимость всех товаров в кофейне до минимальной
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost("{coffeeHouseId:int}/reset-assortment-price/all")]
    [ProducesResponseType(typeof(List<ProductInAssortment>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CoffeeHouseNotFound), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPrice(int coffeeHouseId)
    {
        var coffeeHouse = await _dataContext.CoffeeHouses
            .Where(x => x.Id == coffeeHouseId)
            .Include(coffeeHouse => coffeeHouse.Assortment)
            .ThenInclude(assortment => assortment.Product)
            .FirstOrDefaultAsync();

        if (coffeeHouse is null)
            return new CoffeeHouseNotFound(coffeeHouseId);
        
        foreach (var productInAssortment in coffeeHouse.Assortment)
        {
            productInAssortment.Price = productInAssortment.Product.MinimalPrice;
        }

        _dataContext.ProductsInAssortments.UpdateRange(coffeeHouse.Assortment);
        _dataContext.CoffeeHouses.Update(coffeeHouse);

        await _dataContext.SaveChangesAsync();
        
        return Ok(coffeeHouse.Assortment);
    }

    /// <summary>
    /// Добавить новую кофейню
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost("add-new-point")]
    [ProducesResponseType(typeof(CoffeeHouse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddCoffeeHouse([FromBody] AddCoffeeHouseRequest request)
    {
        var coffeeHouse = _mapper.Map<CoffeeHouse>(request);
        await _dataContext.CoffeeHouses.AddAsync(coffeeHouse);
        await _dataContext.SaveChangesAsync();

        return Ok(coffeeHouse);
    }

    /// <summary>
    /// Изменение кол-ва товара в наличии
    /// </summary>
    /// <param name="productInAssortmentId">ID товара в ассортименте</param>
    /// <param name="request">Параметры</param>
    /// <returns></returns>
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("edit-stock-quantity/{productInAssortmentId:int}")]
    [ProducesResponseType(typeof(ProductInAssortment), StatusCodes.Status200OK)]
    public async Task<IActionResult> EditStockQuantity([FromQuery] int productInAssortmentId, [FromBody] ProductEditStockQuantityRequest request)
    {
        var productInAssortment = await _dataContext.ProductsInAssortments
            .FirstOrDefaultAsync(productInAssortment => productInAssortment.Id == productInAssortmentId);

        if (productInAssortment is null)
            return NotFound();
        
        productInAssortment.Count = request.Amount;
        _dataContext.ProductsInAssortments.Update(productInAssortment);
        await _dataContext.SaveChangesAsync();
        
        return Ok(productInAssortment);
    }  
}