//Solution for https://adventofcode.com/2017/day/4 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of passphrases like "nyot babgr babgr kqtu kqtu kzshonp ylyk psqk"

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Part 1 - Count how many passphrases are valid.
// A passphrase is valid if it doesn't contain any duplicate words

int validPassphraseCount = 0;

string[] passphrases = myInput.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

foreach (string passphrase in passphrases)
{
    string[] passphraseParts = passphrase.Split(' ');
    if (passphraseParts.Length == passphraseParts.Distinct().Count()) validPassphraseCount++;
}

Console.WriteLine("Part 1 - Valid passphrase count: " + validPassphraseCount);

// ** Part 2 - Count how many passphrases are valid (again :))
// A passphrase is valid if it doesn't contain any duplicate sequence of chars in any order

validPassphraseCount = 0;

foreach (string passphrase in passphrases)
{
    string[] passphraseParts = passphrase.Split(' ');

    //preprocess all parts to sort all chars in a passphase, so anagrams are filtered out
    for (int i = 0; i < passphraseParts.Length; i++)
    {
        passphraseParts[i] = string.Concat (passphraseParts[i].Order());
    }

    if (passphraseParts.Length == passphraseParts.Distinct().Count()) validPassphraseCount++;
}

Console.WriteLine("Part 2 - Valid passphrase count: " + validPassphraseCount);

