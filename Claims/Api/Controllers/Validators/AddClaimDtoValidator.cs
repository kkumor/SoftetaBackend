using Claims.Api.Controllers.Model;
using Claims.Application.Services;
using Claims.Application.Shared;
using JetBrains.Annotations;

namespace Claims.Api.Controllers.Validators;

[UsedImplicitly]
public class AddClaimDtoValidator(ICoversService coversService) : IValidator<AddClaimDto>
{
    public async Task<ValidatorResult> Validate(AddClaimDto inputModel)
    {
        const decimal maxDamageCost = 100000m;
        var errors = new List<string>();
        if (inputModel.DamageCost > maxDamageCost)
        {
            errors.Add($"DamageCost cannot exceed {maxDamageCost}");
        }

        var relatedCover = await coversService.GetCoverAsync(inputModel.CoverId);
        if (relatedCover == null)
        {
            errors.Add($"Invalid CoverId value {inputModel.CoverId}");
        }
        else
        {
            var beforeCoverStart = relatedCover.StartDate.ToDateTime(TimeOnly.MinValue) >= inputModel.Created;
            var afterCoverEnd = relatedCover.EndDate.ToDateTime(TimeOnly.MaxValue) <= inputModel.Created;
            if (beforeCoverStart || afterCoverEnd)
            {
                errors.Add("Created date must be within the period of the related Cover");
            }
        }

        return new ValidatorResult(errors);
    }
}