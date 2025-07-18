using AuthenticationMicroservice.Domain.Models;
using AuthenticationMicroservice.Contracts.DTOs;

namespace AuthenticationMicroservice.Business.Service.Abstractions;

public interface IUsersService
{
    Task<User> Register(string email, string password);
    Task<AccessAndRefreshToken> Login(string email, string password);
    Task Logout(int userId);
    Task<string> RefreshAccessToken(string refreshToken);
    Task<string> GetNewRefreshToken(int id);
}
