using FluentValidation;

using Domain.Entities;


namespace Application.Covers.CreateCover
{
    public class CreateCoverValidator: AbstractValidator<Cover>
    {
        public CreateCoverValidator()
        {
            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate)
                .WithMessage("Start date has to be before End date.");

            RuleFor(x => x.StartDate)
                .GreaterThan(x => DateTime.UtcNow)
                .WithMessage("Start date cannot be in the past.");

            RuleFor(x => x)
                .Must(x => x.EndDate.Subtract(x.StartDate).TotalDays <= 365)
                .WithMessage("Total insurance period cannot exceed 1 year");
        }
    }
}
