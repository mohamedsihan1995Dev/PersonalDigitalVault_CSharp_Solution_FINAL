namespace PersonalDigitalVault.Api.Storage;
public class FileStorageService
{
    private readonly string _root;
    public FileStorageService(IConfiguration config)
    {
        _root=Path.GetFullPath(config["Storage:RootPath"]??"../SecureStorage");
        Directory.CreateDirectory(_root);
    }
    public async Task<string> SaveAsync(string storedFileName,byte[] encrypted)
    {
        var path=Path.Combine(_root,storedFileName);
        await File.WriteAllBytesAsync(path,encrypted);
        return path;
    }
    public Task<byte[]> ReadAsync(string path)=>File.ReadAllBytesAsync(path); 
    public void Delete(string path){if(File.Exists(path))File.Delete(path);
    }
}
