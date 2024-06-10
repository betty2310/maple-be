using Maple.Application.Projects.Commands.CreateProject;
using Maple.Contracts.Projects;
using Maple.Domain.Entities;
using Mapster;

namespace Maple.API.Common.Mapping;

public class ProjectMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateProjectRequest, CreateProjectCommand>();
        config.NewConfig<Project, ProjectResponse>();
    }
}