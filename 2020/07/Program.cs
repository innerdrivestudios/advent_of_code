// Solution for https://adventofcode.com/2020/day/7 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of bag specifications,
// telling us which bags can contain other bags

using System.Text.RegularExpressions;

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Part 1 - Find out which bags may eventually lead to a "shiny gold" bag.

// First we need to parse the input and convert the input into a directed graph.
//
// But what are the nodes and what are the edges?
//
// We can store the information in different ways:
// 1. map a bag to the bags it can contain
// 2. map a bag to the bags it can be contained by
// 3. both ways ...

// Option 2 makes the most sense here, that way we can easily recursively find all the bags
// that may eventually lead to a specific type of "final" bag (bottom up type of search).
//
// We could also use option 1 and do a bottom down type of search, but the search space would
// probably be way bigger if it ever ends at all.

string[] bagSpecifications = myInput.Split(
	Environment.NewLine, 
	StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
);

Dictionary<string, List<string>> containedBy = BagParserPart1();

Dictionary<string, List<string>> BagParserPart1()
{
	//Keep track of which bag type is contained by which bag
	Dictionary<string, List<string>> containedBy = new();
	
	//So we take everything in front of the word bag
	Regex bagParser = new Regex(@"(\w+\s\w+) bag");

	foreach (string bagSpecification in bagSpecifications)
	{
		//Then we get all matches...
		MatchCollection matches = bagParser.Matches(bagSpecification);

		//Get the containing bags
		string container = matches[0].Groups[1].Value;

		// Parse the containees

		for (int i = 1; i < matches.Count; i++)
		{
			// Get the bag possibly contained by the container
			string containee = matches[i].Groups[1].Value;

			// Look up the collection of bags possibly containing this bag
			if (!containedBy.TryGetValue(containee, out var containers))
			{
				containedBy[containee] = containers = new();
			}

			// And add the container to that list
			containers.Add(container);
		}
	}

	return containedBy;
}

// Now that we've mapped all the containers for every bag type, let's see where
// our "shiny gold" bag may lead us...

HashSet<string> FindAllContainers (string pBagType, HashSet<string> pContainers)
{
	// If the given bag type still has possible containers...

	if (containedBy.TryGetValue(pBagType, out var containers))
	{
        foreach (var container in containers)
        {
			// If we added a container, not seen before, explore that container as well
            if (pContainers.Add(container))
			{
				FindAllContainers(container, pContainers);
			}
        }
    }

	return pContainers;
}

Console.WriteLine(
	"Part 1 - Possible amount of container colors: " +
	FindAllContainers ("shiny gold", new()).Count
);

// ** Part 2 - Basically the same kind of recursive search, but now the other way around:
// top down, while also incorporating the AMOUNT of bags contained by a parent bag
// 
// So, again, we'll need to reparse our bag content, slightly more sophisticated this time...

// Keep track of which containers lead to which containers and how many...

Dictionary<string, List<(string bag, int count)>> containers = BagParserPart2();

Dictionary<string, List<(string, int)>> BagParserPart2()
{
	Dictionary<string, List<(string, int)>> containers = new();

	// Reparse the base content, this time using 2 different regexes for convenience
	Regex containingBagParser = new Regex(@"(\w+\s\w+) bags");
	Regex containedBagParser = new Regex(@"(\d+) (\w+\s\w+) bag");

	foreach (string bagSpecification in bagSpecifications)
	{
		string[] bagParts = bagSpecification.Split(" contain ");

		// First check if the given bag actually contains other bags...
		MatchCollection containedBagMatches = containedBagParser.Matches(bagParts[1]);

		if (containedBagMatches.Count > 0)
		{
			// Get the root bag
			Match containingBagMatch = containingBagParser.Match(bagParts[0]);
			string container = containingBagMatch.Groups[1].Value;

			// Make sure there is a list entry for this container
			if (!containers.TryGetValue(container, out var containees))
			{
				containers[container] = containees = new();
			}

			// Get the child bags for the given container ...
			for (int i = 0; i < containedBagMatches.Count; i++)
			{
				containees.Add(
					(
						containedBagMatches[i].Groups[2].Value,			//bag name
						int.Parse (
							containedBagMatches[i].Groups[1].Value		//bag count
						)
					)
				);
            }
        }
	}

	return containers;
}

// Now that we have that information we can start counting ;)
// following the exact same counting mechanism as outlined on the puzzle page

int FindContaineeCount (string pRootBag)
{
	int bagCount = 0;

	if (containers.TryGetValue (pRootBag, out var childBags))
	{
		foreach (var bag in childBags)
		{
			bagCount += bag.count + bag.count * FindContaineeCount(bag.bag);
        }
	}

	return bagCount;	
}

Console.WriteLine("Part 2 - Shiny gold bag count:" + FindContaineeCount("shiny gold"));
