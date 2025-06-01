using CoffeeBeanExplorer.Application.DTOs;
using FluentValidation;

namespace CoffeeBeanExplorer.Application.Validators
{
    public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewDtoValidator()
        {
            RuleFor(x => x.BeanId).GreaterThan(0);
            RuleFor(x => x.Rating).InclusiveBetween(1, 5);
            RuleFor(x => x.Comment).MaximumLength(500);
        }
    }

    public class UpdateReviewDtoValidator : AbstractValidator<UpdateReviewDto>
    {
        public UpdateReviewDtoValidator()
        {
            RuleFor(x => x.Rating).InclusiveBetween(1, 5);
            RuleFor(x => x.Comment).MaximumLength(500);
        }
    }
}
