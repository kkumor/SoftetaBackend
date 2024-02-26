using Claims.Application.Services;
using Claims.Model;

namespace Claims.Application.Covers.Rates;

public class RateService : IRateService
{
    private const int BaseRate = 1250;

    public decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        var insuranceLength = endDate.DayNumber - startDate.DayNumber;
        if (insuranceLength <= 0)
            return 0m;

        var rateModificator = RateModificatorFactory.GetRateModificator(coverType);
        var premiumPerDay = BaseRate * rateModificator.Multiplier;

        return Enumerable.Range(0, insuranceLength)
            .Select(insuranceDay => premiumPerDay - rateModificator.GetDiscount(insuranceDay))
            .Sum();
    }
}