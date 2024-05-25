using maple_backend.Models;

namespace maple_backend.Services;

public interface ISpiceService
{
    SimulationResponse Run(SimulationRequest simulationRequest);
}