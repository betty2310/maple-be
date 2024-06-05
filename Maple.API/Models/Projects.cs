using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Maple.API.Models;

[Table("projects")]
public class Projects : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}