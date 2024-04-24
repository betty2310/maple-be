namespace maple_backend.Models;

public class SimulationResponse
{
    public  List<double>? Input { get; set; }
    public required List<double> Output { get; set; }

}