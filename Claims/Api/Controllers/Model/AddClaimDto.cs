using Claims.Model;
using Newtonsoft.Json;

namespace Claims.Api.Controllers.Model;

public class AddClaimDto
{
    [JsonProperty(PropertyName = "coverId")]
    public required Guid CoverId { get; set; }

    [JsonProperty(PropertyName = "name")] public required string Name { get; set; }

    [JsonProperty(PropertyName = "claimType")]
    public ClaimType Type { get; set; }

    [JsonProperty(PropertyName = "damageCost")]
    public decimal DamageCost { get; set; }

    [JsonProperty(PropertyName = "created")]
    public DateTime Created { get; set; }
}