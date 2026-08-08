//using Microsoft.EntityFrameworkCore;
//using PersonalDigitalVault.Api.Data;
//using PersonalDigitalVault.Api.Entities;
//using PersonalDigitalVault.Api.Interfaces.Repositories;

//namespace PersonalDigitalVault.Api.Repositories;

//public class UserRepository : IUserRepository
//{
//    private readonly AppDbContext _context;

//    public UserRepository(AppDbContext context)
//    {
//        _context = context;
//    }

//    public async Task<User?> GetByEmailAsync(string email)
//    {
//        return await _context.Users
//            .FirstOrDefaultAsync(x => x.Email == email);
//    }

//    public async Task<User?> GetByIdAsync(int id)
//    {
//        return await _context.Users
//            .FirstOrDefaultAsync(x => x.Id == id);
//    }

//    public async Task<List<User>> GetAllAsync()
//    {
//        return await _context.Users
//            .OrderByDescending(x => x.CreatedAt)
//            .ToListAsync();
//    }

//    public async Task<int> CountAsync()
//    {
//        return await _context.Users.CountAsync();
//    }

//    public async Task AddAsync(User user)
//    {
//        await _context.Users.AddAsync(user);
//    }

//    public async Task UpdateAsync(User user)
//    {
//        _context.Users.Update(user);

//        await _context.SaveChangesAsync();
//    }

//    public async Task SaveChangesAsync()
//    {
//        await _context.SaveChangesAsync();
//    }
//}


using Microsoft.EntityFrameworkCore;
using PersonalDigitalVault.Api.Data;
using PersonalDigitalVault.Api.Entities;
using PersonalDigitalVault.Api.Interfaces.Repositories;

namespace PersonalDigitalVault.Api.Repositories;

public class UserRepository(AppDbContext db)
    : IUserRepository
{
    // Email base panni user find pannum
    public Task<User?> GetByEmailAsync(string email)
    {
        return db.Users
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    // Id base panni user find pannum
    public Task<User?> GetByIdAsync(int id)
    {
        return db.Users
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    // Admin page-ku users ellam return pannum
    public Task<List<User>> GetAllAsync()
    {
        return db.Users
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    // Dashboard total users
    public Task<int> CountAsync()
    {
        return db.Users.CountAsync();
    }

    // Registration
    public async Task AddAsync(User user)
    {
        db.Users.Add(user);

        await db.SaveChangesAsync();
    }

    // User enable / disable update
    public async Task UpdateAsync(User user)
    {
        db.Users.Update(user);

        await db.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await db.SaveChangesAsync();
    }
}