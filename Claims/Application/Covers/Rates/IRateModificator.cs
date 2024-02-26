namespace Claims.Application.Covers.Rates;

public interface IRateModificator
{
    decimal Multiplier { get; init; }
    decimal GetDiscount(int insuranceDay);
}