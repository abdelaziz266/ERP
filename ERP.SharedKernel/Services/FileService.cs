using ERP.SharedKernel.Exceptions;
using ERP.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ERP.SharedKernel.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
    private static readonly string[] AllowedImageMimeTypes = ["image/jpeg", "image/png", "image/gif", "image/webp"];

    public FileService(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
    {
        _environment = environment;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> UploadImageAsync(IFormFile file, string folder)
    {
        if (file == null || file.Length == 0)
            throw new AppException("File is required", 400);

        if (!IsValidImage(file))
            throw new AppException("Only image files (jpg, jpeg, png, gif, webp) are allowed", 400);

        using var stream = file.OpenReadStream();
        return await UploadFileAsync(stream, file.FileName, folder);
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

        return $"{filePath}";
    }

    private static bool IsValidImage(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var mimeType = file.ContentType.ToLowerInvariant();

        return AllowedImageExtensions.Contains(extension) && AllowedImageMimeTypes.Contains(mimeType);
    }
}
