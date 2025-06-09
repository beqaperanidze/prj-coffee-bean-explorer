using CoffeeBeanExplorer.Application.DTOs;
using FluentValidation;

namespace CoffeeBeanExplorer.Application.Validators;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required")
            .NotNull().WithMessage("Token cannot be null");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required")
            .NotNull().WithMessage("Refresh token cannot be null");
    }
}

public class RevokeTokenRequestValidator : AbstractValidator<RevokeTokenRequest>
{
    public RevokeTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required")
            .NotNull().WithMessage("Refresh token cannot be null");
    }
}