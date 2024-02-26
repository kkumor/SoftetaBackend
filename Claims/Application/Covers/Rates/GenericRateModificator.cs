namespace Claims.Application.Covers.Rates;

public record GenericRateModificator(decimal Multiplier) : IRateModificator
{
    public decimal GetDiscount(int insuranceDay) =>
        insuranceDay switch
        {
            < 30 => 0m,
            < 180 => 0.02m,
            < 365 => 0.03m,
            _ => 1,
            //_ => throw new ArgumentException("Total insurance period cannot exceed 1 year")
        };
}