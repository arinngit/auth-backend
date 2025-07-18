namespace AuthenticationMicroservice.Domain.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
    public int RoleId { get; set; }
    public bool IsEmailConfirmed { get; set; } = false;
    public string ProfileImageUrl { get; set; }

    public User()
    {
        Email = string.Empty;
        PasswordHash = string.Empty;
        Salt = string.Empty;
        ProfileImageUrl = string.Empty;
    }

    public User(
            int id,
            string email,
            string nickname,
            string passwordHash,
            string salt,
            int roleId,
            bool isEmailConfirmed,
            string profileImageUrl
        )
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        Salt = salt;
        RoleId = roleId;
        IsEmailConfirmed = isEmailConfirmed;
        ProfileImageUrl = profileImageUrl;
    }
}
