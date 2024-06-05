using System.Numerics;
using Maple.Application.Common.Interfaces;
using Maple.Domain.Common;
using Maple.Infrastructure.Common;
using Maple.Infrastructure.Common.DiodeModels;
using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Entities;
using SpiceSharp.Simulations;
using SpiceSharpParser;
using SpiceSharpParser.Models.Netlist.Spice;

namespace Maple.Infrastructure.Simulator;

public class SimulatorRepository : ISimulatorRepository
{
    public List<Domain.Entities.Simulator> Run(string netlist, List<ExportNode> exportNodes, SimulatorMode mode)
    {
        var netlistStr = netlist.Replace("\\n", "\n");

        var netlistParser = Parser(netlistStr);

        var reader = new SpiceSharpReader();
        var spiceSharpModel = reader.Read(netlistParser);

        var circuit = spiceSharpModel.Circuit;

        // Check if the circuit has a voltage source

        IEnumerable<IEntity> voltageSources = circuit.ToList().Where(e => e.Name.Contains('V'));
        var enumerable = voltageSources.ToList();
        if (enumerable.Count == 0)
        {
            throw new InvalidOperationException("Must have a voltage source");
        }

        // we need to remove the diode node and add my custom
        var diodeNodes = circuit.ToList().Where(e => e.Name.Contains('D')).ToList();
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

        // similar to transistor
        var transistorNodes = BjtTransistor.GetTransistorsFromNetlist(netlistStr);

        if (transistorNodes.Count > 0)
        {
            foreach (var transistorNode in transistorNodes)
            {
                circuit.Add(transistorNode);
            }

            circuit.Add(BjtTransistor.NpNmjd44H11Transistor());
        }

        var response = new List<Domain.Entities.Simulator>();

        switch (mode)
        {
            case SimulatorMode.DCSweep:
                var sweeps = enumerable
                    .Select(a => new ParameterSweep(a.Name, new LinearSweep(0, 3, 0.1)))
                    .ToList();

                var dc = new DC("dc 1", sweeps);

                var exports = exportNodes.Select<ExportNode, IExport<double>>(node =>
                    node.Type == ExportType.I
                        ? new RealPropertyExport(dc, node.node, "i")
                        : new RealVoltageExport(dc, node.node)).ToList();


                response = _analyzeDC(dc, circuit, exports);
                break;
            case SimulatorMode.Transient:
                const double step = 1e-3;
                const double final = 0.1;
                var tran = new Transient("Tran 1", step, final);


                var exportsTransient = exportNodes.Select<ExportNode, IExport<double>>(node =>
                    node.Type == ExportType.I
                        ? new RealPropertyExport(tran, node.node, "i")
                        : new RealVoltageExport(tran, node.node)).ToList();


                response = _analyzeTransient(tran, circuit, exportsTransient);
                break;
            case SimulatorMode.ACSweep:
                var ac = new AC("AC 1", new DecadeSweep(1e-2, 1.0e3, 5));
                var exportsAc = exportNodes.Select<ExportNode, IExport<Complex>>(node =>
                    node.Type == ExportType.I
                        ? new ComplexPropertyExport(ac, node.node, "i")
                        : new ComplexVoltageExport(ac, node.node)).ToList();

                response = _analyzeAC(ac, circuit, exportsAc);
                break;
            default:
                break;
        }


        return response;
    }

    private static SpiceNetlist Parser(string netlistText)
    {
        var parser = new SpiceNetlistParser();
        var parseResult = parser.ParseNetlist(netlistText);
        var nestList = parseResult.FinalModel;
        return nestList;
    }

    /// <summary>
    /// Perform DC analysis
    /// </summary>
    /// <param name="sim">Simulation</param>
    /// <param name="ckt">Circuit</param>
    /// <param name="exports">Exports</param>
    private static List<Domain.Entities.Simulator> _analyzeDC(DC sim, Circuit ckt, IEnumerable<IExport<double>> exports)
    {
        var responses = new List<Domain.Entities.Simulator>();
        ArgumentNullException.ThrowIfNull(exports);

        sim.ExportSimulationData += (sender, data) =>
        {
            using var exportIt = exports.GetEnumerator();
            var response = new Domain.Entities.Simulator();
            var index = 0;

            while (exportIt.MoveNext())
            {
                var actual = exportIt.Current?.Value ?? double.NaN;
                var exportNodeKey = $"exportNodes[{index}]";
                response.ExportNodes[exportNodeKey] = actual;
                index++;
            }

            var values = sim.GetCurrentSweepValue();
            response.Input = values[0];

            responses.Add(response);
        };
        sim.Run(ckt);
        return responses;
    }

    /// <summary>
    /// Perform a test for transient analysis
    /// </summary>
    /// <param name="sim">Simulation</param>
    /// <param name="ckt">Circuit</param>
    /// <param name="exports">Exports</param>
    private static List<Domain.Entities.Simulator> _analyzeTransient(Transient sim, Circuit ckt,
        IEnumerable<IExport<double>> exports)
    {
        var responses = new List<Domain.Entities.Simulator>();

        ArgumentNullException.ThrowIfNull(exports);

        sim.ExportSimulationData += (sender, data) =>
        {
            using var exportsIt = exports.GetEnumerator();
            var response = new Domain.Entities.Simulator();

            var index = 0;
            while (exportsIt.MoveNext())
            {
                var actual = exportsIt.Current?.Value ?? throw new ArgumentNullException();
                var exportNodeKey = $"exportNodes[{index}]";
                response.ExportNodes[exportNodeKey] = actual;
                index++;
            }

            var t = data.Time;
            response.Input = t;

            responses.Add(response);
        };
        sim.Run(ckt);
        return responses;
    }

    private static List<Domain.Entities.Simulator> _analyzeAC(AC sim, Circuit ckt,
        IEnumerable<IExport<Complex>> exports)
    {
        var responses = new List<Domain.Entities.Simulator>();

        ArgumentNullException.ThrowIfNull(exports);

        sim.ExportSimulationData += (sender, data) =>
        {
            using var exportsIt = exports.GetEnumerator();
            var response = new Domain.Entities.Simulator();

            var index = 0;
            while (exportsIt.MoveNext())
            {
                var actual = exportsIt.Current?.Value ?? throw new ArgumentNullException();
                var exportNodeKey = $"exportNodes[{index}]";
                var decibels = 10.0 * Math.Log10(actual.Real * actual.Real + actual.Imaginary * actual.Imaginary);
                response.ExportNodes[exportNodeKey] = decibels;
                index++;
            }

            var t = data.Frequency;
            response.Input = t;

            responses.Add(response);
        };
        sim.Run(ckt);
        return responses;
    }
}