using BlazorCatAPI.Modeles;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Security.Claims;

namespace BlazorCatAPI.Services;

public class CatService
{
    private static readonly string apiBaseUrl = "https://api.thecatapi.com/v1/";
    private static RestClient client = new RestClient(apiBaseUrl);
    private static string apiKey = String.Empty;

    public async Task<Cat?> GetCatImage()
    {
        RestRequest request = new RestRequest("images/search", Method.Get);
        request.AddParameter("has_breeds", "1");
        RestResponse result = await ExecuteRequest(request);
        return result.IsSuccessStatusCode ? JsonConvert.DeserializeObject<Cat[]>(result.Content).First() : null;
    }

    public async Task<bool> FavoriteCat(string imageId, string userId)
    {
        RestRequest request = new RestRequest("favourites", Method.Post);
        Dictionary<string, string> body = new Dictionary<string, string>
        {
            { "image_id", imageId }, { "sub_id", userId}
        };

        RestResponse result = await ExecuteRequest(request);
        // TODO récupérer l'id du Favori + le stocker en BDD.
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> UnFavoriteCat(string imageId, string userId)
    {
        // TODO : Request BDD to get the favorite ID to delete
        RestRequest request = new RestRequest("favourites/{favID}", Method.Delete);
        RestResponse result = await ExecuteRequest(request);
        return result.IsSuccessStatusCode;
    }
    private async Task<RestResponse> ExecuteRequest(RestRequest req)
    {
        req.AddHeader("x-api-key", apiKey);
        return await client.GetAsync(req);
    }

    public CatService(IConfiguration configuration)
    {
        apiKey = configuration["ApiKey"] ?? throw new Exception("Can't retrieve the API key");
    }
}