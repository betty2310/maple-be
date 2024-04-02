using System.Text;
using maple_backend.Models;
using SpiceSharpParser;
using SpiceSharpParser.Models.Netlist.Spice;

namespace maple_backend.Services;

public class SpiceService : ISpiceService
{
    public SpiceNetlist Parser(string netlistText)
    {
        var parser = new SpiceNetlistParser();
        var parseResult = parser.ParseNetlist(netlistText);
        var nestList = parseResult.FinalModel;
        return nestList;
    }

    public List<double> Run(NetlistRequest netlistRequest)
    {
        Console.WriteLine(netlistRequest.ExportNode);
        
        // Parsing part
        var netlist = Parser(netlistRequest.Netlist);

        // Translating netlist model to SpiceSharp
        var reader = new SpiceSharpReader();
        var spiceSharpModel = reader.Read(netlist);

        // Simulation using SpiceSharp
        var simulation = spiceSharpModel.Simulations.Single();
        var export = spiceSharpModel.Exports.Find(e => e.Name == netlistRequest.ExportNode);
        
        var outputList = new List<double>();
        simulation.ExportSimulationData += (sender, args) =>
        {
            var output = Convert.ToDouble(export.Extract());
            outputList.Add(output);
        };

        simulation.Run(spiceSharpModel.Circuit);

        return outputList;
    }
}