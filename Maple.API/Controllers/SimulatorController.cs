using Maple.Application.Simulator.Commands.Simulate;
using Maple.Application.Simulator.Common;
using Maple.Contracts.Simulator;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Maple.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SimulatorController(ISender mediator) : ApiController
{
    // POST api/<Simulator>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SimulationRequest simulation)
    {
        var command = new SimulateCommand(simulation.Netlist, simulation.ExportNodes, simulation.Mode);
        var response = await mediator.Send(command);
        return response.Match(
            result => Ok(Map(result)),
            Problem
        );
    }

    private static List<SimulationResponse> Map(List<SimulateResult> result)
    {
        return result.Select(r => new SimulationResponse
        {
            Input = r.Input,
            ExportNodes = r.ExportNodes,
        }).ToList();
    }
}