using FluentValidation;
using AuthenticationMicroservice.Contracts.Requests;

namespace AuthenticationMicroservice.Api.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotNull().NotEmpty().MinimumLength(6);
    }
}
