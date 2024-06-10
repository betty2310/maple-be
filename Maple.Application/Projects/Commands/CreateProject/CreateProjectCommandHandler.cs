using ErrorOr;
using Maple.Application.Common.Interfaces;
using Maple.Domain.Entities;
using MediatR;

namespace Maple.Application.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ErrorOr<Project>>
{
    private readonly IProjectRepository _projectRepository;

    public CreateProjectCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ErrorOr<Project>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new Project
        {
            Name = request.Name,
            Content = request.Content
        };
        var res = await _projectRepository.AddProject(project);
        return res!;
    }
}