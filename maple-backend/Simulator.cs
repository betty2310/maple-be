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
           var response =  spiceService.Run(simulation);
           return Ok(response);
        }
    }
}