namespace CoffeeExchange.Services;

/// <summary>
/// 
/// </summary>
public interface ISaveServiceAsync<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<T> SaveAsync(IFormFile targetFile);
}