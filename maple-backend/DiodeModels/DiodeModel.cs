namespace maple_backend.DiodeModels;

public interface DiodeModel
{
    public SpiceSharp.Components.DiodeModel GetModel();
}