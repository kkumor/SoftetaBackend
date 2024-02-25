using Newtonsoft.Json;

namespace Claims.Model
{
    public class Claim
    {
        [JsonProperty(PropertyName = "id")] 
        public required string Id { get; set; }

        [JsonProperty(PropertyName = "coverId")]
        public required string CoverId { get; set; }

        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }

        [JsonProperty(PropertyName = "name")] 
        public required string Name { get; set; }

        [JsonProperty(PropertyName = "claimType")]
        public ClaimType Type { get; set; }

        [JsonProperty(PropertyName = "damageCost")]
        public decimal DamageCost { get; set; }
    }
}