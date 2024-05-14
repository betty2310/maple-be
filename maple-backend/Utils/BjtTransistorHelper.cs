using SpiceSharp.Components;

namespace maple_backend.Utils;

public class BjtTransistorHelper
{
    public static List<BipolarJunctionTransistor> GetTransistorsFromNetlist(string netlist)
    {
        var transistors = new List<BipolarJunctionTransistor>();
        var lines = netlist.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        var linesStartingWithQ = lines.Where(line => line.StartsWith("Q"));
        foreach (var line in linesStartingWithQ)
        {
            var tokens = line.Split(" ");
            var name = tokens[0];
            var c = tokens[1];
            var b = tokens[2];
            var e = tokens[3];
            var subst = tokens[4];
            var model = tokens[5];
            var bjt = CreateBjt(name, c, b, e, subst, model);
            transistors.Add(bjt);
        }

        return transistors;
    }
    
    public static BipolarJunctionTransistor CreateBjt(string name,
        string c, string b, string e, string subst,
        string model)
    {
        // Create the transistor
        var bjt = new BipolarJunctionTransistor(name, c, b, e, subst, model);
        return bjt;
    }

    public static BipolarJunctionTransistorModel CreateBjtModel(string name, string parameters)
    {
        var bjtmodel = new BipolarJunctionTransistorModel(name);
        Utils.ApplyParameters(bjtmodel, parameters);
        return bjtmodel;
    }

    public static BipolarJunctionTransistorModel NPNmjd44h11Transistor()
    {
        var model = CreateBjtModel("mjd44h11", string.Join(" ",
            "IS = 1.45468e-14 BF = 135.617 NF = 0.85 VAF = 10",
            "IKF = 5.15565 ISE = 2.02483e-13 NE = 3.99964 BR = 13.5617",
            "NR = 0.847424 VAR = 100 IKR = 8.44427 ISC = 1.86663e-13",
            "NC = 1.00046 RB = 1.35729 IRB = 0.1 RBM = 0.1",
            "RE = 0.0001 RC = 0.037687 XTB = 0.90331 XTI = 1",
            "EG = 1.20459 CJE = 3.02297e-09 VJE = 0.649408 MJE = 0.351062",
            "TF = 2.93022e-09 XTF = 1.5 VTF = 1.00001 ITF = 0.999997",
            "CJC = 3.0004e-10 VJC = 0.600008 MJC = 0.409966 XCJC = 0.8",
            "FC = 0.533878 CJS = 0 VJS = 0.75 MJS = 0.5",
            "TR = 2.73328e-08 PTF = 0 KF = 0 AF = 1"));
        return model;
    }
}