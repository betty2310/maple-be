namespace Maple.Infrastructure.Common.DiodeModels;

public interface IDiodeModel
{
    public SpiceSharp.Components.DiodeModel GetModel();
}