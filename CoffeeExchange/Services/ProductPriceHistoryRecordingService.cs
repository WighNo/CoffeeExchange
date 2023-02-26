using CoffeeExchange.Data.Context;
using CoffeeExchange.Data.Context.Entities;

namespace CoffeeExchange.Services;

/// <summary>
/// Сервис записи истории изменения цены на товар
/// </summary>
public class ProductPriceHistoryRecordingService
{
    private readonly CoffeeHouse _coffeeHouse;
    private readonly List<ProductInAssortment> _targetAssortment;
    private readonly List<ProductInCart> _productInCarts;

    /// <summary>
    /// Конструктор класса
    /// </summary>
    /// <param name="coffeeHouse">Кофейня</param>
    /// <param name="targetAssortment">Покупаемые товары из ассортимента кофейни</param>
    /// <param name="productInCarts">Покупаемые товары</param>
    public ProductPriceHistoryRecordingService(CoffeeHouse coffeeHouse, List<ProductInAssortment> targetAssortment,
        List<ProductInCart> productInCarts)
    {
        _coffeeHouse = coffeeHouse;
        _targetAssortment = targetAssortment;
        _productInCarts = productInCarts;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public List<ProductPriceHistory> CreateRecordsList()
    {
        DateTime dealDate = DateTime.UtcNow;
        List<ProductPriceHistory> histories = new();

        foreach (var productInCart in _productInCarts)
        {
            for (int i = 0; i < productInCart.Count; i++)
            {
                var productInAssortment = _targetAssortment.First(x => x.Product.Id == productInCart.Product.Id);
                decimal price = productInAssortment.Price;
                
                ProductPriceHistory record = new ProductPriceHistory()
                {
                    CoffeeHouse = _coffeeHouse,
                    Product = productInCart.Product,
                    Price = price,
                    Date = dealDate
                };
                
                histories.Add(record);
            }
        }

        return histories;
    }
}