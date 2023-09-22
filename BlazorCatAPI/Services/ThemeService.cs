namespace BlazorCatAPI.Services
{
    public class ThemeService
    {
        public Dictionary<string, bool> isSessionDarkMode { get; set; } = new Dictionary<string, bool>();
    }
}