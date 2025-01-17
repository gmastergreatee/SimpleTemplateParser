namespace SimpleTokenParser
{
    public class TokenParserConstants
    {
        public static string ModelTokenCharacter { get; set; } = "@@@";
        public static string PropertyTokenCharacter { get; set; } = "##";
        public static string TokenClassSplitCharacter { get; set; } = ".";
        public static string[] IgnoreTokens { get; set; } = new string[]
        {
            "BODY"
        };
    }
}
