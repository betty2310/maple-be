using maple_backend.Models;
using SpiceSharpParser.Models.Netlist.Spice;

namespace maple_backend.Services;

public interface ISpiceService
{
    SimulationResponse Run(NetlistRequest netlistRequest);
}