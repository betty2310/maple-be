using ErrorOr;
using Maple.Application.Common.Interfaces;
using Maple.Domain.Common;
using Maple.Domain.Common.Errors;

namespace Maple.Application.Services.Simulator.Queries;

public class SimulatorQueryService : ISimulatorQueryService
{
    private readonly ISimulatorRepository _simulatorRepository;
    
    public SimulatorQueryService(ISimulatorRepository simulatorRepository)
    {
        _simulatorRepository = simulatorRepository;
    }
    
    public ErrorOr<List<Domain.Entities.Simulator>> Run(string netlist, List<ExportNode> exportNodes, SimulatorMode mode)
    {
        if (exportNodes.Count == 0)
        {
            return Errors.Simulator.NoExportNodesProvided;
        }
        try 
        {
            var result = _simulatorRepository.Run(netlist, exportNodes, mode);
            return result;
        }
        catch (InvalidOperationException e)
        {
            return Errors.Simulator.MustHaveVoltageSource;
        }
        
    }
}