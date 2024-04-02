namespace maple_backend.Models;

public class SimulationResponse
{
    public required List<double> Output { get; set; }
    public required string Node { get; set; }
}