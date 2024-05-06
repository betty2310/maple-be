namespace maple_backend.Models;

public enum Mode
{
    Transient,
    DC,
    AC,
}

public class NetlistRequest
{
    public string Netlist { get; set; }
    public string? Title { get; set; }
    public List<ExportNode> ExportNodes { get; set; }
    public Mode Mode { get; set; }
}