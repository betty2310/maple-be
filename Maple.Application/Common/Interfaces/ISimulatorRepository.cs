using Maple.Application.Simulator.Common;
using Maple.Domain.Common;

namespace Maple.Application.Common.Interfaces;

public interface ISimulatorRepository
{
    List<SimulateResult> Run(string netlist, List<ExportNode> exportNodes, SimulatorMode mode);
}