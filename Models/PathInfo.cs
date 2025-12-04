namespace TestProject.Models {
    public class PathInfo {
        public string? Path { get; set; }
        public string? FullPath { get; set; }
        public bool? IsDirectory { get; set; }
        public List<DirectoryEntry>? Entries { get; set; }
        public string? FileContents { get; set; }
    }
}
