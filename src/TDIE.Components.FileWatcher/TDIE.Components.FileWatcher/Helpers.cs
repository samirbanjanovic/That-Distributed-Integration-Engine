using System.Text.RegularExpressions;

namespace TDIE.Components.FileWatcher
{
    public static class Helpers
    {
        public static string WildcardToRegex(string wildcard)
        {
            return "^" + Regex.Escape(wildcard)
                              .Replace("\\*", ".*")
                              .Replace("\\?", ".") + "$";
        }

        public static bool IsWildcardMatch(this string value, string wildcard)
        {
            return Regex.IsMatch(value, WildcardToRegex(wildcard), RegexOptions.IgnoreCase);
        }
    }
}
