using Maple.Domain.Common;

namespace Maple.Contracts.Simulator;

public class SimulationRequest
{
    public string Netlist { get; set; } = null!;
    public string? Title { get; set; }
    public List<ExportNode> ExportNodes { get; set; } = null!;
    public SimulatorMode Mode { get; set; }
}