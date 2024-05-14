using System.Text;
using maple_backend.DiodeModels;
using maple_backend.Models;
using maple_backend.Utils;
using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Entities;
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

        // we need to remote the diode node and add my custom
        var diodeNodes = circuit.ToList().Where(circuit => circuit.Name.Contains('D')).ToList();
        if (diodeNodes.Count > 0)
        {
            foreach (var diodeNode in diodeNodes)
            {
                if (diodeNode is not Component cp) continue;
                var diode = new Diode(cp.Name, cp.Nodes[0], cp.Nodes[1], "1N914");
                circuit.Remove(diodeNode);
                circuit.Add(diode);
            }

            circuit.Add(new DiodeModel1N914().GetModel());
        }

        var transistorNodes = BjtTransistorHelper.GetTransistorsFromNetlist(netlistStr);

        if (transistorNodes.Count > 0)
        {
            foreach (var transistorNode in transistorNodes)
            {
                circuit.Add(transistorNode);
            }
            circuit.Add(BjtTransistorHelper.NPNmjd44h11Transistor());
        }

        var xAxisValues = new List<double>();
        var yAxisvalues = new List<double>();
        var mode = netlistRequest.Mode;

        switch (mode)
        {
            case Mode.DCSweep:
                SimulateDcSweep(voltageSource, xAxisValues, exportNode, yAxisvalues, circuit);
                break;
            case Mode.Transient:
                const double step_ = 1e-3;
                var final = 0.1;
                var tran = new Transient("Tran 1", step_, final);

                for (var i = 0.0; i <= final; i += step_)
                {
                    xAxisValues.Add(i);
                }

                var export = new RealVoltageExport(tran, exportNode.node);
                tran.ExportSimulationData += (sender, args) =>
                {
                    var output = export.Value;
                    yAxisvalues.Add(output);
                };
                try
                {
                    tran.Run(circuit);
                }
                catch (ValidationFailedException e)
                {
                    LoggingUtil.LogMessage(logger, LogLevel.Error, e.Message);
                    yAxisvalues.Add(0);
                }

                break;
            case Mode.ACSweep:
                const double initial = 1e-2;
                const double finalValue = 1.0e3;
                const int pointsPerDecade = 5;
                var ac = new AC("AC 1", new DecadeSweep(initial, finalValue, pointsPerDecade));
                xAxisValues = _generateFrequencyPoints(initial, finalValue, pointsPerDecade);
                var exportVoltage = new ComplexVoltageExport(ac, exportNode.node);
                ac.ExportSimulationData += (sender, args) =>
                {
                    var output = exportVoltage.Value;
                    var decibels = 10.0 * Math.Log10(output.Real * output.Real + output.Imaginary * output.Imaginary);
                    yAxisvalues.Add(decibels);
                };
                ac.Run(circuit);
                break;
            default:
                break;
        }


        var response = new SimulationResponse
        {
            Input = xAxisValues,
            Output = yAxisvalues
        };

        return response;
    }

    private static void SimulateDcSweep(IEntity voltageSource, List<double> inputList, ExportNode exportNode,
        List<double> outputList,
        Circuit circuit)
    {
        const double start = 0;
        const double stop = 3;
        const double step = 0.1;
        var voltageSources= circuit.ToList().Where(c => c.Name.Contains('V')).ToList();
        var sweeps = voltageSources.Select(source => new ParameterSweep(source.Name, new LinearSweep(0, 3, 0.1))).ToList();

        var dc = new DC("DC 1", sweeps);
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
    }

    private static List<double> _generateFrequencyPoints(double initial, double final, int pointsPerDecade)
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