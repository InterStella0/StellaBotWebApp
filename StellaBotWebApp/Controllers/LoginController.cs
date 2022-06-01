using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellaBotWebApp.Models;
using System.Security.Claims;

namespace StellaBotWebApp.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = "Discord")]
        public async Task<IActionResult> Oauth()
        {
            var discord = new DiscordUser(User);
            if (!discord.IsValid)
            {
                return Unauthorized();
            }

            return View(new LoginModelView()
            {
                DiscordLogged = discord
            });
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect(Url.Content("~/"));
        }
    }
}
