using Claims.Api.Controllers.Validators;

namespace Claims.Application.Shared;

public interface IValidator<in T>
{
    Task<ValidatorResult> Validate(T inputModel);
}