namespace maple_backend.Models;

public enum ExportType
{
    V,
    I
}

public class ExportNode
{
    public ExportType type { get; set; }
    public string node { get; set; }
}