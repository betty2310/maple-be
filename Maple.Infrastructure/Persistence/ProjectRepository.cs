using Maple.Application.Common.Interfaces.Persistence;
using Maple.Domain.Entities;

namespace Maple.Infrastructure.Persistence;

public class ProjectRepository : IProjectRepository
{
    public List<Project>? GetProjects()
    {
        throw new NotImplementedException();
    }
}