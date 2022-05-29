namespace StellaBotWebApp.Common
{
    public static class StringUtils
    {
        public static string ShowValue(string? value)
        {
            return value is null || value.Equals("") ? "N/A": value;
        }
    }
}
