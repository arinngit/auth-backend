using FluentValidation;
using AuthenticationMicroservice.Contracts.Requests;

namespace AuthenticationMicroservice.Api.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotNull().NotEmpty().MinimumLength(6);
    }
}
