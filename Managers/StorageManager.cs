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

        private bool isSymLink(string fullPath) {
            if (!Directory.Exists(fullPath)) { return false; }

            DirectoryInfo info = new DirectoryInfo(fullPath);

            return info.LinkTarget is not null;
        }

        private (int, int, long) calcCountsAndSize(string fullPath) {
            if (File.Exists(fullPath)) {
                FileInfo info = new FileInfo(fullPath);
                return (0, 0, info.Length);
            }

            if (!Directory.Exists(fullPath)) { return (0, 0, 0); }

            // Just in case, symlinks don't exist don't want infinite loops
            if (isSymLink(fullPath)) { return (0, 0, 0); }

            int folderCount = 0;
            int fileCount = 0;
            long size = 0;

            var dirs = Directory.EnumerateDirectories(fullPath);
            var files = Directory.EnumerateFiles(fullPath);

            folderCount += dirs.Count();
            fileCount += files.Count();

            foreach (string entry in dirs.Concat(files)) {
                (int nextFolderCount, int nextFileCount, long nextSize)
                    = calcCountsAndSize(entry);

                folderCount += nextFolderCount;
                fileCount += nextFileCount;
                size += nextSize;
            }

            return (folderCount, fileCount, size);
        }

        public PathInfo? GetPathInfo(string path) {
            string fullPath = getFullPath(path);

            if (Directory.Exists(fullPath)) {

                var dirs = Directory.EnumerateDirectories(fullPath);
                var files = Directory.EnumerateFiles(fullPath);

                var entries = dirs.Select(d => {
                        (int dc, int fc, long s) = calcCountsAndSize(d);
                        return new DirectoryEntry {
                            Name = Path.GetFileName(d),
                            IsDirectory = true,
                            FolderCount = dc,
                            FileCount = fc,
                            Size = s,
                        };
                }).Concat(
                    files.Select(f => {
                        (int dc, int fc, long s) = calcCountsAndSize(f);
                        return new DirectoryEntry {
                          Name = Path.GetFileName(f),
                          IsDirectory = false,
                          Size = s,
                        };
                    })
                ).ToList();

                (int folderCount, int fileCount, long size) =
                    calcCountsAndSize(fullPath);

                return new PathInfo {
                    Path = path,
                    FullPath = fullPath,
                    IsDirectory = true,
                    Entries = entries,
                    FolderCount = folderCount,
                    FileCount = fileCount,
                    Size = size,
                    FileContents = null,
                };
            } else if (File.Exists(fullPath)) {
                // If the file is a text or markdown file, send it's contents
                string? contents = null;
                var ext = Path.GetExtension(fullPath).ToLower();
                if (ext == ".txt" || ext == ".md") {
                    contents = File.ReadAllText(fullPath);
                }

                (_, _, long size) = calcCountsAndSize(fullPath);

                return new PathInfo {
                    Path = path,
                    FullPath = fullPath,
                    IsDirectory = false,
                    Size = size,
                    FileContents = contents,
                };
            } else {
                throw new DirectoryNotFoundException($"The path '{path}' does not exist.");
            }
        }

        public FileInfo? GetFileInfo(string path) {
            string fullPath = getFullPath(path);

            if (File.Exists(fullPath)) {
                FileInfo fileInfo = new FileInfo(fullPath);

                return fileInfo;
            } else {
                return null;
            }
        }

        public async Task<bool> UploadFile(string path, IFormFile formFile) {
            string fullPath = getFullPath(path);

            Console.WriteLine($"Received file for path {path} with size {formFile.Length}B");

            string newFilePath = $"{fullPath}/{formFile.FileName}";

            if (File.Exists(newFilePath) || Directory.Exists(newFilePath)) {
                Console.WriteLine($"File already exists at path {newFilePath}");
                return false;
            }

            Console.WriteLine($"Creating new file at path {newFilePath}");

            // Security issues galore, but beyond the scope of this exercise
            using (FileStream? newFileStream = File.Create(newFilePath)) {
                await formFile.CopyToAsync(newFileStream);
            }

            return true;
        }
    }
}
