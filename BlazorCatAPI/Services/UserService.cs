using BlazorCatAPI.DBLib;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorCatAPI.Services
{
    public class UserService
    {

        private readonly IDbContextFactory<BlazorCatAPIDbContext> ContextFactory;
        BlazorCatAPIDbContext context;

        public UserService(IDbContextFactory<BlazorCatAPIDbContext> contextFactory)
        {
            ContextFactory = contextFactory;
            context = ContextFactory.CreateDbContext();
        }

        public bool IsUserConnected(string userId)
        {
            User? user = context.Users.ToList().Where(u => u.NameIdentifier.Equals(userId)).FirstOrDefault();
            if (user != null)
                return user.IsAuthenticated;
            return false;
        }

        public bool IsUserDarkMode(string userId)
        {
            User? user = context.Users.ToList().Where(u => u.NameIdentifier.Equals(userId)).FirstOrDefault();
            if (user != null)
                return user.isDarkMode;
            return false;
        }

        public async Task SetUserDarkMode(string userId, bool isDarkMode)
        {
            User? user = context.Users.ToList().Where(u => u.NameIdentifier.Equals(userId)).FirstOrDefault();
            if (user != null)
            {
                user.isDarkMode = isDarkMode;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsEmailUnique(string email)
        {
            String? mail = await context.Users.Select(u => u.Email).Where(e => e.ToLower().Equals(email.ToLower())).FirstOrDefaultAsync();
            return mail == null;
        }

        public async Task RegisterUser(string surname, string givenName, string email, string password, string avatar = "", string nameIdentifier = "")
        {
            User user = new User();
            user.NameIdentifier = nameIdentifier != "" ? nameIdentifier : Guid.NewGuid().ToString();
            user.Surname = surname;
            user.GivenName = givenName;
            user.Email = email;
            user.Password = password;
            user.Avatar = avatar;
            user.isDarkMode = false;
            user.IsAuthenticated = false;
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task<bool> AuthUser(string email, string password)
        {
            User? user = await context.Users.Where(u => u.Email.ToLower().Equals(email.ToLower())).FirstOrDefaultAsync();
            if (user != null)
            {
                return user.Email.ToLower().Equals(email.ToLower()) && user.Password.Equals(password);
            }
            return false;
        }

        public async Task<ClaimsPrincipal?> BuildClaimsPrincipal(string email)
        {
            User? user = await context.Users.Where(u => u.Email.ToLower().Equals(email.ToLower())).FirstOrDefaultAsync();

            if (user == null)
                return null;
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.NameIdentifier),
                new Claim(ClaimTypes.Name, user.GivenName + ' ' + user.Surname),
                new Claim(ClaimTypes.GivenName, user.GivenName),
                new Claim(ClaimTypes.Surname, user.Surname),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("urn:blazorcatapi:avatar", user.Avatar),
            };
            
            return new ClaimsPrincipal(new ClaimsIdentity(claims, "db"));
        }
    }
}