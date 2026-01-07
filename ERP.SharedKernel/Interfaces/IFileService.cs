namespace ERP.SharedKernel.Interfaces;

public interface IFileService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder);
    Task<bool> DeleteFileAsync(string filePath);
    string GetFileUrl(string filePath);
}
