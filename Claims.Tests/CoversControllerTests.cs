using Claims.Controllers.Model;
using Claims.Model;
using FluentAssertions;
using Xunit;

namespace Claims.Tests;

public class CoversControllerTests
{
    private readonly TestClient _sut = TestClientBuilder.CreateTestClient();

    [Fact]
    public async Task ShouldBeAbleToCreateNewCover()
    {
        var addCover = new AddCoverDto
        {
            EndDate = new DateOnly(2024, 10, 1),
            StartDate = new DateOnly(2024, 10, 2),
            Type = CoverType.ContainerShip
        };

        var createdCover = await _sut.CreateCover(addCover);
        createdCover.Should().NotBeNull();

        var cover = await _sut.GetCover(createdCover.Id);
        cover.Should().NotBeNull();
        cover.Id.Should().Be(createdCover.Id);
    }

    [Fact]
    public async Task ShouldBeAbleToCreateNewCoverAndFindItInCoversCollection()
    {
        var addCover = new AddCoverDto
        {
            EndDate = new DateOnly(2024, 10, 1),
            StartDate = new DateOnly(2024, 10, 2),
            Type = CoverType.BulkCarrier
        };

        var createdCover = await _sut.CreateCover(addCover);
        createdCover.Should().NotBeNull();

        var covers = await _sut.GetCovers();
        covers.Should().NotBeNull();
        covers.Should().ContainSingle(x => x.Id == createdCover.Id);
    }

    [Fact]
    public async Task ShouldBeAbleToDeleteCover()
    {
        var addCover = new AddCoverDto
        {
            EndDate = new DateOnly(2024, 10, 1),
            StartDate = new DateOnly(2024, 10, 2),
            Type = CoverType.PassengerShip
        };

        var createdCover = await _sut.CreateCover(addCover);
        createdCover.Should().NotBeNull();

        await _sut.DeleteCover(createdCover.Id);
        var covers = await _sut.GetCovers();
        covers.Should().NotBeNull();
        covers.Should().NotContainEquivalentOf(createdCover);
    }
}