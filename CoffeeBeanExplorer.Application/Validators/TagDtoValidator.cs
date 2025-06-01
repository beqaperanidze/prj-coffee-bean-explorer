using CoffeeBeanExplorer.Application.DTOs;
using FluentValidation;

namespace CoffeeBeanExplorer.Application.Validators
{
    public class CreateTagDtoValidator : AbstractValidator<CreateTagDto>
    {
        public CreateTagDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        }
    }

    public class UpdateTagDtoValidator : AbstractValidator<UpdateTagDto>
    {
        public UpdateTagDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        }
    }
}
