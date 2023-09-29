using System.Reflection.Metadata;

namespace BlazorCatAPI.DBLib
{
    public class Favorite
    {
        public string Id { get; set; }
        public string ImageId { get; set; }
        public User User { get; set; } = null!; // Required reference navigation to principal

    }
}