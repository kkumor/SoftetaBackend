using Claims.Model;
using Newtonsoft.Json;

namespace Claims.Api.Controllers.Model;

public class AddCoverDto
{
    [JsonProperty(PropertyName = "startDate")]
    public DateOnly StartDate { get; set; }

    [JsonProperty(PropertyName = "endDate")]
    public DateOnly EndDate { get; set; }

    [JsonProperty(PropertyName = "claimType")]
    public CoverType Type { get; set; }
}