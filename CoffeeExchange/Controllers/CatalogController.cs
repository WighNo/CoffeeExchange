using CoffeeExchange.Data.Context;
using CoffeeExchange.Data.Context.Entities;
using CoffeeExchange.Data.Requests.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeExchange.Controllers;

// [Authorize]
[ApiController]
[Route("catalog")]
public class CatalogController : ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(ILogger<CatalogController> logger, DataContext dataContext)
    {
        _logger = logger;
        _dataContext = dataContext;
    }

    [HttpGet("get-products")]
    public IActionResult GetProducts()
    {
        var products = _dataContext.Products.AsNoTracking();
        return Ok(products);
    }

    [HttpPost("create-product")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        Product product = new(request);

        var result = await _dataContext.AddAsync(product);
        await _dataContext.SaveChangesAsync();

        return Ok(result.Entity);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("edit-product")]
    public async Task<IActionResult> EditProduct([FromQuery] int productId, [FromBody] CreateProductRequest request)
    {
        await _dataContext.Products.Where(p => p.Id == productId).ExecuteUpdateAsync(calls =>
            calls.SetProperty(product => product.Name, product => request.Name)
                .SetProperty(product => product.MinimalPrice, product => request.MinimalPrice));
        /*
        var entity = await _dataContext.Products.FirstOrDefaultAsync(product => product.Id == productId);

        if (entity is null)
            return NotFound();
        
        entity.Name = request.Name;
        entity.MinimalPrice = request.MinimalPrice;
        */

        return Ok();
    }
}