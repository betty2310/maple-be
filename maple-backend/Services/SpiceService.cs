using System.Text;
using maple_backend.Models;
using maple_backend.Utils;
using SpiceSharp.Simulations;
using SpiceSharpParser;
using SpiceSharpParser.Models.Netlist.Spice;

namespace maple_backend.Services;

public class SpiceService(ILogger<SpiceService> logger) : ISpiceService
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
        var netlistStr = netlistRequest.Netlist.Replace("\\n", "\n");
        LoggingUtil.LogMessage(logger, LogLevel.Information, netlistStr);

        var exportNode = netlistRequest.ExportNodes[0];

        LoggingUtil.LogMessage(logger, LogLevel.Information, exportNode.node);

        // Parsing part
        var netlist = Parser(netlistStr);

        // Translating netlist model to SpiceSharp
        var reader = new SpiceSharpReader();
        var spiceSharpModel = reader.Read(netlist);

        var circuit = spiceSharpModel.Circuit;
        var voltageSource = circuit.ToList().First(circuit => circuit.Name.Contains('V'));

        // Simulation using SpiceSharp
        var start = -1.0;
        var stop = 1.0;
        var step = 0.2;
        var dc = new DC("DC 1", voltageSource.Name, start, stop, step);
        var inputList = new List<double>();
        var outputList = new List<double>();

        var mode = netlistRequest.Mode;

        switch (mode)
        {
            case Mode.DCSweep:
                for (var i = start; i <= stop; i += step)
                {
                    inputList.Add(i);
                }
                dc.ExportSimulationData += (sender, args) =>
                {
                    var output = args.GetVoltage(exportNode.node);
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
                    var output = args.GetVoltage(exportNode.node);
                    outputList.Add(output);
                };
                tran.Run(circuit);
                break;
            case Mode.ACSweep:
                const double initial = 1e-2;
                const double finalValue = 1.0e3;
                const int pointsPerDecade = 5;
                var ac = new AC("AC 1", new DecadeSweep(initial, finalValue, pointsPerDecade));
                inputList = _generateFrequencyPoints(initial, finalValue, pointsPerDecade);
                var exportVoltage = new ComplexVoltageExport(ac, exportNode.node);
                ac.ExportSimulationData += (sender, args) =>
                {
                    var output = exportVoltage.Value;
                    var decibels = 10.0 * Math.Log10(output.Real * output.Real + output.Imaginary * output.Imaginary);
                    outputList.Add(decibels);
                };
                ac.Run(circuit);
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
    private List<double> _generateFrequencyPoints(double initial, double final, int pointsPerDecade)
    {
        var frequencyPoints = new List<double>();

        var decades = Math.Log10(final) - Math.Log10(initial);
        var totalPoints = (int)(decades * pointsPerDecade);

        for (var i = 0; i <= totalPoints; i++)
        {
            var frequency = initial * Math.Pow(10, (double)i / pointsPerDecade);
            frequencyPoints.Add(frequency);
        }

        return frequencyPoints;
    }
}