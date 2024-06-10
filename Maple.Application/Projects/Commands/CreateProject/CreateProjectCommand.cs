using ErrorOr;
using Maple.Domain.Entities;
using MediatR;

namespace Maple.Application.Projects.Commands.CreateProject;

public record CreateProjectCommand(string Name, string? Content) : IRequest<ErrorOr<Project>>;