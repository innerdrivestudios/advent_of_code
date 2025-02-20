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
}