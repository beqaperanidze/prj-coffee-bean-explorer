using CoffeeBeanExplorer.Application.DTOs;
using FluentValidation;

namespace CoffeeBeanExplorer.Application.Validators;

public class CreateOriginDtoValidator : AbstractValidator<CreateOriginDto>
{
    public CreateOriginDtoValidator()
    {
        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(100);

        RuleFor(x => x.Region)
            .MaximumLength(100);
    }
}

public class UpdateOriginDtoValidator : AbstractValidator<UpdateOriginDto>
{
    public UpdateOriginDtoValidator()
    {
        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(100);

        RuleFor(x => x.Region)
            .MaximumLength(100);
    }
}