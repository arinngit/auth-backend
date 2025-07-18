namespace AuthenticationMicroservice.Contracts.Requests;

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
