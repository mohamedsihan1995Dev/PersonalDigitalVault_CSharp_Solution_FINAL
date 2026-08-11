using System.Security.Cryptography;
using System.Text;
namespace PersonalDigitalVault.Api.Security;
public class AesEncryption
{
    private readonly byte[] _key;
    public AesEncryption(IConfiguration config) { _key = Convert.FromBase64String(config["Security:AesKeyBase64"] ?? throw new InvalidOperationException("AES key missing")); if (_key.Length != 32) throw new InvalidOperationException("AES key must be 32 bytes."); }
    public string EncryptString(string value) => Convert.ToBase64String(EncryptBytes(Encoding.UTF8.GetBytes(value)));
    public string DecryptString(string value) => Encoding.UTF8.GetString(DecryptBytes(Convert.FromBase64String(value)));
    public byte[] EncryptBytes(byte[] plain) { using var aes = Aes.Create(); aes.Key = _key; aes.GenerateIV(); using var enc = aes.CreateEncryptor(); var cipher = enc.TransformFinalBlock(plain, 0, plain.Length); return aes.IV.Concat(cipher).ToArray(); }
    public byte[] DecryptBytes(byte[] encrypted) { using var aes = Aes.Create(); aes.Key = _key; var iv = encrypted[..16]; var cipher = encrypted[16..]; aes.IV = iv; using var dec = aes.CreateDecryptor(); return dec.TransformFinalBlock(cipher, 0, cipher.Length); }
}
