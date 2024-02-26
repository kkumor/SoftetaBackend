using Claims.Controllers.Model;
using Claims.Model;
using FluentAssertions;

namespace Claims.Tests.Controllers;

public class ClaimsControllerTests
{
    private readonly TestClient _sut = TestClientBuilder.CreateTestClient();

    [Fact]
    public async Task ShouldBeAbleToCreateNewClaim()
    {
        var addClaim = new AddClaimDto
        {
            Name = "Test",
            CoverId = "1",
            DamageCost = 2,
            Type = ClaimType.Grounding
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
            CoverId = "2",
            DamageCost = 3,
            Type = ClaimType.BadWeather
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
            CoverId = "3",
            DamageCost = 4,
            Type = ClaimType.Fire
        };

        var createdClaim = await _sut.CreateClaim(addClaim);
        createdClaim.Should().NotBeNull();

        await _sut.DeleteClaim(createdClaim.Id);
        var claims = await _sut.GetClaims();
        claims.Should().NotBeNull();
        claims.Should().NotContainEquivalentOf(createdClaim);
    }
}