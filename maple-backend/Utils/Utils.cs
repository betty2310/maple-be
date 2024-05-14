using System.Text.RegularExpressions;
using SpiceSharp.Entities;

namespace maple_backend.Utils;

public partial class Utils
{
    /// <summary>
    /// Apply a parameter definition to an entity
    /// Parameters are a series of assignments [name]=[value] delimited by spaces.
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="definition">Definition string</param>
    public static void ApplyParameters(Entity entity, string definition)
    {
        // Get all assignments
        definition = MyRegex().Replace(definition, "=");
        var assignments = definition.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var assignment in assignments)
        {
            // Get the name and value
            var parts = assignment.Split('=');
            if (parts.Length != 2)
                throw new Exception("Invalid assignment");
            var name = parts[0].ToLower();
            var value = double.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);

            // Set the entity parameter
            entity.SetParameter(name, value);
        }
    }

    [GeneratedRegex(@"\s*\=\s*")]
    private static partial Regex MyRegex();
}