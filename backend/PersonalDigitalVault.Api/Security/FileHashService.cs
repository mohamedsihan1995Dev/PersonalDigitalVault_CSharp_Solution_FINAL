using System.Security.Cryptography;
namespace PersonalDigitalVault.Api.Security;
public class FileHashService 
{
    public string Sha256(byte[] data)=>Convert.ToHexString(SHA256.HashData(data));
}
