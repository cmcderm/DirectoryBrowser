using TestProject.Models;

namespace TestProject.Managers {
    public interface IStorageManager {
        PathInfo? GetPathInfo(string path);
        FileInfo? GetFileInfo(string path);
        Task<bool> UploadFile(string path, IFormFile file);
    }
}
