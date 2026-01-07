using ERP.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ERP.SharedKernel.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FileService(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
    {
        _environment = environment;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder)
    {
        var uploadsFolder = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads", folder);
        
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

        return Path.Combine("uploads", folder, uniqueFileName).Replace("\\", "/");
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return false;

        var fullPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, filePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return true;
        }

        return false;
    }

    public string GetFileUrl(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return string.Empty;

        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null)
            return filePath;

        return $"{request.Scheme}://{request.Host}/{filePath}";
    }
}
