using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc; 
using PersonalDigitalVault.Api.DTOs.Document; 
using PersonalDigitalVault.Api.Interfaces.Services;

namespace PersonalDigitalVault.Api.Controllers;
[ApiController]
[Authorize]
[Route("api/documents")] 
public class DocumentController(IDocumentService service):ControllerBase
{ [HttpPost("upload")]
    [RequestSizeLimit(10*1024*1024)]
    public async Task<IActionResult> Upload([FromForm]UploadDocumentDto dto)=>Ok(await service.UploadAsync(dto)); 
    [HttpGet]
    public async Task<IActionResult> GetAll()=>Ok(await service.GetAllAsync());
    [HttpGet("{id:int}")] 
    public async Task<IActionResult> Get(int id)=>Ok(await service.GetAsync(id)); 
    [HttpGet("{id:int}/download")]
    public async Task<IActionResult> Download(int id){var f=await service.DownloadAsync(id);
        return File(f.Data,f.ContentType,f.FileName);}
    [HttpPut("{id:int}")] 
    public async Task<IActionResult> Update(int id,UpdateDocumentDto dto)=>Ok(await service.UpdateAsync(id,dto)); 
    [HttpDelete("{id:int}")] 
    public async Task<IActionResult> Delete(int id){await service.DeleteAsync(id);return NoContent();} }
