using System.Security.Cryptography;
using System.Text;

namespace AuthenticationMicroservice.Infrastructure.Services.Concrete;

public static class Hasher
{
    public static Task<string> HashAsync(string data)
    {
        SHA256 hash = SHA256.Create();
        byte[] hashedPasswordBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Task.FromResult(Convert.ToBase64String(hashedPasswordBytes));
    }
}