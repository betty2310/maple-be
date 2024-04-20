﻿using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Simulations;
using SpiceSharpParser;
using SpiceSharpParser.ModelReaders;
using System.Text;
using System.Text.Json;

namespace Maple;

public abstract class Roles
{
    public const string Administrator = nameof(Administrator);
}

public class Program
{
    public static void JsonSerialization()
    {
        var voltage = new VoltageSource("V1", "in", "0", 1.0);
        var json = JsonSerializer.Serialize(voltage);
        Console.WriteLine(json);
    }

    public static void SpiceSharp()
    {
        var ckt = new Circuit(
            new VoltageSource("V1", "in", "0", 1.0),
            new Resistor("R1", "in", "out", 1.0e4),
            new Resistor("R2", "out", "0", 2.0e4)
        );

        var dc = new DC("DC 1", "V1", -1.0, 1.0, 0.2);

        // Catch exported data
        var sb = new StringBuilder();
        dc.ExportSimulationData += (sender, args) =>
        {
            var input = args.GetVoltage("in");
            var output = args.GetVoltage("out");
            sb.AppendLine($"V(in)={input} V(out)={output}");
        };
        dc.Run(ckt);
    }

    public static void Main()
    {
        Console.WriteLine(Roles.Administrator);
        SpiceSharpParse();
    }

    public static void SpiceSharpParse()
    {
        string netlistString = string.Join(Environment.NewLine,
        "V1 in 0 1.0",
        "R1 in out 10k",
        "R2 out 0 20k",
        ".DC V1 -1 1 0.2",
        ".SAVE v(out)",
        ".END");

        //var netlistText = string.Join(Environment.NewLine,
        //               "Diode circuit",
        //               "D1 OUT 0 1N914",
        //               "V1 OUT 0 0",
        //               ".model 1N914 D(Is=2.52e-9 Rs=0.568 N=1.752 Cjo=4e-12 M=0.4 tt=20e-9)",
        //               ".DC V1 -1 1 10e-3",
        //               ".SAVE i(D1)",
        //               ".END");
        var netlistText = string.Join(Environment.NewLine,
               "Diode circuit",
               "V1 in 0 0 AC 1",
               "R1 in out 10k",
               "C1 out 0 1u",
               ".AC dec 5 10m 1k",
               ".SAVE i(C1)",
               ".END"
        );

        // Parse the SPICE netlist
        var parser = new SpiceNetlistParser();
        var parseResult = parser.ParseNetlist(netlistText);
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
        var export = spiceSharpModel.Exports.Find(e => e.Name == "i(C1)");
        simulation.ExportSimulationData += (sender, args) =>
        {
            Console.WriteLine(export?.Extract());
        };

        // Run the simulation
        simulation.Run(circuit);
    }
}
