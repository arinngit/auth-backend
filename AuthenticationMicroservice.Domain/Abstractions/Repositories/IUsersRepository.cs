using AuthenticationMicroservice.Domain.Models;

namespace AuthenticationMicroservice.Domain.Abstractions.Repositories;

public interface IUsersRepository
{
    Task<User> GetByIdAsync(int id);
    Task<User> GetByEmailAsync(string email);
    Task<User> AddAsync(string email, string password, string salt, int roleId);
    Task<RefreshToken> AddRefreshTokenAsync(RefreshToken refreshToken);
    Task<RefreshToken> GetRefreshTokenByToken(string token);
    Task<bool> RemoveOldRefreshToken(string token);
    Task<bool> RemoveUsersRefreshTokens(int userId);
}
