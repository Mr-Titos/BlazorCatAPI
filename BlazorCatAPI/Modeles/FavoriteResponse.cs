using Newtonsoft.Json;

namespace BlazorCatAPI.Modeles
{
    public class FavoriteResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }
    }
}
