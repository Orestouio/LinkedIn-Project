using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ImageManipulation.Data.Services;

public enum FileFilterResult
{
    Ok,
    ExtensionNotAllowed,
    TooBig
}

public interface IFileService
{
    FileFilterResult FilterFile(IFormFile formFile, params Func<IFormFile, FileFilterResult>[] filters);
    Task<string> SaveFileAsync(IFormFile file, string directoryPath);
    void DeleteFile(string fileNameWithExtension);
    Task<byte[]> GetFileContents(string fileName, string directoryPath);

    
}

public class FileService(IWebHostEnvironment environment) : IFileService
{
    public FileFilterResult FilterFile(
        IFormFile formFile, 
        Func<IFormFile, FileFilterResult>[] filters
    )
    {
        foreach( var filter in filters )
        {
            FileFilterResult result = filter.Invoke(formFile);
            if(result is not FileFilterResult.Ok) return result;
        }
        return FileFilterResult.Ok;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string path)
    {
        ArgumentNullException.ThrowIfNull(file);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        // generate a unique filename
        var ext = Path.GetExtension(file.Name);
        var fileName = $"{Guid.NewGuid()}.{ext}";
        var fileNameWithPath = Path.Combine(path, fileName);
        using var stream = new FileStream(fileNameWithPath, FileMode.Create);
        await file.CopyToAsync(stream);
        return fileName;
    }

    public async Task<byte[]> GetFileContents(string fileName, string directoryPath)
    {
        var fullPath = $"{directoryPath}/{fileName}";
        if(!File.Exists(fullPath)) return [];
        return await File.ReadAllBytesAsync(fullPath);
    }


    public void DeleteFile(string fileNameWithExtension)
    {
        if (string.IsNullOrEmpty(fileNameWithExtension))
        {
            throw new ArgumentNullException(nameof(fileNameWithExtension));
        }
        var contentPath = environment.ContentRootPath;
        var path = Path.Combine(contentPath, $"Uploads", fileNameWithExtension);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Invalid file path");
        }
        File.Delete(path);
    }

}