namespace AuthenticationMicroservice.Domain.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public int UserId { get; set; }

    public RefreshToken()
    {
        Token = string.Empty;
    }

    public RefreshToken(int id, string token, DateTime expriresAt, int userId)
    {
        Id = id;
        Token = token;
        ExpiresAt = expriresAt;
        UserId = userId;
    }
}
