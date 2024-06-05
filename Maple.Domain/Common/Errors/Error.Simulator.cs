using ErrorOr;

namespace Maple.Domain.Common.Errors;

public static partial class Errors
{
    public static class Simulator
    {
        public static Error NoExportNodesProvided =>
            Error.Failure(code: "NoExportNodesProvided", description: "No export nodes provided");
        
        public static Error MustHaveVoltageSource =>
            Error.Failure(code: "MustHaveVoltageSource", description: "The circuit must have a voltage source");
    }
}