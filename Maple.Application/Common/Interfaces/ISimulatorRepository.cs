using Maple.Application.Simulator.Common;
using Maple.Domain.Common;
using Maple.Domain.Entities.SimulatorMode;

namespace Maple.Application.Common.Interfaces;

public interface ISimulatorRepository
{
    List<SimulateResult> Run(string netlist, List<ExportNode> exportNodes, SimulatorMode mode, Arguments arguments);
}