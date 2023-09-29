using BlazorCatAPI.DBLib;
using BlazorCatAPI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddAuthenticationCore();
builder.Services.AddAuthentication();
builder.Services.AddSingleton<UserService>(); // Scoped service was still sharing between multiple browsers ?
builder.Services.AddSingleton<CatService>();

builder.Services.AddDbContextFactory<BlazorCatAPIDbContext>(
    o => o.UseSqlite("Data Source=BlazorCatAPI.db"));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddAuthentication().AddGoogle(options =>
{
    var clientId = builder.Configuration["Google:ClientId"];
    options.ClientId = clientId;
    options.ClientSecret = builder.Configuration["Google:ClientSecret"];
    options.ClaimActions.MapJsonKey("urn:google:profile", "link");
    options.ClaimActions.MapJsonKey("urn:google:image", "picture");
});

// From https://github.com/aspnet/Blazor/issues/1554

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HttpContextAccessor>();
// Required for HttpClient support in the Blazor project
builder.Services.AddHttpClient();
builder.Services.AddScoped<HttpClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
} else
{
    using IServiceScope scope = app.Services.CreateScope();

    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<BlazorCatAPIDbContext>>();

    using BlazorCatAPIDbContext context = factory.CreateDbContext();

    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    context.Users.Add(new User()
    {
        NameIdentifier = "117331122439535472241",
        GivenName = "Arthur",
        Surname = "Titos",
        Avatar = "https://screenshots.codesandbox.io/xtpzc/605.png",
        IsAuthenticated = false,
        isDarkMode = true,
    });

    context.SaveChanges();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseCookiePolicy();

app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();