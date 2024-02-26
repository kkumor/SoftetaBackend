using Claims.Model;

namespace Claims.Application.Services;

public interface IRateService
{
    decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType);
}