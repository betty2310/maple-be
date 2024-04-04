using maple_backend.Models;
using maple_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace maple_backend
{
    [Route("api/[controller]")]
    [ApiController]
    public class Simulator(ISpiceService spiceService) : ControllerBase
    {
        // POST api/<Simulator>
        [HttpPost]
        public IActionResult Post([FromBody] NetlistRequest netlist)
        {
           var simulationOuput =  spiceService.Run(netlist);
           var response = new SimulationResponse
           {
               Node = netlist.ExportNode,
               Output = simulationOuput
           };
           return Ok(response);
        }
    }
}