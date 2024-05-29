namespace maple_backend.Models;

public class SimulationResponse
{
    public double Input { get; set; }
    public Dictionary<string, double> ExportNodes { get; set; } = new();
}