using Maple.Application.Simulator.Commands.Simulate;
using Maple.Contracts.Simulator;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Maple.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SimulatorController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public SimulatorController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    // POST api/<Simulator>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SimulationRequest simulation)
    {
        var command = _mapper.Map<SimulateCommand>(simulation);
        var response = await _mediator.Send(command);
        return response.Match(
            result => Ok(_mapper.Map<List<SimulationResponse>>(result)),
            Problem
        );
    }
}