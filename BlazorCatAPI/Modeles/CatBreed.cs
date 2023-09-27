using Newtonsoft.Json;
namespace BlazorCatAPI.Modeles;
public class CatBreed
{
    [JsonProperty("id")]
    public string id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("origin")]
    public string origin { get; set; }

    [JsonProperty("country_code")]
    public string country_code { get; set; }

    [JsonProperty("life_span")]
    public string life_span {  get; set; }

    [JsonProperty("temperament")]
    public string temperament { get; set; }

    [JsonProperty("weight")]
    public CatWeight weight { get; set; }

}
