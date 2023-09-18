using BlazorCatAPI.Modeles;
using Newtonsoft.Json;
using RestSharp;

namespace BlazorCatAPI.Services;

public class CatService
{
    private static readonly string apiBaseUrl = "https://api.thecatapi.com/v1/";
    private static RestClient client = new RestClient(apiBaseUrl);
    private static string apiKey = String.Empty;

    public async Task<Cat?> GetCatImage()
    {
        var result = await ExecuteRequest(new RestRequest("images/search", Method.Get));
        return result.IsSuccessStatusCode ? JsonConvert.DeserializeObject<Cat[]>(result.Content)?.FirstOrDefault() : null;
    }

    private async Task<RestResponse> ExecuteRequest(RestRequest req)
    {
        req.AddParameter("x-api-key", apiKey);
        return await client.GetAsync(req);
    }

    public CatService(IConfiguration configuration)
    {
        apiKey = configuration["ApiKey"] ?? throw new Exception("Can't retrieve the API key");
    }
}