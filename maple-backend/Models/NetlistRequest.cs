namespace maple_backend.Models;

public enum Mode
{
    Interactive,
    Transient,
    DC,
    AC,
}

public class NetlistRequest
{
    public string Netlist { get; set; }
    public string? Title { get; set; }
    public string ExportNode { get; set; }
    public Mode Mode { get; set; }
}