using Claims.Api.Controllers.Model;
using Claims.Model;
using FluentAssertions;

namespace Claims.Tests.Controllers;

public class ClaimsControllerTests
{
    private static readonly TestClient _sut = TestClientBuilder.CreateTestClient();
    private Cover _testCover = CreateTestCover();

    [Fact]
    public async Task ShouldBeAbleToCreateNewClaim()
    {
        var addClaim = new AddClaimDto
        {
            Name = "Test",
            CoverId = new Guid(_testCover.Id),
            DamageCost = 2,
            Type = ClaimType.Grounding,
            Created = DateTime.UtcNow
        };

        var createdClaim = await _sut.CreateClaim(addClaim);
        createdClaim.Should().NotBeNull();

        var claim = await _sut.GetClaim(createdClaim.Id);
        claim.Should().NotBeNull();
        claim.Id.Should().Be(createdClaim.Id);
    }

    [Fact]
    public async Task ShouldBeAbleToCreateNewClaimAndFindItInClaimsCollection()
    {
        var addClaim = new AddClaimDto
        {
            Name = "Test",
            CoverId = new Guid(_testCover.Id),
            DamageCost = 3,
            Type = ClaimType.BadWeather,
            Created = DateTime.UtcNow
        };

        var createdClaim = await _sut.CreateClaim(addClaim);
        createdClaim.Should().NotBeNull();

        var claims = await _sut.GetClaims();
        claims.Should().NotBeNull();
        claims.Should().ContainSingle(x => x.Id == createdClaim.Id);
    }

    [Fact]
    public async Task ShouldBeAbleToDeleteClaim()
    {
        var addClaim = new AddClaimDto
        {
            Name = "Test",
            CoverId = new Guid(_testCover.Id),
            DamageCost = 4,
            Type = ClaimType.Fire,
            Created = DateTime.UtcNow
        };

        var createdClaim = await _sut.CreateClaim(addClaim);
        createdClaim.Should().NotBeNull();

        await _sut.DeleteClaim(createdClaim.Id);
        var claims = await _sut.GetClaims();
        claims.Should().NotBeNull();
        claims.Should().NotContainEquivalentOf(createdClaim);
    }

    private static Cover? CreateTestCover()
    {
        var addCover = new AddCoverDto
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10)),
            Type = CoverType.ContainerShip
        };

        return _sut.CreateCover(addCover).GetAwaiter().GetResult()!;
    }
}