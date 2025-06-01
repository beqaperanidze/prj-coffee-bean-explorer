using CoffeeBeanExplorer.Application.DTOs;
using FluentValidation;

namespace CoffeeBeanExplorer.Application.Validators
{
    public class CreateListItemDtoValidator : AbstractValidator<CreateListItemDto>
    {
        public CreateListItemDtoValidator()
        {
            RuleFor(x => x.ListId).GreaterThan(0);
            RuleFor(x => x.BeanId).GreaterThan(0);
        }
    }
}
