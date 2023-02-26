using CoffeeExchange.Data.Context.Entities;

namespace CoffeeExchange.Services;

/// <summary>
/// Сервис формирования цен на товары
/// </summary>
public class ProductPricingService : IPricingService
{
    private readonly List<ProductInCart> _orderProducts;
    private readonly List<ProductInAssortment> _productInAssortments;

    /// <summary>
    /// Конструктор класса
    /// </summary>
    /// <param name="orderProducts">Товары в заказе</param>
    /// <param name="productInAssortments">Товары в ассортименте кофейни</param>
    public ProductPricingService(List<ProductInCart> orderProducts, List<ProductInAssortment> productInAssortments)
    {
        _orderProducts = orderProducts;
        _productInAssortments = productInAssortments;
    }

    /// <summary>
    /// Метод, формирующий новые цены
    /// </summary>
    public void GeneratePrices()
    {
        foreach (var productInAssortment in _productInAssortments)
        {
            decimal notCartProductWeight = 0.99999m;
            decimal cartProductWeight = 1.00001m;

            if (_orderProducts.Any(x => x.Product.Id == productInAssortment.Product.Id) == false)
                productInAssortment.Price *= notCartProductWeight;
            else
                productInAssortment.Price *= cartProductWeight;
        }
    }
}