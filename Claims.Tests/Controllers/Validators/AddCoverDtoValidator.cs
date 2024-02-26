using Claims.Api.Controllers.Model;
using Claims.Api.Controllers.Validators;
using FluentAssertions;

namespace Claims.Tests.Controllers.Validators;

public class AddCoverDtoValidatorTests
{
    private readonly AddCoverDtoValidator _validator = new();

    [Fact]
    public async Task ValidateShouldReturnErrorWhenStartDateIsInThePast()
    {
        // Arrange
        var inputModel = new AddCoverDto
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.Today)
        };

        // Act
        var result = await _validator.Validate(inputModel);

        // Assert
        result.Errors.Should().Contain(error => error.Contains("StartDate cannot be in the past"));
    }

    [Fact]
    public async Task ValidateShouldReturnErrorWhenCoverPeriodExceedsOneYear()
    {
        // Arrange
        var inputModel = new AddCoverDto
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddYears(1).AddDays(1))
        };

        // Act
        var result = await _validator.Validate(inputModel);

        // Assert
        result.Errors.Should().Contain(error => error.Contains("Total insurance period cannot exceed 1 year"));
    }
}