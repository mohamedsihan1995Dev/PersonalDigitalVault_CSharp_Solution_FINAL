
namespace PersonalDigitalVault.Api.Entities;

public class Folder
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<Document> Documents { get; set; } = new List<Document>();
}