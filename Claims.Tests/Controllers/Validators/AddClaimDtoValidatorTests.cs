using Claims.Api.Controllers.Model;
using Claims.Api.Controllers.Validators;
using Claims.Application.Services;
using Claims.Model;
using FluentAssertions;
using Moq;

namespace Claims.Tests.Controllers.Validators;

public class AddClaimDtoValidatorTests
{
    private readonly Mock<ICoversService> _coversServiceMock;
    private readonly AddClaimDtoValidator _validator;

    public AddClaimDtoValidatorTests()
    {
        _coversServiceMock = new Mock<ICoversService>();
        _validator = new AddClaimDtoValidator(_coversServiceMock.Object);
    }

    [Fact]
    public async Task ValidateShouldReturnErrorWhenDamageCostExceedsMax()
    {
        // Arrange
        var inputModel = new AddClaimDto { DamageCost = 100001m, Name = "", CoverId = Guid.Empty };

        // Act
        var result = await _validator.Validate(inputModel);

        // Assert
        result.Errors.Should().Contain("DamageCost cannot exceed 100000");
    }

    [Fact]
    public async Task ValidateShouldReturnErrorWhenCoverIdIsInvalid()
    {
        // Arrange
        var inputModel = new AddClaimDto { CoverId = Guid.NewGuid(), Name = "" };
        _coversServiceMock
            .Setup(service => service.GetCoverAsync(inputModel.CoverId, default))
            .ReturnsAsync((Cover)null);

        // Act
        var result = await _validator.Validate(inputModel);

        // Assert
        result.Errors.Should().Contain($"Invalid CoverId value {inputModel.CoverId}");
    }

    [Fact]
    public async Task ValidateShouldReturnErrorWhenCreatedDateIsOutsideCoverPeriod()
    {
        // Arrange
        var inputModel = new AddClaimDto { CoverId = Guid.NewGuid(), Created = DateTime.UtcNow, Name = "" };
        var relatedCover = new Cover
        {
            StartDate = DateOnly.FromDateTime(inputModel.Created.AddDays(-2)),
            EndDate = DateOnly.FromDateTime(inputModel.Created.AddDays(-1))
        };
        _coversServiceMock
            .Setup(service => service.GetCoverAsync(inputModel.CoverId, default))
            .ReturnsAsync(relatedCover);

        // Act
        var result = await _validator.Validate(inputModel);

        // Assert
        result.Errors.Should().Contain("Created date must be within the period of the related Cover");
    }
}