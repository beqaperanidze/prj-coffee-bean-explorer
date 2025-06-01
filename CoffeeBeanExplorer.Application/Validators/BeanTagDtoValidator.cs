using CoffeeBeanExplorer.Application.DTOs;
using FluentValidation;

namespace CoffeeBeanExplorer.Application.Validators
{
    public class CreateBeanTagDtoValidator : AbstractValidator<CreateBeanTagDto>
    {
        public CreateBeanTagDtoValidator()
        {
            RuleFor(x => x.BeanId).GreaterThan(0);
            RuleFor(x => x.TagId).GreaterThan(0);
        }
    }
}
