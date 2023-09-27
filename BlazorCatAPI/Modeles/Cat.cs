using Newtonsoft.Json;

namespace BlazorCatAPI.Modeles;

public class Cat
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("width")]
    public int Width { get; set; }

    [JsonProperty("height")]
    public int Height { get; set; }

    [JsonProperty("breeds")]
    public List<CatBreed> Breeds { get; set; }

    
    public override String ToString()
    {
        return $"Origin : {Breeds.FirstOrDefault()?.origin}" +
            $"\nAverage life span : {Breeds.FirstOrDefault()?.life_span}" +
            $"\nAverage weight : {Breeds.FirstOrDefault()?.weight}" +
            $"\nTemperament : {Breeds.FirstOrDefault()?.temperament}";
    }
}
