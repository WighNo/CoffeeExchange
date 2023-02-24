using CoffeeExchange.Data.Context.Entities;

namespace CoffeeExchange.Data.Models.DTO;

public record CoffeeHouseWithAssortmentDto(CoffeeHouse CoffeeHouse, IQueryable<ProductInAssortment> Products);
