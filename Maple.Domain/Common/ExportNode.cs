namespace Maple.Domain.Common;

public enum ExportType
{
    V,
    I
}

public class ExportNode
{
    public ExportType Type { get; set; }
    public string node { get; set; }
}