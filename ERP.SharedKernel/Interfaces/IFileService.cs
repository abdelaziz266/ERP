using Microsoft.AspNetCore.Http;

namespace ERP.SharedKernel.Interfaces;

public interface IFileService
{
    Task<string> UploadImageAsync(IFormFile file, string folder);
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder);
    Task<bool> DeleteFileAsync(string filePath);
    string GetFileUrl(string filePath);
}
