using Maple.Application.Projects.Commands.CreateProject;
using Maple.Contracts.Projects;
using Maple.Infrastructure;
using Maple.Infrastructure.DbContext;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maple.API.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ProjectsController : ApiController
{
    private readonly SupabaseClient _supabaseClient;
    private readonly IMapper _mapper;
    private readonly ISender _mediator;

    public ProjectsController(SupabaseClient supabaseClient, IMapper mapper, ISender mediator)
    {
        _supabaseClient = supabaseClient;
        _mapper = mapper;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        var client = _supabaseClient.GetClient();
        var result = await client
            .From<ProjectModelContext>()
            .Get();
        return Ok(result.Models);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
    {
        var command = _mapper.Map<CreateProjectCommand>(request);
        var createProjectResult = await _mediator.Send(command);
        return createProjectResult.Match(
            res => Ok(_mapper.Map<ProjectResponse>(res)),
            Problem
        );
    }
}