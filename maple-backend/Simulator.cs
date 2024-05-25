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
        public IActionResult Post([FromBody] SimulationRequest simulation)
        {
            if (simulation.ExportNodes.Count == 0)
            {
                return BadRequest("Export nodes are required");
            }

            SimulationResponse response;
            try
            {
                response = spiceService.Run(simulation);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(response);
        }
    }
}