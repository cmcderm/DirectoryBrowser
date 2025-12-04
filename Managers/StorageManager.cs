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
            string path = Path.Combine(_basePath, relativePath);
            // Path.Combine() allows relativePath to override the _basePath and
            // allow the user to browse the entire server, this checks if the
            // new path has escaped the _basePath
            if (!path.Contains(_basePath)) {
              path = _basePath;
            }
            return path;
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
                    FullPath = fullPath,
                    IsDirectory = true,
                    Entries = entries,
                };
            } else if (File.Exists(fullPath)) {
                // If the file is a text or markdown file, send it's contents
                string? contents = null;
                var ext = Path.GetExtension(fullPath).ToLower();
                if (ext == ".txt" || ext == ".md") {
                    contents = File.ReadAllText(fullPath);
                }

                return new PathInfo {
                    Path = path,
                    FullPath = fullPath,
                    IsDirectory = false,
                    FileContents = contents,
                };
            } else {
                throw new DirectoryNotFoundException($"The path '{path}' does not exist.");
            }
        }
    }
}
