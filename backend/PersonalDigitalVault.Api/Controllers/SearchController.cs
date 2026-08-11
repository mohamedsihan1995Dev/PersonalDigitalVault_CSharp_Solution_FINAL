using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalDigitalVault.Api.Interfaces.Services;

namespace PersonalDigitalVault.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/search")]
public class SearchController(ISearchService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string keyword = "") => Ok(await service.SearchAsync(keyword));
}
