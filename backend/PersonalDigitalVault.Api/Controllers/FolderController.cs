using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc; 
using PersonalDigitalVault.Api.DTOs.Folder; 
using PersonalDigitalVault.Api.Interfaces.Services;

namespace PersonalDigitalVault.Api.Controllers;
[ApiController]
[Authorize]
[Route("api/folders")]
public class FolderController(IFolderService service):ControllerBase

{ [HttpGet]
    public async Task<IActionResult> Get()=>Ok(await service.GetAllAsync());
    [HttpPost] 
    public async Task<IActionResult> Create(CreateFolderDto dto)=>Ok(await service.CreateAsync(dto)); 
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id,UpdateFolderDto dto)=>Ok(await service.UpdateAsync(id,dto));
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id){await service.DeleteAsync(id);return NoContent();} }
