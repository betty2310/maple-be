using System.Text;
using System.Text.Json;
using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Simulations;
using SpiceSharpParser;

namespace maple_test;

public abstract class Roles
{
    public const string Administrator = nameof(Administrator);
}

public class Program
{
    private const string V = "2";

    public static void JsonSerialization()
    {
        var voltage = new VoltageSource("V1", "in", "0", 1.0);
        var json = JsonSerializer.Serialize(voltage);
        Console.WriteLine(json);
    }

    public static void SpiceSharp()
    {
        var ckt = new Circuit(
            new VoltageSource("V1", "r1", "0", 1.0),
            new Resistor("R1", "r2", "r1", 1.0e4),
            new Resistor("R2", "0", "r2", 2.0e4)
        );

        var dc = new DC("DC 1", "V1", -1.0, 1.0, 0.2);

        // Create the simulation
        var tran = new Transient("Tran 1", 1e-3, 0.1);

        // Make the exports
        var inputExport = new RealVoltageExport(tran, "R1");
        var outputExport = new RealVoltageExport(tran, "R2");

        // Catch exported data
        var sb = new StringBuilder();
        tran.ExportSimulationData += (sender, args) =>
        {
            // var input = args.GetVoltage("R1");
            // var output = args.GetVoltage("R2");
            // sb.AppendLine($"V(in)={input} V(out)={output}");
            // Console.WriteLine($"V(in)={input} V(out)={output}");
            var input = inputExport.Value;
            var output = outputExport.Value;
            Console.WriteLine($"V(in)={input} V(out)={output}");
        };
        tran.Run(ckt);
    }

    public static void Main()
    {
        //Console.WriteLine(Roles.Administrator);
        // SpiceSharpParse();
        // SpiceSharp();
        // SpiceSharpParserDiode();
        // new MachChinhLuuNuaChuky().Execute();
        Console.WriteLine("=======================");
        // new MachChinhLuuNuaChuky().ExecuteWithSwitch();
        new MachTransistor().Execute();
    }

    private static void SpiceSharpParse()
    {
        var netlistString = string.Join(Environment.NewLine,
            "test",
            "V1	1 0 0",
            "V2 2 0 0",
            "Q1 2 1 0 0 mjd44h11",
            ".model mjd44h11 IS = 1.45468e-14 BF = 135.617 NF = 0.85 VAF = 10",
            ".END"
        );
        

        // Parse the SPICE netlist
        var parser = new SpiceNetlistParser();
        var parseResult = parser.ParseNetlist(netlistString);
        var spiceNetlist = parseResult.FinalModel;

        // Create a SpiceSharp reader and read the parsed model
        var reader = new SpiceSharpReader();
        var spiceSharpModel = reader.Read(spiceNetlist);

        // Get the circuit and simulation objects
        var circuit = spiceSharpModel.Circuit;

        var dc = new DC("DC 1", "V0", -1.0, 1.0, 0.2);
        var tran = new Transient("Tran 1", 1e-3, 0.1);

        var simulation = spiceSharpModel.Simulations.Single();

        // Output variable
        var outputList = new List<double>();


        // Catch exported data
        var export = spiceSharpModel.Exports.Find(e => e.Name == "i(R2)");

        dc.ExportSimulationData += (sender, args) =>
        {
            Console.WriteLine($"V: {args.GetVoltage(V)}");
            var current = args.GetSweepValues();
            Console.WriteLine($"Current: {current[0]}");
        };

        // Run the simulation
        dc.Run(circuit);
    }

    public static void SpiceSharpParserDiode()
    {
        var netlistText = string.Join(Environment.NewLine,
            "Diode circuit",
            "D1 OUT 0 1N914",
            "V1 OUT 0 0",
            ".model 1N914 D(Is=2.52e-9 Rs=0.568 N=1.752 Cjo=4e-12 M=0.4 tt=20e-9)",
            ".DC V1 -1 1 10e-3",
            ".SAVE i(V1) i(D1)",
            ".END");

        // Parsing part
        var parser = new SpiceNetlistParser();
        var parseResult = parser.ParseNetlist(netlistText);
        var netlist = parseResult.FinalModel;

        // Translating netlist model to SpiceSharp
        var reader = new SpiceSharpReader();
        var spiceSharpModel = reader.Read(netlist);

        var d1 = new Diode("D1", "OUT", "0", "1N914");
        var circuit = spiceSharpModel.Circuit;
        var diodeNode = circuit.ToList().First(circuit => circuit.Name.Contains('D'));
        circuit.Remove(diodeNode);
        circuit.Add(d1);
        // Simulation using SpiceSharp
        // var simulation = spiceSharpModel.Simulations.Single();
        var dc = new DC("DC 1", "V1", -1, 1, 10e-3);
        // var export = spiceSharpModel.Exports.Find(e => e.Name == "i(V1)");
        var currentExport = new RealPropertyExport(dc, "V1", "i");

        dc.ExportSimulationData += (sender, args) => Console.WriteLine(currentExport.Value);
        dc.Run(circuit);
    }
    
}