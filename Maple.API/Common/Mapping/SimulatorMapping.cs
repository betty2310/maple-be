using Maple.Application.Simulator.Commands.Simulate;
using Maple.Application.Simulator.Common;
using Maple.Contracts.Simulator;
using Mapster;

namespace Maple.API.Common.Mapping;

// use this class to register object mappings
public class SimulatorMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<SimulateCommand, SimulationRequest>();
        config.NewConfig<SimulateResult, SimulationResponse>();
    }
}