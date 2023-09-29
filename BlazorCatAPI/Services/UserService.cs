using BlazorCatAPI.DBLib;
using Microsoft.EntityFrameworkCore;

namespace BlazorCatAPI.Services
{
    public class UserService
    {
        private readonly IDbContextFactory<BlazorCatAPIDbContext> ContextFactory;

        public UserService(IDbContextFactory<BlazorCatAPIDbContext> contextFactory)
        {
            ContextFactory = contextFactory;
        }

        public bool IsUserConnected(string userId)
        {
            BlazorCatAPIDbContext context = ContextFactory.CreateDbContext();
            var user = context.Users.ToList().Where(u => u.NameIdentifier.Equals(userId)).FirstOrDefault();
            if (user != null)
                return user.IsAuthenticated;
            return false;
        }

        public bool IsUserDarkMode(string userId)
        {
            BlazorCatAPIDbContext context = ContextFactory.CreateDbContext();
            var user = context.Users.ToList().Where(u => u.NameIdentifier.Equals(userId)).FirstOrDefault();
            if (user != null)
                return user.isDarkMode;
            return false;
        }

        public async Task SetUserDarkMode(string userId, bool isDarkMode)
        {
            BlazorCatAPIDbContext context = ContextFactory.CreateDbContext();
            var user = context.Users.ToList().Where(u => u.NameIdentifier.Equals(userId)).FirstOrDefault();
            if (user != null)
            {
                user.isDarkMode = isDarkMode;
                await context.SaveChangesAsync();
            }
        }

        
    }
}