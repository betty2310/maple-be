using Maple.Application.Services.Simulator.Queries;
using Maple.Contracts.Simulator;
using Microsoft.AspNetCore.Mvc;

namespace Maple.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SimulatorController(ISimulatorQueryService simulatorQueryService) : ApiController
{
    // POST api/<Simulator>
    [HttpPost]
    public IActionResult Post([FromBody] SimulationRequest simulation)
    {
        var response = simulatorQueryService.Run(simulation.Netlist, simulation.ExportNodes, simulation.Mode);
        return response.Match(
            Ok,
            Problem
        );
    }
}