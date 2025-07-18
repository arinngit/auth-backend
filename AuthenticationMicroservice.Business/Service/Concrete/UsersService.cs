using AuthenticationMicroservice.Business.Service.Abstractions;
using AuthenticationMicroservice.Domain.Abstractions.Repositories;
using AuthenticationMicroservice.Domain.Models;
using AuthenticationMicroservice.Infrastructure.Services.Abstractions;
using AuthenticationMicroservice.Infrastructure.Services.Concrete;
using AuthenticationMicroservice.Contracts.DTOs;
using Serilog;

namespace AuthenticationMicroservice.Business.Service.Concrete;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;
    private readonly ITokenGenerator _tokenGenerator;

    public UsersService(IUsersRepository usersRepository, ITokenGenerator tokenGenerator)
    {
        _usersRepository = usersRepository;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AccessAndRefreshToken> Login(string email, string password)
    {
        User user = await _usersRepository.GetByEmailAsync(email);

        if (user.Id == 0)
            return new AccessAndRefreshToken();

        if (await Hasher.HashAsync($"{password}{user.Salt}") == user.PasswordHash)
        {
            string refreshToken = _tokenGenerator.GenerateRefreshToken();

            await _usersRepository.AddRefreshTokenAsync(
                new RefreshToken
                {
                    Token = refreshToken,
                    ExpiresAt = DateTime.UtcNow + TimeSpan.FromDays(3),
                    UserId = user.Id,
                }
            );

            return new AccessAndRefreshToken
            {
                AccessToken = _tokenGenerator.GenerateAccessToken(user),
                RefreshToken = refreshToken
            };
        }

        return new AccessAndRefreshToken();
    }

    public async Task<User> Register(string email, string password)
    {
        string salt = Guid.NewGuid().ToString();

        string hashedPassword = await Hasher.HashAsync($"{password}{salt}");

        User user = await _usersRepository.AddAsync(email, hashedPassword, salt, 1);

        return user;
    }

    public async Task<string> RefreshAccessToken(string token)
    {
        RefreshToken refreshToken = await _usersRepository.GetRefreshTokenByToken(token);

        if (refreshToken.ExpiresAt < DateTime.Now)
        {
            return string.Empty;
        }

        User user = await _usersRepository.GetByIdAsync(refreshToken.UserId);

        if (user.Id == 0)
        {
            return string.Empty;
        }

        return _tokenGenerator.GenerateAccessToken(user);
    }

    public async Task Logout(int userId)
    {
        await _usersRepository.RemoveUsersRefreshTokens(userId);

        Log.Information($"User With Id {userId} Was Logout");
    }

    public async Task<string> GetNewRefreshToken(int id)
    {
        bool result = await _usersRepository.RemoveUsersRefreshTokens(id);

        if (!result)
        {
            Log.Information("Something Went Wrong At Deleting Users All Old Refresh Tokens");

            return string.Empty;
        }

        string refreshToken = _tokenGenerator.GenerateRefreshToken();

        Log.Information($"New Refresh Token Is {refreshToken}");

        return refreshToken;
    }
}
