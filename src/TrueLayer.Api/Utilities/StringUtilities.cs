using System.Text.RegularExpressions;

namespace TrueLayer.Api.Utilities
{
    public static class StringUtilities
    {
        private static readonly Regex WhitespaceRegex = new Regex(@"\s+", RegexOptions.Compiled | RegexOptions.Multiline);
        
        public static string CompactWhitespace(this string str)
        {
            return WhitespaceRegex.Replace(str, " ");
        }
    }
}