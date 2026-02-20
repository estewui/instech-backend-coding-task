using FluentValidation;

using Application.Abstractions.Persistence;
using Domain.Entities;

namespace Application.Claims.CreateClaim
{
    public class CreateClaimValidator : AbstractValidator<Claim>
    {
        public CreateClaimValidator(ICoverRepository covers)
        {
            RuleFor(x => x)
                .MustAsync(async (claim, ct) =>
                {
                    var coverDb = await covers.GetById(claim.CoverId);
                    if (coverDb == default)
                        return false;

                    return coverDb.StartDate <= claim.Created && coverDb.EndDate >= claim.Created;

                })
                .WithMessage("Created date must be within the period of the related Cover.");


            RuleFor(x => x.DamageCost)
                .InclusiveBetween(0, 100000)
                .WithMessage("DamageCost cannot exceed 100.000.");
        }
    }

}
