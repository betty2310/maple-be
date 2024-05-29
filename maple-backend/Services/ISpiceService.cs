using maple_backend.Models;

namespace maple_backend.Services;

public interface ISpiceService
{
    List<SimulationResponse> Run(SimulationRequest simulationRequest);
}