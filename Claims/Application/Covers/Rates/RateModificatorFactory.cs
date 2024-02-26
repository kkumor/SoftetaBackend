using Claims.Model;

namespace Claims.Application.Covers.Rates;

public static class RateModificatorFactory
{
    public static IRateModificator GetRateModificator(CoverType coverType)
    {
        var multiplier = GetMultiplier(coverType);
        return coverType switch
        {
            CoverType.Yacht => new YachtRateModificator(multiplier),
            _ => new GenericRateModificator(multiplier)
        };
    }

    private static decimal GetMultiplier(CoverType coverType)
    {
        return coverType switch
        {
            CoverType.Yacht => 1.1m,
            CoverType.PassengerShip => 1.2m,
            CoverType.Tanker => 1.5m,
            _ => 1.3m
        };
    }
}