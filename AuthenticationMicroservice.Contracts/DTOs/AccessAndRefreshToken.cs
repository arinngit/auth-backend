namespace AuthenticationMicroservice.Contracts.DTOs;

public class AccessAndRefreshToken
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

