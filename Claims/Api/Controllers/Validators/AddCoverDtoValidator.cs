using Claims.Api.Controllers.Model;
using Claims.Application.Shared;
using JetBrains.Annotations;

namespace Claims.Api.Controllers.Validators;

[UsedImplicitly]
public class AddCoverDtoValidator : IValidator<AddCoverDto>
{
    public Task<ValidatorResult> Validate(AddCoverDto inputModel)
    {
        const int daysInYear = 365;
        var coverPeriod = inputModel.EndDate.DayNumber - inputModel.StartDate.DayNumber;
        var errors = new List<string>();
        if (inputModel.StartDate < DateOnly.FromDateTime(DateTime.Today))
        {
            errors.Add("StartDate cannot be in the past");
        }

        if (coverPeriod > daysInYear)
        {
            errors.Add("Total insurance period cannot exceed 1 year");
        }

        return Task.FromResult(new ValidatorResult(errors));
    }
}