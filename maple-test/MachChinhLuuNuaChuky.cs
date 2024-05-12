using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Simulations;

namespace Maple;

public class MachChinhLuuNuaChuky
{
    public void Execute()
    {
        var voltageSource = new VoltageSource("V1", "1", "0", new Sine(0, 1, 50));
        voltageSource.SetParameter("acmag", 1.0);
        var diode = new Diode("D1", "1", "2", "1N914");
        var resistor = new Resistor("R1", "0", "2", 1000);
        var diodeModel = Create1N914Model();

        var ckt = new Circuit(voltageSource, resistor, diode, diodeModel);
        var tran = new Transient("tran", 1e-3, 0.1);

        var voltateOutputExport = new RealVoltageExport(tran, "2");
        tran.ExportSimulationData += (sender, args) =>
        {
            var output = voltateOutputExport.Value;
            Console.WriteLine($"V(out)={output}");
        };
        tran.Run(ckt);
    }
    
    public void ExecuteWithSwitch()
    {
        var voltageSource = new VoltageSource("V1", "1", "0", new Sine(0, 1, 50));
        voltageSource.SetParameter("acmag", 1.0);
        var switchNode = new Resistor("RS1", "2", "1", 1000000000);
        var diode = new Diode("D1", "3", "2", "1N914");
        var resistor = new Resistor("R1", "0", "3", 1000);
        var diodeModel = Create1N914Model();

        var ckt = new Circuit(voltageSource, resistor, diode, diodeModel, switchNode);
        var tran = new Transient("tran", 1e-3, 0.1);

        var voltateOutputExport = new RealVoltageExport(tran, "3");
        tran.ExportSimulationData += (sender, args) =>
        {
            var output = voltateOutputExport.Value;
            Console.WriteLine($"V(out)={output}");
        };
        tran.Run(ckt);
    }

    private static DiodeModel Create1N914Model()
    {
        var diodeModel = new DiodeModel("1N914");
        diodeModel.SetParameter("is", 1e-14);
        diodeModel.SetParameter("rs", 0.0);
        diodeModel.SetParameter("n", 1.0);
        diodeModel.SetParameter("bv", 1e+30);
        diodeModel.SetParameter("tt", 0.0);
        diodeModel.SetParameter("cjo", 0.0);
        diodeModel.SetParameter("vj", 1.0);
        diodeModel.SetParameter("m", 0.5);
        diodeModel.SetParameter("eg", 1.11);
        diodeModel.SetParameter("xti", 3.0);
        diodeModel.SetParameter("kf", 0.0);
        diodeModel.SetParameter("af", 1.0);
        diodeModel.SetParameter("fc", 0.5);
        diodeModel.SetParameter("ibv", 1e-10);
        return diodeModel;
    }
}