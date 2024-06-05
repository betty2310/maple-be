namespace Maple.Domain.Entities;

public class Simulator
{
    public double Input { get; set; }
    public Dictionary<string, double> ExportNodes { get; set; } = new();
}