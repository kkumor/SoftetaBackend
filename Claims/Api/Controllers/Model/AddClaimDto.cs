using Claims.Model;
using Newtonsoft.Json;

namespace Claims.Controllers.Model;

public class AddClaimDto
{
    [JsonProperty(PropertyName = "coverId")]
    public required string CoverId { get; set; }

    [JsonProperty(PropertyName = "name")] public required string Name { get; set; }

    [JsonProperty(PropertyName = "claimType")]
    public ClaimType Type { get; set; }

    [JsonProperty(PropertyName = "damageCost")]
    public decimal DamageCost { get; set; }
}