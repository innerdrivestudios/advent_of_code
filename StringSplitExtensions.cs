static class StringExtensions
{
    private const StringSplitOptions Clean =
        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    // string separator
    public static string[] Splat(this string input, string separator) =>
        input.Split(separator, Clean);

    // string[] separators
    public static string[] Splat(this string input, string[] separators) =>
        input.Split(separators, Clean);

    // char separator
    public static string[] Splat(this string input, char separator) =>
        input.Split(separator, Clean);

    // char[] separators
    public static string[] Splat(this string input, char[] separators) =>
        input.Split(separators, Clean);

    // default whitespace split (like string.Split())
    public static string[] Splat(this string input) =>
        input.Split((char[]?)null, Clean);

    // split on Environment.NewLine
    public static string[] SplatLines(this string input) =>
        input.Split(Environment.NewLine, Clean);

    // split on Environment.NewLine + Environment.NewLine
    public static string[] SplatParagraphs(this string input) =>
        input.Split(
            Environment.NewLine + Environment.NewLine,
            Clean);
}
