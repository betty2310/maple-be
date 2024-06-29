using Maple.Domain.Common;
using Maple.Domain.Entities.SimulatorMode;

namespace Maple.Contracts.Simulator;

public class SimulationRequest
{
    public string Netlist { get; set; } = null!;
    public string? Title { get; set; }
    public List<ExportNode> ExportNodes { get; set; } = null!;
    public SimulatorMode Mode { get; set; }
    public Arguments Arguments { get; set; } = null!;
}