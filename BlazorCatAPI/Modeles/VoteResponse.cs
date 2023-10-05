using Newtonsoft.Json;

namespace BlazorCatAPI.Modeles
{
    public class VoteResponse
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("value")]
        public int value { get; set; }

        [JsonProperty("sub_id")]
        public string userId { get; set; }

        [JsonProperty("image_id")]
        public string catId { get; set; }
    }
}
