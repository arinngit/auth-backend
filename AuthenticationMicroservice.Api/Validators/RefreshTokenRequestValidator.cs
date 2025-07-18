using FluentValidation;
using AuthenticationMicroservice.Contracts.Requests;

namespace AuthenticationMicroservice.Api.Validators;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotNull().NotEmpty();
    } 
}
