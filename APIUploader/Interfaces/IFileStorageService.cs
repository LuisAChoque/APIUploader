using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string userId);
    }
}
