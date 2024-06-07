using Maple.Application.Common.Interfaces;
using Maple.Domain.Entities;
using Maple.Infrastructure.DbContext;
using Supabase.Postgrest;

namespace Maple.Infrastructure.Persistence;

public class ProjectRepository : IProjectRepository
{
    private readonly SupabaseClient _supabaseClient;

    public ProjectRepository(SupabaseClient supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public List<Project>? GetProjects()
    {
        throw new NotImplementedException();
    }

    public async Task<Project> AddProject(Project project)
    {
        var projectContext = new ProjectModelContext
        {
            Name = project.Name,
            Content = project.Content ?? string.Empty
        };
        var res = await _supabaseClient.GetClient().From<ProjectModelContext>().Insert(projectContext,
            new QueryOptions { Returning = QueryOptions.ReturnType.Representation });
        if (res.Models.Count == 0)
            throw new Exception("Failed to create project");
        return new Project
        {
            Id = res.Models.First().Id,
            Name = res.Models.First().Name,
            Content = res.Models.First().Content,
            CreatedAt = res.Models.First().CreatedAt,
            UpdatedAt = res.Models.First().UpdatedAt
        };
    }
}