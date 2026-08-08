//using Microsoft.EntityFrameworkCore;
//using PersonalDigitalVault.Api.Entities;
//namespace PersonalDigitalVault.Api.Data;
//public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
//{
//    public DbSet<User> Users => Set<User>(); public DbSet<Folder> Folders => Set<Folder>(); public DbSet<Document> Documents => Set<Document>(); public DbSet<Credential> Credentials => Set<Credential>();
//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
//        modelBuilder.Entity<Folder>().HasOne(x => x.User).WithMany(x => x.Folders).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
//        modelBuilder.Entity<Document>().HasOne(x => x.User).WithMany(x => x.Documents).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
//        modelBuilder.Entity<Document>().HasOne(x => x.Folder).WithMany(x => x.Documents).HasForeignKey(x => x.FolderId).OnDelete(DeleteBehavior.SetNull);
//        modelBuilder.Entity<Credential>().HasOne(x => x.User).WithMany(x => x.Credentials).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
//    }
//}


using Microsoft.EntityFrameworkCore;
using PersonalDigitalVault.Api.Entities;

namespace PersonalDigitalVault.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Folder> Folders => Set<Folder>();

    public DbSet<Document> Documents => Set<Document>();

    public DbSet<Credential> Credentials => Set<Credential>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Email must be unique
        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();

        // User -> Folders
        // No automatic cascade delete.
        modelBuilder.Entity<Folder>()
            .HasOne(x => x.User)
            .WithMany(x => x.Folders)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // User -> Documents
        // No automatic cascade delete.
        modelBuilder.Entity<Document>()
            .HasOne(x => x.User)
            .WithMany(x => x.Documents)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // Folder -> Documents
        // Folder delete pannina document delete aaga koodathu.
        // FolderId mattum NULL aagum.
        modelBuilder.Entity<Document>()
            .HasOne(x => x.Folder)
            .WithMany(x => x.Documents)
            .HasForeignKey(x => x.FolderId)
            .OnDelete(DeleteBehavior.SetNull);

        // User -> Credentials
        // User hard delete namma system-la use panna maatom.
        modelBuilder.Entity<Credential>()
            .HasOne(x => x.User)
            .WithMany(x => x.Credentials)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}