namespace BlazorCatAPI.DBLib
{
    public class User
    {
        public string NameIdentifier { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
        public bool isDarkMode { get; set; }
        public bool IsAuthenticated { get; set; }
        public ICollection<Favorite> Favorites { get; } = new List<Favorite>();

    }
}