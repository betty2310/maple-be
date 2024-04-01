namespace maple_backend.Models;

public class NetlistRequest
{
    public string Netlist { get; set; }
    public string? Title { get; set; }
    public string ExportNode { get; set; }
}