using Newtonsoft.Json;

namespace BlazorCatAPI.Modeles
{
    public class FavoriteResponse
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }
    }
}
