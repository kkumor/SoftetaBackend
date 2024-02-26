namespace Claims.Api.Controllers.Validators;

public class ValidatorResult(ICollection<string> errors)
{
    public bool IsValid => Errors.Count == 0;
    public ICollection<string> Errors { get; } = errors;
}