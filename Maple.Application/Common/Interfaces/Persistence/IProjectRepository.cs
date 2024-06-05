using Maple.Domain.Entities;

namespace Maple.Application.Common.Interfaces.Persistence;

public interface IProjectRepository
{
    List<Project>? GetProjects();
}