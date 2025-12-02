using TestProject.Models;

namespace TestProject.Managers {
    public class StorageManager : IStorageManager {

        private readonly ILogger<StorageManager> _logger;
        private readonly string _basePath;

        public StorageManager(ILogger<StorageManager> logger, IConfiguration config) {
            _logger = logger;
            _basePath = config.GetValue<string>("StorageBasePath") ??
                Directory.GetCurrentDirectory() + "/data";

            if (!Directory.Exists(_basePath)) {
                Directory.CreateDirectory(_basePath);
            }
        }

        private string getFullPath(string relativePath) {
            return Path.Combine(_basePath, relativePath);
        }

        public PathInfo? GetPathInfo(string path) {
            string fullPath = getFullPath(path);

            if (Directory.Exists(fullPath)) {

                var dirs = Directory.EnumerateDirectories(fullPath);
                var files = Directory.EnumerateFiles(fullPath);

                var entries = dirs.Select(d => new DirectoryEntry {
                    Name = Path.GetFileName(d),
                    IsDirectory = true,
                }).Concat(
                    files.Select(f => new DirectoryEntry {
                        Name = Path.GetFileName(f),
                        IsDirectory = false,

                    })
                ).ToList();

                return new PathInfo {
                    Path = path,
                    IsDirectory = true,
                    Entries = entries,
                };
            } else if (File.Exists(fullPath)) {
                // Will make a separate endpoint for opening a file
                return new PathInfo {
                    Path = path,
                    IsDirectory = false,
                    Entries = null,
                };
            } else {
                throw new DirectoryNotFoundException($"The path '{path}' does not exist.");
            }
        }
    }
}
