namespace Maple.Application.Simulator.Common;

public class SimulateResult
{
    public double Input { get; set; }
    public Dictionary<string, double> ExportNodes { get; set; } = new();
}