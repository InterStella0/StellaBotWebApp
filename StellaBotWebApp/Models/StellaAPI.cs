
using Newtonsoft.Json;

namespace StellaBotWebApp.Models
{

    public class APIBotInfo
    {
        public string Avatar { get; set; }
        public string OwnerAvatar { get; set; }
        public string OwnerName { get; set; }
        public string Name { get; set; }
    }
    public class StellaAPI
    {
        private static readonly HttpClient client = new HttpClient();
        public APIBotInfo BotInfo { get; set; }
        const string BaseUrl = "http://interstella.online";
        public string FormUrl(params string[] subs)
        {
            return $"{BaseUrl}/{string.Join('/', subs)}";
        }
        public async Task<Dictionary<string, string>> GetRequestAsync(string path)
        {
            var full_path = FormUrl("stella_bot", path);
            var response = await client.GetAsync(full_path);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);

        }
        public async Task Init()
        {
            var botResult = await GetRequestAsync("name");
            var ownerResult = await GetRequestAsync("owner_name");
            BotInfo = new APIBotInfo()
            {
                Avatar = FormUrl("stella_bot", "avatar"),
                OwnerAvatar = FormUrl("stella_bot", "owner_avatar"),
                Name = botResult["full"],
                OwnerName = ownerResult["full"]
            };
        }

    }
}
