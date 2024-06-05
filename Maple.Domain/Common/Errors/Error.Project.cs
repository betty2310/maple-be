using ErrorOr;

namespace Maple.Domain.Common.Errors;

public static partial class Errors
{
    public static class Project
    {
        public static Error DuplicatedProject =>
            Error.Validation(code: "NoExportNodesProvided", description: "No export nodes provided");
    }
}