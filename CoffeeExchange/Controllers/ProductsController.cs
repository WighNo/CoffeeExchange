using AutoMapper;
using CoffeeExchange.Configs;
using CoffeeExchange.Data.Context;
using CoffeeExchange.Data.Context.Entities;
using CoffeeExchange.Data.Requests.Models;
using CoffeeExchange.Data.Response.Errors.NotFound;
using CoffeeExchange.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeExchange.Controllers;

/// <summary>
/// Контроллер
/// </summary>
[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly ISaveServiceAsync<string> _productPhotosSaveService;

    /// <summary>
    /// Конструктор класса
    /// </summary>
    /// <param name="dataContext">Провайдер данных</param>
    /// <param name="mapper">Маппер данных</param>
    /// <param name="productPhotosSaveService">Сервис сохранения изображений</param>
    public ProductsController(DataContext dataContext, IMapper mapper, ProductPhotosSaveService productPhotosSaveService)
    {
        _dataContext = dataContext;
        _mapper = mapper;
        _productPhotosSaveService = productPhotosSaveService;
    }

    /*/// <summary>
    /// Получить историю продаж всех товаров
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IQueryable<ProductPriceHistory>), 200)]
    public IActionResult AllProductsSalesHistory()
    {
        var records = _dataContext.ProductSalesHistories.AsNoTracking();
        return Ok(records);
    }*/

    /// <summary>
    /// Получить историю продаж конкретного товара
    /// </summary>
    /// <param name="coffeeHouseId">ID кофейни</param>
    /// <param name="productId">ID товара</param>
    /// <returns></returns>
    [HttpGet("{coffeeHouseId:int}/{productId:int}/sales-history")]
    [ProducesResponseType(typeof(IQueryable<ProductPriceHistory>), StatusCodes.Status200OK)]
    public IActionResult ProductSalesHistory(int coffeeHouseId, int productId)
    {
        var records = _dataContext.ProductSalesHistories
            .AsNoTracking()
            .Where(x => x.Product.Id == productId && x.CoffeeHouse.Id == coffeeHouseId);

        return Ok(records);
    }
    
    /// <summary>
    /// Добавить новый товар
    /// </summary>
    /// <param name="request">Параметры товара</param>
    /// <returns></returns>
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost("create")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request)
    {
        if (ModelState.IsValid == false)
            return BadRequest();

        var formCollection = await HttpContext.Request.ReadFormAsync();
        var photo = formCollection.Files[request.FormImageKey];
        
        if (photo is null)
            return new FormFileNotFound(request.FormImageKey);

        var product = _mapper.Map<Product>(request);

        product.PhotoUrl = await _productPhotosSaveService.SaveAsync(photo);
        
        var result = await _dataContext.AddAsync(product);
        await _dataContext.SaveChangesAsync();

        return Ok(result.Entity);
    }

    /// <summary>
    /// Изменить существующий товар
    /// </summary>
    /// <param name="productId">ID товара</param>
    /// <param name="request">Новые параметры товара</param>
    /// <returns></returns>
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("edit")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProductNotFound), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EditProduct([FromQuery] int productId, [FromBody] CreateProductRequest request)
    {
        var product = await _dataContext.Products.FirstOrDefaultAsync(product => product.Id == productId);

        if (product is null)
            return NotFound(new ProductNotFound(productId));
        
        product = _mapper.Map<Product>(request);
        
        _dataContext.Products.Update(product);
        await _dataContext.SaveChangesAsync();
        
        return Ok(product);
    }

    /// <summary>
    /// Удалить товар
    /// </summary>
    /// <param name="productId">ID товара</param>
    /// <returns></returns>
    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduct([FromBody] int productId)
    {
        await _dataContext.Products.Where(p => p.Id == productId).ExecuteDeleteAsync();
        await _dataContext.SaveChangesAsync();
        return Ok();
    }
}