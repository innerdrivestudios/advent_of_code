//Solution for https://adventofcode.com/2015/day/13 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of personal +- happiness gains/losses based
// on who a person is seated next to at a round dinner table

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

// ** Your tasks: find the optimal seating arrangement to ensure the highest overall dinner table happiness

//Step 1. Process the input into a neighbour vs neighbour to "cost" map, while building a list of unique persons

Dictionary<(string, string), int> neighbours2CostMap = new Dictionary<(string personA, string personB), int>();
HashSet<string> partyAttendants = new HashSet<string>();
ConvertInput (myInput, neighbours2CostMap, partyAttendants);

//Step 2. Run both challenges

// ** Part 1: Use the provided seating configuration as is:

List<List<string>> possibleSeatingArrangements = partyAttendants.ToList().GetPermutations();
Console.WriteLine("Part 1:" + GetBestConfigurationScore(possibleSeatingArrangements, neighbours2CostMap));

// ** Part 2: Add "Me", but no relationships so all my scores will equal to zero

partyAttendants.Add("Me");
possibleSeatingArrangements = partyAttendants.ToList().GetPermutations();

Console.WriteLine("Part 2:" + GetBestConfigurationScore(possibleSeatingArrangements, neighbours2CostMap));

//////////////////////////////////// HELPER METHODS ///////////////////////////////////////

// Convert input to a costmap and attendents set

void ConvertInput (string pInput, Dictionary<(string, string), int> pCostmap, HashSet<string> pAttendants)
{
    string pattern = @"(\w+) would (lose|gain) (\d+) happiness units by sitting next to (\w+)\.";
    MatchCollection matches = Regex.Matches(pInput, pattern);

    foreach (Match match in matches)
    {
        if (match.Success)
        {
            string personA = match.Groups[1].Value;
            string personB = match.Groups[4].Value;
            string action = match.Groups[2].Value;
            int value = int.Parse(match.Groups[3].Value);

            pCostmap[(personA, personB)] = (action == "gain") ? value : -value;
            pAttendants.Add(personA);
        }
    }
}

// Go through all possible seating arrangements and check which arrangements has the best score

int GetBestConfigurationScore(List<List<string>> pPossibleSeatingArrangements, Dictionary<(string, string), int> pCostmap)
{
    int bestConfiguration = int.MinValue;

    foreach (List<string> seatingArrangement in pPossibleSeatingArrangements)
    {
        int score = GetResult(seatingArrangement, pCostmap);
        bestConfiguration = Math.Max(bestConfiguration, score);
    }
	
	return bestConfiguration;
}

// Checks a seating arrangements around the table (loops around) and adds all the "costs" based on the provided costmap

int GetResult (List<string> pSeatingArrangement, Dictionary<(string, string), int> pCostmap)
{
	int total = 0;
	for (int i = 0; i < pSeatingArrangement.Count; i++)
	{
        string personA = pSeatingArrangement[i];
        string personB = pSeatingArrangement[(i + 1) % pSeatingArrangement.Count];

        //If there are people in the attendance list who are not in the map just use a score of zero for them (aka skip 'm)
        if (!pCostmap.ContainsKey ((personA, personB)) || !pCostmap.ContainsKey((personB, personA))) continue;

        //Don't use exceptions to catch invalid items, since it would be very slow
	    total += pCostmap[(personA, personB)];
	    total += pCostmap[(personB, personA)];
    }

	return total;
}
