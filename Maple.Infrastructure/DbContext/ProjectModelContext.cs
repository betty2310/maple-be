using System.Text.Json.Nodes;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Maple.Infrastructure.DbContext;

[Table("projects")]
public class ProjectModelContext : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }
    
    [Column("content")]
    public JsonNode Content { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}