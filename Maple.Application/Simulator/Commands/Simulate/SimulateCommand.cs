using ErrorOr;
using Maple.Application.Simulator.Common;
using Maple.Domain.Common;
using MediatR;

namespace Maple.Application.Simulator.Commands.Simulate;

public record SimulateCommand(string Netlist, List<ExportNode> ExportNodes, SimulatorMode Mode)
    : IRequest<ErrorOr<List<SimulateResult>>>;