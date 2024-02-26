namespace Claims.Application.Covers.Rates;

public record YachtRateModificator(decimal Multiplier) : IRateModificator
{
    public decimal GetDiscount(int insuranceDay) =>
        insuranceDay switch
        {
            < 30 => 0m,
            < 180 => 0.05m,
            < 365 => 0.08m,
            _ => 1,
            //_ => throw new ArgumentException("Total insurance period cannot exceed 1 year")
        };
}