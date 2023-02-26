namespace CoffeeExchange.Services;

/// <summary>
/// 
/// </summary>
public class ProductPhotosSaveService : ISaveServiceAsync<string>
{
    private readonly string _webRootDirectoryPath;

    /// <summary>
    /// Название папки для сохранения изображений
    /// </summary>
    public const string SaveFolder = "products";

    /// <summary>
    /// Конструктор класса
    /// </summary>
    /// <param name="webRootDirectoryPath">Путь до wwwroot. Задаётся в Program.cs</param>
    public ProductPhotosSaveService(string webRootDirectoryPath)
    {
        _webRootDirectoryPath = webRootDirectoryPath;
    }

    /// <summary>
    /// Сохранение изображения в wwwroot/SaveFolder
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public async Task<string> SaveAsync(IFormFile formFile)
    {
        if(formFile is null)
            throw new NullReferenceException("Нет целевого файла на сохранение");
        
        string saveDirectory = GetSaveDirectoryPath();

        string fileName = GenerateFileName(formFile.FileName);
        string savePath = GenerateSavePath(saveDirectory, fileName);
        
        await using Stream stream = new FileStream(savePath, FileMode.Create);
        await formFile.CopyToAsync(stream);

        return Path.Combine(SaveFolder, fileName);
    }
    
    private string GetSaveDirectoryPath()
    {
        string savePath = Path.Combine(_webRootDirectoryPath, SaveFolder);

        if (Directory.Exists(savePath) == false)
            Directory.CreateDirectory(savePath);

        return savePath;
    }

    private string GenerateFileName(string sourceFileName)
    {
        string sourceExtension = Path.GetExtension(sourceFileName);
        string randomFileName = Path.GetRandomFileName();
        
        string fileName = Path.ChangeExtension(randomFileName, sourceExtension);
        
        return fileName;
    }

    private string GenerateSavePath(string fullPathToSaveDirectory, string fileName)
    {
        string savePath = Path.Combine(fullPathToSaveDirectory, fileName);
        return savePath;
    }
}