using AuthenticationMicroservice.Domain.Models;

namespace AuthenticationMicroservice.Infrastructure.Services.Abstractions;

public interface ITokenGenerator
{
    string GenerateAccessToken(User user);

    string GenerateRefreshToken();
}