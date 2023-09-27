using Newtonsoft.Json;
namespace BlazorCatAPI.Modeles;
public class CatWeight
{
    [JsonProperty("imperial")]
    public string imperial { get; set; }

    [JsonProperty("metric")]
    public string metric { get; set; }
}
