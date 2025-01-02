//Solution for https://adventofcode.com/2015/day/13 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

//Your input: a list of personal +- happiness gains/losses based
//on who a person is seated next to at a round dinner table
string myInput = "Alice would lose 2 happiness units by sitting next to Bob.\r\nAlice would lose 62 happiness units by sitting next to Carol.\r\nAlice would gain 65 happiness units by sitting next to David.\r\nAlice would gain 21 happiness units by sitting next to Eric.\r\nAlice would lose 81 happiness units by sitting next to Frank.\r\nAlice would lose 4 happiness units by sitting next to George.\r\nAlice would lose 80 happiness units by sitting next to Mallory.\r\nBob would gain 93 happiness units by sitting next to Alice.\r\nBob would gain 19 happiness units by sitting next to Carol.\r\nBob would gain 5 happiness units by sitting next to David.\r\nBob would gain 49 happiness units by sitting next to Eric.\r\nBob would gain 68 happiness units by sitting next to Frank.\r\nBob would gain 23 happiness units by sitting next to George.\r\nBob would gain 29 happiness units by sitting next to Mallory.\r\nCarol would lose 54 happiness units by sitting next to Alice.\r\nCarol would lose 70 happiness units by sitting next to Bob.\r\nCarol would lose 37 happiness units by sitting next to David.\r\nCarol would lose 46 happiness units by sitting next to Eric.\r\nCarol would gain 33 happiness units by sitting next to Frank.\r\nCarol would lose 35 happiness units by sitting next to George.\r\nCarol would gain 10 happiness units by sitting next to Mallory.\r\nDavid would gain 43 happiness units by sitting next to Alice.\r\nDavid would lose 96 happiness units by sitting next to Bob.\r\nDavid would lose 53 happiness units by sitting next to Carol.\r\nDavid would lose 30 happiness units by sitting next to Eric.\r\nDavid would lose 12 happiness units by sitting next to Frank.\r\nDavid would gain 75 happiness units by sitting next to George.\r\nDavid would lose 20 happiness units by sitting next to Mallory.\r\nEric would gain 8 happiness units by sitting next to Alice.\r\nEric would lose 89 happiness units by sitting next to Bob.\r\nEric would lose 69 happiness units by sitting next to Carol.\r\nEric would lose 34 happiness units by sitting next to David.\r\nEric would gain 95 happiness units by sitting next to Frank.\r\nEric would gain 34 happiness units by sitting next to George.\r\nEric would lose 99 happiness units by sitting next to Mallory.\r\nFrank would lose 97 happiness units by sitting next to Alice.\r\nFrank would gain 6 happiness units by sitting next to Bob.\r\nFrank would lose 9 happiness units by sitting next to Carol.\r\nFrank would gain 56 happiness units by sitting next to David.\r\nFrank would lose 17 happiness units by sitting next to Eric.\r\nFrank would gain 18 happiness units by sitting next to George.\r\nFrank would lose 56 happiness units by sitting next to Mallory.\r\nGeorge would gain 45 happiness units by sitting next to Alice.\r\nGeorge would gain 76 happiness units by sitting next to Bob.\r\nGeorge would gain 63 happiness units by sitting next to Carol.\r\nGeorge would gain 54 happiness units by sitting next to David.\r\nGeorge would gain 54 happiness units by sitting next to Eric.\r\nGeorge would gain 30 happiness units by sitting next to Frank.\r\nGeorge would gain 7 happiness units by sitting next to Mallory.\r\nMallory would gain 31 happiness units by sitting next to Alice.\r\nMallory would lose 32 happiness units by sitting next to Bob.\r\nMallory would gain 95 happiness units by sitting next to Carol.\r\nMallory would gain 91 happiness units by sitting next to David.\r\nMallory would lose 66 happiness units by sitting next to Eric.\r\nMallory would lose 75 happiness units by sitting next to Frank.\r\nMallory would lose 99 happiness units by sitting next to George.\r\n";

//Your task: find the optimal seating arrangement to ensure the highest overall dinner table happiness

//Step 1. Process the input into a neighbour vs neighbour to "cost" map, while building a list of unique persons

Dictionary<(string, string), int> neighbours2CostMap = new Dictionary<(string personA, string personB), int>();
HashSet<string> partyAttendants = new HashSet<string>();

ProcessInput ();

//Step 2. Get all possible seating arrangements:
List<List<string>> possibleSeatingArrangements = partyAttendants.ToList().GetPermutations();

//Step 3. Run both challenges
Part1();
Part2();
Console.ReadKey();

void Part1()
{
    Console.WriteLine("Part 1:"+GetBestConfigurationScore());
}

void Part2()
{
    partyAttendants.Add("Me");
    possibleSeatingArrangements = partyAttendants.ToList().GetPermutations();
    Console.WriteLine("Part 2:" + GetBestConfigurationScore());
}

void ProcessInput ()
{
    string pattern = @"(\w+) would (lose|gain) (\d+) happiness units by sitting next to (\w+)\.\r\n";
    MatchCollection matches = Regex.Matches(myInput, pattern);

    foreach (Match match in matches)
    {
        if (match.Success)
        {

            string personA = match.Groups[1].Value;
            string personB = match.Groups[4].Value;
            string action = match.Groups[2].Value;
            int value = int.Parse(match.Groups[3].Value);

            neighbours2CostMap[(personA, personB)] = (action == "gain") ? value : -value;
            partyAttendants.Add(personA);
        }
    }
}

int GetBestConfigurationScore()
{
    int bestConfiguration = int.MinValue;

    foreach (List<string> seatingArrangement in possibleSeatingArrangements)
    {
        int score = GetResult(seatingArrangement);
        bestConfiguration = Math.Max(bestConfiguration, score);
    }
	
	return bestConfiguration;
}

int GetResult (List<string> pSeatingArrangement)
{
	int total = 0;
	for (int i = 0; i < pSeatingArrangement.Count; i++)
	{
        //If there are people in the attendance list who are not in the map just use a score of zero for them (aka skip 'm)
        string personA = pSeatingArrangement[i];
        string personB = pSeatingArrangement[(i + 1) % pSeatingArrangement.Count];

        if (personA == "Me" || personB == "Me") continue;

        //Don't use exceptions to catch invalid items, since it would be very slow
	    total += neighbours2CostMap[(personA, personB)];
	    total += neighbours2CostMap[(personB, personA)];
    }

	return total;
}
