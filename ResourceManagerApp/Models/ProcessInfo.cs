namespace ResourceManagerApp.Models;

public class ProcessInfo
{
    public string Name { get; set; }
    public int PID { get; set; }
    public double MemoryUsage { get; set; } // в MB
}