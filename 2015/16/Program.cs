//Solution for https://adventofcode.com/2015/day/16 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;
using Aunt = System.Collections.Generic.Dictionary<string, int>; //Aunts map characterics to values

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input:
// - things you know about the aunt that sent your gift
// - all aunts sue you know ;)

string senderAuntSue = "children: 3\r\ncats: 7\r\nsamoyeds: 2\r\npomeranians: 3\r\nakitas: 0\r\nvizslas: 0\r\ngoldfish: 5\r\ntrees: 3\r\ncars: 2\r\nperfumes: 1\r\n";
string allAuntSueData = File.ReadAllText(args[0]).ReplaceLineEndings();

// ** Your task: find out which Aunt Sue in the allAuntSueData matches the senderAuntSue

// Step 1. Convert all input data to a more useable format

// Add our senderAuntSue as sue 0 to the input string so we can parse everything at once and make sure id matches index in the aunts[]
allAuntSueData = "Sue 0: " + senderAuntSue.Replace("\r\n", ",") + "\r\n" + allAuntSueData;
Aunt[] aunts = ConvertInput(allAuntSueData);

Aunt[] ConvertInput (string pInput)
{
	List<Aunt> sues = new List<Aunt>();
	
	//We only capture the string that contains all characteristics from the Sue blahblah string
	Regex sueParser = new Regex(@"Sue \d*: (.*)\r\n");

	foreach(Match match in sueParser.Matches(pInput))
	{
		//split the characterics (groups[1]) on , then on : and convert the resulting pairs to a dictionary 
        sues.Add(
            match.Groups[1].Value.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split(":")).ToDictionary(x => x[0].Trim(), x => int.Parse(x[1]))
        );
	}

	return sues.ToArray();
}

// Step 2. Find the correct aunt(s)

Console.WriteLine("Part 1:" + FindExactMatchIndex(aunts));
Console.WriteLine("Part 2:" + FindRangedMatchIndex(aunts));

int FindExactMatchIndex(Aunt[] pAunts)
{
    for (int i = 1; i < pAunts.Length; i++)
    {
		if (IsExactMatch(pAunts[0], pAunts[i])) return i;
    }

	return -1;
}

int FindRangedMatchIndex(Aunt[] pAunts)
{
    for (int i = 1; i < pAunts.Length; i++)
    {
        if (IsRangedMatch(pAunts[0], pAunts[i])) return i;
    }

    return -1;
}


bool IsExactMatch(Aunt pAuntA, Aunt pAuntB)
{
    foreach (var keyvaluePair in pAuntB)
    {
		//if auntA HAS a key defined by auntB it HAS to match otherwise return false
        if (pAuntA.ContainsKey(keyvaluePair.Key) && pAuntA[keyvaluePair.Key] != keyvaluePair.Value) return false;
    }

    return true;
}

bool IsRangedMatch(Aunt pAuntA, Aunt pAuntB)
{
	foreach (var keyvaluePair in pAuntB)
	{
		if (pAuntA.ContainsKey(keyvaluePair.Key))
		{
			switch (keyvaluePair.Key)
			{
				case "cats":
				case "trees":
					//auntA has to have less than auntB
					if (pAuntA[keyvaluePair.Key] >= keyvaluePair.Value) return false;
					break;

				case "pomeranians":
				case "goldfish":
					//auntA has to have more than auntB
					if (pAuntA[keyvaluePair.Key] <= keyvaluePair.Value) return false;
					break;

				default:
					if (pAuntA[keyvaluePair.Key] != keyvaluePair.Value) return false;
					break;
			}
		}
	}

	return true;
}
