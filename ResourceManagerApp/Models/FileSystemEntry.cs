namespace ResourceManagerApp.Models;

public class FileSystemEntry
{
    public string Name { get; set; }
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public bool IsDirectory { get; set; }
}
