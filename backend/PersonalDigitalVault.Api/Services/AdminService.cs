//using PersonalDigitalVault.Api.DTOs.Admin; 
//using PersonalDigitalVault.Api.Interfaces.Repositories; 
//using PersonalDigitalVault.Api.Interfaces.Services;
//namespace PersonalDigitalVault.Api.Services;
//public class AdminService(IUserRepository users,IDocumentRepository documents) : IAdminService
//{
//    public async Task<AdminDashboardDto> GetDashboardAsync()
//    {
//        var userCount=await users.CountAsync();var docCount=await documents.CountAsync();return new AdminDashboardDto
//        {
//            TotalUsers=userCount,TotalUploads=docCount,TotalStoredFiles=docCount};
//    } 

//    public async Task<List<UserListDto>> GetUsersAsync()=>(await users.GetAllAsync()).Select(x=>new UserListDto
//    {
//        Id=x.Id,FullName=x.FullName,Email=x.Email,Role=x.Role,IsActive=x.IsActive }).ToList();

//    public async Task UpdateUserStatusAsync(int id,UpdateUserStatusDto dto)
//    {
//        var u=await users.GetByIdAsync(id)??throw new KeyNotFoundException("User not found.");u.IsActive=dto.IsActive;await users.UpdateAsync(u);
//    }
//}


//using PersonalDigitalVault.Api.DTOs.Admin;
//using PersonalDigitalVault.Api.Interfaces.Repositories;
//using PersonalDigitalVault.Api.Interfaces.Services;

//namespace PersonalDigitalVault.Api.Services;

//public class AdminService(
//    IUserRepository users,
//    IDocumentRepository documents)
//    : IAdminService
//{
//    public async Task<AdminDashboardDto> GetDashboardAsync()
//    {
//        var userCount = await users.CountAsync();

//        var documentCount = await documents.CountAsync();

//        return new AdminDashboardDto
//        {
//            TotalUsers = userCount,
//            TotalUploads = documentCount,
//            TotalStoredFiles = documentCount
//        };
//    }

//    public async Task<List<UserListDto>> GetUsersAsync()
//    {
//        var userList = await users.GetAllAsync();

//        return userList
//            .Select(x => new UserListDto
//            {
//                Id = x.Id,
//                FullName = x.FullName,
//                Email = x.Email,
//                Role = x.Role,
//                IsActive = x.IsActive
//            })
//            .ToList();
//    }

//    public async Task UpdateUserStatusAsync(
//        int id,
//        UpdateUserStatusDto dto)
//    {
//        var user = await users.GetByIdAsync(id);

//        if (user == null)
//        {
//            throw new KeyNotFoundException("User not found.");
//        }

//        user.IsActive = dto.IsActive;

//        await users.UpdateAsync(user);
//    }
//}
using PersonalDigitalVault.Api.DTOs.Admin;
using PersonalDigitalVault.Api.Interfaces.Repositories;
using PersonalDigitalVault.Api.Interfaces.Services;

namespace PersonalDigitalVault.Api.Services;

public class AdminService(
    IUserRepository users,
    IDocumentRepository documents)
    : IAdminService
{
    // =====================================================
    // ADMIN DASHBOARD
    // =====================================================
    public async Task<AdminDashboardDto> GetDashboardAsync()
    {
        // Total users
        var totalUsers =
            await users.CountAsync();

        // Total uploaded/stored documents
        var totalDocuments =
            await documents.CountAsync();

        // Upload activity list
        var uploadedDocuments =
            await documents.GetAllForAdminAsync();


        // Document entity-lendhu
        // safe Admin DTO-ku map pannrom.
        //
        // StoragePath
        // FileHash
        // StoredFileName
        // File content
        //
        // edhuvum Admin-ku return panna maatom.
        var uploads =
            uploadedDocuments
                .Select(document =>
                    new AdminUploadDto
                    {
                        UploadedBy =
                            document.User?.FullName
                            ?? "Unknown User",

                        FileName =
                            document.OriginalFileName,

                        FileSize =
                            document.FileSize,

                        UploadedAt =
                            document.CreatedAt
                    })
                .ToList();


        return new AdminDashboardDto
        {
            TotalUsers =
                totalUsers,

            // Current schema-la upload history table illa.
            // So document records count use pannrom.
            TotalUploads =
                totalDocuments,

            TotalStoredFiles =
                totalDocuments,

            RecentUploads =
                uploads
        };
    }


    // =====================================================
    // USER ACCOUNT LIST
    // =====================================================
    public async Task<List<UserListDto>> GetUsersAsync()
    {
        var userList =
            await users.GetAllAsync();

        return userList
            .Select(user =>
                new UserListDto
                {
                    Id =
                        user.Id,

                    FullName =
                        user.FullName,

                    Email =
                        user.Email,

                    Role =
                        user.Role,

                    IsActive =
                        user.IsActive
                })
            .ToList();
    }


    // =====================================================
    // ENABLE / DISABLE USER
    // =====================================================
    public async Task UpdateUserStatusAsync(
        int id,
        UpdateUserStatusDto dto)
    {
        var user =
            await users.GetByIdAsync(id);

        if (user == null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }


        // Admin account-a another Admin disable panna koodathu.
        if (user.Role.Equals(
            "Admin",
            StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Admin account status cannot be changed.");
        }


        // true  -> Enable
        // false -> Disable
        user.IsActive =
            dto.IsActive;


        await users.UpdateAsync(user);
    }
}