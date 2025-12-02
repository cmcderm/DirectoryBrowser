namespace TestProject.Models {
    public class PathInfo {
        public string? Path { get; set; }
        public bool? IsDirectory { get; set; }
        public List<DirectoryEntry>? Entries { get; set; }
    }
}
