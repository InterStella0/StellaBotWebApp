using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;

namespace StellaBotWebApp.Models
{
    public class PayloadUser
    {
        public const string Id = "id";
        public const string Name = "username";
        public const string Discriminator = "discriminator";
        public const string Avatar = "avatar";
        public const string Banner = "banner";
        public const string AccentColor = "accent_color";
        public const string PublicFlags = "public_flags";
    }
    public class DiscordUser
    {
        public const string CDN = "https://cdn.discordapp.com";
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Discriminator { get; set; }
        public string? Avatar { get; set; }
        public string? Banner { get; set; }
        public int? AccentColor { get; set; }
        public int PublicFlags { get; set; }
        public ClaimsPrincipal? User;
        private string GetValue(string type)
        {
            if (User == null)
                return "";

            var claims = User.Claims;
            var value = claims.FirstOrDefault(c => c.Type == type);
            if (value == null)
                return "";

            return value.Value;
        }
        public DiscordUser(ClaimsPrincipal? user)
        {
            if (user is null)
            {
                Id = null;
                Name = "";
                Discriminator = "";
                PublicFlags = 0;
                return;
            }

            User = user;
            Id = GetValue(ClaimTypes.NameIdentifier);
            Name = GetValue(ClaimTypes.Name);
            Discriminator = GetValue(PayloadUser.Discriminator);
            Avatar = GetValue(PayloadUser.Avatar);
            Banner = GetValue(PayloadUser.Banner);
            var accent = GetValue(PayloadUser.AccentColor);
            var flags = GetValue(PayloadUser.PublicFlags);
            AccentColor = accent != ""? int.Parse(accent): null;
            PublicFlags = flags != "" ? int.Parse(flags) : 0;
        }
        public bool IsValid { get => Id is not null; }
        public string AvatarUrl { get => $"{CDN}/avatars/{Id}/{Avatar}"; }
        public string Username { get => $"{Name}#{Discriminator}"; }
    }
    public class LoginModelView
    {
        public DiscordUser DiscordLogged;
    }
}
