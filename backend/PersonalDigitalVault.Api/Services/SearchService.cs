using PersonalDigitalVault.Api.DTOs.Search; using PersonalDigitalVault.Api.Interfaces.Repositories; using PersonalDigitalVault.Api.Interfaces.Services; using PersonalDigitalVault.Api.Security;
namespace PersonalDigitalVault.Api.Services;
public class SearchService(IFolderRepository folders,IDocumentRepository documents,ICredentialRepository credentials,CurrentUserService current) : ISearchService
{
    public async Task<List<SearchResultDto>> SearchAsync(string keyword)
    {
        keyword=(keyword??string.Empty).Trim();var results=new List<SearchResultDto>();
        results.AddRange((await folders.GetByUserAsync(current.UserId)).Where(x=>x.Name.Contains(keyword,StringComparison.OrdinalIgnoreCase)).Select(x=>new SearchResultDto{Type="Folder",Id=x.Id,Title=x.Name}));results.AddRange((await documents.GetByUserAsync(current.UserId)).Where(x=>x.OriginalFileName.Contains(keyword,StringComparison.OrdinalIgnoreCase)).Select(x=>new SearchResultDto{Type="Document",Id=x.Id,Title=x.OriginalFileName}));
        results.AddRange((await credentials.GetByUserAsync(current.UserId)).Where(x=>x.Title.Contains(keyword,StringComparison.OrdinalIgnoreCase)).Select(x=>new SearchResultDto{Type="Credential",Id=x.Id,Title=x.Title}));
        return results;
    }
}
