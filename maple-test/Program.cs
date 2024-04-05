using SpiceSharp.Components;
using SpiceSharpParser;
using SpiceSharpParser.ModelReaders;

namespace Maple;

public class Program
{
    public static void Main()
    {
        string netlistString = string.Join(Environment.NewLine,
        "V1 in 0 1.0",
        "R1 in out 10k",
        "R2 out 0 20k",
        ".DC V1 -1 1 0.2",
        ".SAVE v(out)",
        ".END");

        var netlistText = string.Join(Environment.NewLine,
                       "Diode circuit",
                       "D1 OUT 0 1N914",
                       "V1 OUT 0 0",
                       ".model 1N914 D(Is=2.52e-9 Rs=0.568 N=1.752 Cjo=4e-12 M=0.4 tt=20e-9)",
                       ".DC V1 -1 1 10e-3",
                       ".SAVE v(D1)",
                       ".END");

        // Parse the SPICE netlist
        var parser = new SpiceNetlistParser();
        var parseResult = parser.ParseNetlist(netlistString);
        var spiceNetlist = parseResult.FinalModel;

        // Create a SpiceSharp reader and read the parsed model
        var reader = new SpiceSharpReader();
        var spiceSharpModel = reader.Read(spiceNetlist);

        // Get the circuit and simulation objects
        var circuit = spiceSharpModel.Circuit;
        var simulation = spiceSharpModel.Simulations.Single();

        // Output variable
        var outputList = new List<double>();

        // Catch exported data
        var export = spiceSharpModel.Exports.Find(e => e.Name == "v(out)");
        simulation.ExportSimulationData += (sender, args) =>
        {
            Console.WriteLine(export.Extract());
        };

        // Run the simulation
        simulation.Run(circuit);
    }
}
