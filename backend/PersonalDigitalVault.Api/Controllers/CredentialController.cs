using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalDigitalVault.Api.DTOs.Credential;
using PersonalDigitalVault.Api.Interfaces.Services;

namespace PersonalDigitalVault.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/credentials")]
public class CredentialController(ICredentialService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCredentialDto dto) => Ok(await service.CreateAsync(dto));

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) => Ok(await service.GetAsync(id));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateCredentialDto dto) => Ok(await service.UpdateAsync(id, dto));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { await service.DeleteAsync(id); return NoContent(); }
}
