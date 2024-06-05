using ErrorOr;
using Maple.Domain.Common;

namespace Maple.Application.Services.Simulator.Queries;

public interface ISimulatorQueryService
{
    ErrorOr<List<Domain.Entities.Simulator>> Run(string netlist, List<ExportNode> exportNodes, SimulatorMode mode);
}
