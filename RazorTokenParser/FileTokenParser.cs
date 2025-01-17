using SimpleTokenParser.Models;
using System.IO;

namespace SimpleTokenParser
{
    public static class FileTokenParser
    {
        public static ITokenParserModel<T> ParseTemplate<T>(string filePath) where T : class
        {
            var templateLines = File.ReadAllText(filePath);
            return StringTokenParser.ParseTemplate<T>(templateLines);
        }
    }
}
