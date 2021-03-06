
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StellaBotWebApp.Models
{
    public class APIBotInfo
    {
        public string? Avatar { get; set; }
        public string? OwnerAvatar { get; set; }
        public string? OwnerName { get; set; }
        public string? Name { get; set; }
        public long? GuildAmount { get; set; }
        public long? UserAmount { get; set; }
        public List<Dictionary<string, object>> LastCommands { get; set; }
        public double? Latency { get; set; }
        public long? CodeLines { get; set; }
        public DateTime? UpTime { get; set; }
    }
    public class StellaAPI
    {
        private static readonly HttpClient client = new HttpClient();
        public APIBotInfo BotInfo { get; set; }
        const string BaseUrl = "http://api.interstella.online";
        public string FormUrl(params string[] subs)
        {
            return $"{BaseUrl}/{string.Join('/', subs)}";
        }
        private static T strToJson<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        public async Task<T> GetRequestAsync<T>(string path)
        {
            var full_path = FormUrl("stella_bot", path);
            var response = await client.GetAsync(full_path);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            return strToJson<T>(jsonString);
        }
        public async Task<Dictionary<string, string>?> GetRequestJsonAsync(string path)
        {
            try
            {
                return await GetRequestAsync<Dictionary<string, string>>(path);
            }catch (HttpRequestException ex)
            {
                return null;
            }
        }
        private T? GetValueDict<T>(Dictionary<string, T> dict, string key)
        {
            return dict is not null ? dict[key] : default;
        }
        private T? GetValueDictObj<T>(Dictionary<string, object> dict, string key)
        {
            return dict is not null ? (T) dict[key] : default;
        }
        public async Task Init()
        {
            var botResult = await GetRequestJsonAsync("name");
            var ownerResult = await GetRequestJsonAsync("owner_name");
            BotInfo = new APIBotInfo()
            {
                Avatar = FormUrl("stella_bot", "avatar"),
                OwnerAvatar = FormUrl("stella_bot", "owner_avatar"),
                Name = GetValueDict(botResult, "full"),
                OwnerName = GetValueDict(ownerResult, "full")
            };
            try
            {
                var botRealTimeInfo = await GetRequestAsync<Dictionary<string, object>>("info");
                var lastCommands = ((JArray)botRealTimeInfo["last_commands"]).ToObject<List<Dictionary<string, object>>>();

                BotInfo.GuildAmount = GetValueDictObj<long>(botRealTimeInfo, "guild_amount");
                BotInfo.UserAmount = GetValueDictObj<long>(botRealTimeInfo, "user_amount");
                BotInfo.LastCommands = lastCommands;
                BotInfo.Latency = GetValueDictObj<double>(botRealTimeInfo, "latency");
                BotInfo.CodeLines = GetValueDictObj<long>(botRealTimeInfo, "codelines");
                BotInfo.UpTime = GetValueDictObj<DateTime>(botRealTimeInfo, "launch_time");
            }
            catch (HttpRequestException ex)
            {
                BotInfo.LastCommands = new List<Dictionary<string, object>>();
            }

        }
        public static string ToHumanReadableString(TimeSpan t)
        {
            if (t.TotalSeconds <= 1)
            {
                return $@"{t:s\.ff} seconds";
            }
            if (t.TotalMinutes <= 1)
            {
                return $@"{t:%s} seconds";
            }
            if (t.TotalHours <= 1)
            {
                return $@"{t:%m} minutes";
            }
            if (t.TotalDays <= 1)
            {
                return $@"{t:%h} hours";
            }

            return $@"{t:%d} days";
        }

    }
}
