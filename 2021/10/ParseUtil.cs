static class ParseUtil
{
    static string openingChars = "({[<";
    static string closingChars = ")}]>";

    static readonly Dictionary<char, char> charMap = new Dictionary<char, char>() {
        { '(', ')' }, { '[', ']' } , { '<', '>' } , { '{', '}' }
    };

    public static bool IsOpeningChar (char c) => openingChars.Contains(c);
    public static bool IsClosingChar (char c) => closingChars.Contains(c);
    public static bool Match (char c1, char c2) => charMap[c1] == c2;
    public static char GetClosingChar(char c) => charMap[c];
}

