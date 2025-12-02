using TestProject.Models;

namespace TestProject.Managers {
    public interface IStorageManager {
        PathInfo? GetPathInfo(string path);
    }
}
