using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthenticationMicroservice.Domain.Models;
using AuthenticationMicroservice.Infrastructure.Services.Abstractions;
using AuthenticationMicroservice.Infrastructure.Services.Options;
using Microsoft.Extensions.Options;

namespace AuthenticationMicroservice.Infrastructure.Services.Concrete;

public class TokenGenerator : ITokenGenerator
{
    private readonly JwtOptions _options;

    public TokenGenerator(IOptions<JwtOptions> jwtOptions)
    {
        _options = jwtOptions.Value;
    }

    public string GenerateAccessToken(User user)
    {
        List<Claim> claims = new List<Claim>()
        {
            new Claim("id", user.Id.ToString()),
            new Claim("email", user.Email),
            new Claim("role", user.RoleId.ToString()),
            new Claim("iat", ToUnixEpochDate(DateTime.Now).ToString(), ClaimValueTypes.Integer64)
        };

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims,
            expires: DateTime.Now + _options.AccessValidFor,
            signingCredentials: _options.SigningCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }

    private static long ToUnixEpochDate(DateTime date)
    {
        DateTimeOffset offset = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        return (long)Math.Round((date.ToUniversalTime() - offset).TotalSeconds);
    }
}
