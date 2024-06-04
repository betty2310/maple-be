using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace maple_backend;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class Projects : ControllerBase
{
    private readonly SupabaseClient _supabaseClient;

    public Projects(SupabaseClient supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        var client = _supabaseClient.GetClient();
        var result = await client
            .From<Models.Projects>()
            .Get();
        return Ok(result.Models);
    }
}