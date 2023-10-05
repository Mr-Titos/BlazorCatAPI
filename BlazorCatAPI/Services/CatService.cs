using BlazorCatAPI.DBLib;
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

    public CatService(IConfiguration configuration)
    {
        apiKey = configuration["ApiKey"] ?? throw new Exception("Can't retrieve the API key");
    }

    // By default that will get one random cat that has a completed breed
    public async Task<List<Cat>?> SearchCats(Dictionary<string, string> filters = null)
    {
        RestRequest request = new RestRequest("images/search", Method.Get);
        request.AddParameter("has_breeds", "1");

        if (filters != null)
        {
            foreach (var filter in filters)
            {
                request.AddParameter(filter.Key, filter.Value);
            }
        }

        RestResponse result = await ExecuteRequest(request);
        return result.IsSuccessStatusCode ? JsonConvert.DeserializeObject<Cat[]>(result.Content).ToList(): null;
    }
    public async Task<Cat?> GetCat(string imageId)
    {
        RestRequest request = new RestRequest("images/" + imageId, Method.Get);
        RestResponse result = await ExecuteRequest(request);
        return result.IsSuccessStatusCode ? JsonConvert.DeserializeObject<Cat>(result.Content) : null;
    }

    public async Task<bool> FavoriteCat(string imageId, string userId)
    {
        // Add a request to the Cat API because of the in built system of favorite of the API
        // Not useful here because of the need of implementation of the EntityFramework !!
        // TODO : Add a config to be able to switch with the Cat API model and then be able to save favorites in the cloud
        RestRequest request = new RestRequest("favourites", Method.Post);
        Dictionary<string, string> body = new Dictionary<string, string>
        {
            { "image_id", imageId }, { "sub_id", userId}
        };
        request.AddBody(body, ContentType.Json);
        RestResponse result = await ExecuteRequest(request);
        if (!result.Content.Contains("DUPLICATE_FAVOURITE")) // If the image is already favorited with the userID in the API DB ...
        {
            FavoriteResponse favoriteResponse = JsonConvert.DeserializeObject<FavoriteResponse>(result.Content) ?? throw new Exception("Error while adding favorite");
            await UserService.Instance.AddFavorite(userId, favoriteResponse.id, imageId);
        }

        return result.IsSuccessStatusCode;
    }

    public async Task<bool> UnFavoriteCat(string imageId, string userId)
    {
        string favId = await UserService.Instance.RemoveFavorite(userId, imageId);

        // Same as FavoriteCat()
        RestRequest request = new RestRequest($"favourites/{favId}", Method.Delete);
        RestResponse result = await ExecuteRequest(request);

        return result.IsSuccessStatusCode;
    }

    public async Task<bool> VoteCat(string imageId, int isLiked, string userId = "")
    {
        RestRequest request = new RestRequest("votes", Method.Post);
        Dictionary<string, object> body = new Dictionary<string, object>
        {
            {"image_id", imageId}, {"sub_id", userId}, {"value", isLiked}
        };
        request.AddBody(body, ContentType.Json);
        RestResponse result = await ExecuteRequest(request);

        Favorite? f = await UserService.Instance.GetFavorite(userId, imageId);
        if (f != null)
        {
            f.isLiked = isLiked;
            await UserService.Instance.UpdateFavorite(f);
        }
        return result.IsSuccessStatusCode;
    }

    public async Task<List<CatBreed>> GetCatBreeds()
    {
        RestRequest request = new RestRequest("breeds", Method.Get);
        RestResponse result = await ExecuteRequest(request);
        return result.IsSuccessStatusCode ? JsonConvert.DeserializeObject<CatBreed[]>(result.Content).ToList() : throw new Exception("Can't retrieve breeds infos");
    }

    public async Task<List<Cat>> GetHistoric(string userId)
    {
        RestRequest request = new RestRequest("votes", Method.Get);
        request.AddParameter("sub_id", userId);
        request.AddParameter("order", "DESC");
        request.AddParameter("limit", "10");

        RestResponse result = await ExecuteRequest(request);
        List<Cat> historic = new List<Cat>();
        List<VoteResponse> votes = result.IsSuccessStatusCode ? JsonConvert.DeserializeObject<VoteResponse[]>(result.Content).ToList() : throw new Exception("Can't retrieve historic");

        foreach (VoteResponse vote in votes)
        {
            Cat? cat = await GetCat(vote.catId);
            if (cat != null)
            {
                cat.isLiked = vote.value;
                historic.Add(cat);
            }    
        }
        return historic;
    }


    private async Task<RestResponse> ExecuteRequest(RestRequest req)
    {
        req.AddHeader("x-api-key", apiKey);
        return await client.ExecuteAsync(req);
    }
}