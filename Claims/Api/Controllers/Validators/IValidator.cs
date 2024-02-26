namespace Claims.Api.Controllers.Validators;

public interface IValidator<in T>
{
    Task<ValidatorResult> Validate(T inputModel);
}