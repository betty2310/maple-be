using System.Text.Json.Nodes;

namespace Maple.Domain.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid Uid { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}