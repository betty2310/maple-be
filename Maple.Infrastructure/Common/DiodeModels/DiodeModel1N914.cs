namespace Maple.Infrastructure.Common.DiodeModels;

public class DiodeModel1N914 : IDiodeModel{
    public SpiceSharp.Components.DiodeModel GetModel()
    {
        var diodeModel = new SpiceSharp.Components.DiodeModel("1N914");
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