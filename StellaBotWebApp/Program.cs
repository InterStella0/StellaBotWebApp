using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using StellaBotWebApp.Models;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opt.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

}).AddCookie()
.AddOAuth("Discord", opt =>
{
    var discordPath = (string path) => $"https://discord.com/api{path}";
    opt.AuthorizationEndpoint = discordPath("/oauth2/authorize");
    opt.Scope.Add("identify");
    opt.CallbackPath = new PathString("/auth/oauth");
    opt.ClientId = builder.Configuration.GetValue<string>("Discord:Client-Id");
    opt.ClientSecret = builder.Configuration.GetValue<string>("Discord:Client-Secret");

    opt.TokenEndpoint = discordPath("/oauth2/token");
    opt.UserInformationEndpoint = discordPath("/users/@me");

    opt.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, PayloadUser.Id);
    opt.ClaimActions.MapJsonKey(ClaimTypes.Name, PayloadUser.Name);
    opt.ClaimActions.MapJsonKey(PayloadUser.Discriminator, PayloadUser.Discriminator, PayloadUser.Discriminator);
    opt.ClaimActions.MapJsonKey(PayloadUser.Avatar, PayloadUser.Avatar, PayloadUser.Avatar);
    opt.ClaimActions.MapJsonKey(PayloadUser.Banner, PayloadUser.Banner, PayloadUser.Banner);
    opt.ClaimActions.MapJsonKey(PayloadUser.AccentColor, PayloadUser.AccentColor, PayloadUser.AccentColor);
    opt.ClaimActions.MapJsonKey(PayloadUser.PublicFlags, PayloadUser.PublicFlags, PayloadUser.PublicFlags);

    opt.AccessDeniedPath = "/Login/OauthFailed";

    opt.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
            response.EnsureSuccessStatusCode();

            var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

            context.RunClaimActions(user);
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
