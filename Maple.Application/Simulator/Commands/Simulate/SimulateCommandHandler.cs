using ErrorOr;
using Maple.Application.Common.Interfaces;
using Maple.Application.Simulator.Common;
using Maple.Domain.Common.Errors;
using MediatR;

namespace Maple.Application.Simulator.Commands.Simulate;

public class SimulateCommandHandler : IRequestHandler<SimulateCommand, ErrorOr<List<SimulateResult>>>
{
    private readonly ISimulatorRepository _simulatorRepository;
    
    public SimulateCommandHandler(ISimulatorRepository simulatorRepository)
    {
        _simulatorRepository = simulatorRepository;
    }
    
    public async Task<ErrorOr<List<SimulateResult>>> Handle(SimulateCommand command, CancellationToken cancellationToken)
    {
        if (command.ExportNodes.Count == 0)
        {
            return Errors.Simulator.NoExportNodesProvided;
        }
        try 
        {
            var result = _simulatorRepository.Run(command.Netlist, command.ExportNodes, command.Mode);
            return result;
        }
        catch (InvalidOperationException e)
        {
            return Errors.Simulator.MustHaveVoltageSource;
        }
    }
}