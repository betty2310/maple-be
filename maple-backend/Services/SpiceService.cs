using System.Text;
using maple_backend.Models;
using SpiceSharp.Simulations;
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

    public SimulationResponse Run(NetlistRequest netlistRequest)
    {
        var exportNode = netlistRequest.ExportNode;
        int startIndex = exportNode.IndexOf('(') + 1;
        int length = exportNode.IndexOf(')') - startIndex;
        string valueInsideParentheses = exportNode.Substring(startIndex, length);

        // Parsing part
        var netlist = Parser(netlistRequest.Netlist);

        // Translating netlist model to SpiceSharp
        var reader = new SpiceSharpReader();
        var spiceSharpModel = reader.Read(netlist);

        var circuit = spiceSharpModel.Circuit;
        var voltageSource = circuit.ToList().First(circuit => circuit.Name.Contains('V'));

        // Simulation using SpiceSharp
        //var simulation = spiceSharpModel.Simulations.Single();
        var export = spiceSharpModel.Exports.Find(e => e.Name == netlistRequest.ExportNode);
        var start = -1.0;
        var stop = 1.0;
        var step = 0.2;
        var dc = new DC("DC 1", voltageSource.Name, start, stop, step);
        var inputList = new List<double>();
        var outputList = new List<double>();

        var mode = netlistRequest.Mode;

        switch (mode)
        {
            case Mode.DC:
                for (var i = start; i <= stop; i += step)
                {
                    inputList.Add(i);
                }
                dc.ExportSimulationData += (sender, args) =>
                {
                    var output = args.GetVoltage(valueInsideParentheses);
                    outputList.Add(output);
                };

                dc.Run(circuit);
                break;
            case Mode.Transient:
                var step_ = 1e-2;
                var final = 1.0;
                var tran = new Transient("Tran 1", step_, final);

                for (var i = 0.0; i <= final; i += step_)
                {
                    inputList.Add(i);
                }
                tran.ExportSimulationData += (sender, args) =>
                {
                    var output = args.GetVoltage(valueInsideParentheses);
                    outputList.Add(output);
                };
                tran.Run(circuit);
                break;
            case Mode.AC:
                break;
            case Mode.Interactive:
                break;
            default:
                break;
        }


        

        var response = new SimulationResponse
        {
            Input = inputList,
            Output = outputList
        };

        return response;
    }
}