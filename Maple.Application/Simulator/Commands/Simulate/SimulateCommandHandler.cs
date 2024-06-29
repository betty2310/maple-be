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

    public async Task<ErrorOr<List<SimulateResult>>> Handle(SimulateCommand command,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        if (command.ExportNodes.Count == 0)
        {
            return Errors.Simulator.NoExportNodesProvided;
        }

        try
        {
            var result = _simulatorRepository.Run(command.Netlist, command.ExportNodes, command.Mode, command.Arguments);
            return result;
        }
        catch (Exception e)
        {
            return e is InvalidOperationException ? Errors.Simulator.MustHaveVoltageSource : Errors.Simulator.UnknownError;
        }
    }
}