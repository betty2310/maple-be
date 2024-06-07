namespace Maple.Contracts.Projects;

public record ProjectResponse(int Id, string Name, Guid Uid, string Content, DateTime CreatedAt, DateTime UpdatedAt);