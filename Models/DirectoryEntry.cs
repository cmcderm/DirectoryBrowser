namespace TestProject.Models {
    public class DirectoryEntry {
        public string? Name { get; set; }
        public bool? IsDirectory { get; set; }
        public int FolderCount { get; set; }
        public int FileCount { get; set; }
        public long Size { get; set; }
    }
}
