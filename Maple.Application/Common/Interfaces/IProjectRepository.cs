using Maple.Domain.Entities;

namespace Maple.Application.Common.Interfaces;

public interface IProjectRepository
{
    List<Project>? GetProjects();
    public Task<Project> AddProject(Project project);
}