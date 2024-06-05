using Maple.Domain.Common;

namespace Maple.Application.Common.Interfaces;

public interface ISimulatorRepository
{
    List<Domain.Entities.Simulator> Run(string netlist, List<ExportNode> exportNodes, SimulatorMode mode);
}