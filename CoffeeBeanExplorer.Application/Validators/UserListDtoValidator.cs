using CoffeeBeanExplorer.Application.DTOs;
using FluentValidation;

namespace CoffeeBeanExplorer.Application.Validators
{
    public class CreateUserListDtoValidator : AbstractValidator<CreateUserListDto>
    {
        public CreateUserListDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        }
    }

    public class UpdateUserListDtoValidator : AbstractValidator<UpdateUserListDto>
    {
        public UpdateUserListDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        }
    }
}
