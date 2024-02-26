using Claims.Application.Covers.Rates;
using Claims.Model;

namespace Claims.Tests.Application.Covers;

public class RateServiceTests
{
    readonly RateService _sut = new();

    [Fact]
    public async Task ShouldComputePremiumAsExpected()
    {
        var results = GetScenarios()
            .Select(scenario => (scenario, _sut.ComputePremium(scenario.Start, scenario.End, scenario.CoverType)))
            .ToList();

        await Verify(results);
    }

    private static IEnumerable<RateServiceTestScenario> GetScenarios()
    {
        var startDate = new DateOnly(2024, 1, 1);
        (DateOnly start, DateOnly end)[] periods =
        [
            (startDate, startDate),
            (startDate, startDate.AddDays(1)),
            (startDate, startDate.AddDays(30)),
            (startDate, startDate.AddDays(180)),
            (startDate, startDate.AddDays(181)),
            (startDate, startDate.AddDays(365)),
            (startDate, startDate.AddDays(366)),
        ];

        foreach (var coverType in Enum.GetValues<CoverType>())
        foreach (var (start, end) in periods)
        {
            yield return new(start, end, coverType);
        }
    }

    public record RateServiceTestScenario(DateOnly Start, DateOnly End, CoverType CoverType);
}