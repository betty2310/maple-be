namespace Maple.Contracts.Simulator;

public class SimulationResponse
{
    public double Input { get; set; }
    public Dictionary<string, double> ExportNodes { get; set; } = new();
}