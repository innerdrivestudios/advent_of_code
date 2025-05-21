using System.Globalization;
using System.Numerics;

static class ParseUtils
{
	public static T[] FileToNumbers<T>(string pFilename, string pSeparator = ",") where T : INumber<T>
	{
		string fileContents = File.ReadAllText(pFilename);
		fileContents = fileContents.ReplaceLineEndings(Environment.NewLine);
		return StringToNumbers<T>(fileContents, pSeparator);
	}

	public static T[] StringToNumbers<T>(string pFileContents, string pSeparator = ",") where T : INumber<T>
	{
		//If our separator isn't a newline, remove all line endings
		if (pSeparator != Environment.NewLine) pFileContents = pFileContents.ReplaceLineEndings("");

		return pFileContents
			.Split(pSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select (x => T.Parse (x, CultureInfo.InvariantCulture))
			.ToArray();
	}

    public static T[] FileToArrayOf<T>(string pFilename, string pSeparator = ",") 
    {
        string fileContents = File.ReadAllText(pFilename);
        fileContents = fileContents.ReplaceLineEndings(Environment.NewLine);
        return StringToArrayOf<T>(fileContents, pSeparator);
    }

    public static T[] StringToArrayOf<T>(string pFileContents, string pSeparator = ",")
    {
        //If our separator isn't a newline, remove all line endings
        if (pSeparator != Environment.NewLine) pFileContents = pFileContents.ReplaceLineEndings("");

        return pFileContents
            .Split(pSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => (T)Convert.ChangeType(x, typeof (T)))
            .ToArray();
    }

    public static (string, string) FileToStringTuple(string pFilename)
    {
        string input = File.ReadAllText(pFilename);
        input = input.ReplaceLineEndings (Environment.NewLine);

        return input
            .Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Chunk(2)
            .Select(x => (x[0], x[1]))
            .First();
    }

    public static List<(T, U)> StringToTuples<T, U>(string pContent, string pSplitToken)
    {
        pContent = pContent.ReplaceLineEndings(Environment.NewLine);

        return pContent
            .Split([pSplitToken, Environment.NewLine], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Chunk(2)
            .Select(x => ((T)Convert.ChangeType(x[0],typeof(T)), (U)Convert.ChangeType(x[1], typeof(U))))
            .ToList();
    }

    public static string[] SplitBlocks(string pInput)
    {
        return pInput.ReplaceLineEndings(Environment.NewLine).Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

}