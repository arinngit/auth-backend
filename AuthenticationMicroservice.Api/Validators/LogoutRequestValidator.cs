using FluentValidation;
using AuthenticationMicroservice.Contracts.Requests;

namespace AuthenticationMicroservice.Api.Validators;

public class LogoutRequestValidator : AbstractValidator<LogoutRequest>{
    public LogoutRequestValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}
