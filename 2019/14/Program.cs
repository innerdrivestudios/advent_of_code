// Solution for https://adventofcode.com/2019/day/14 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of crafting instructions...

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// Step 1. Let's parse the rules first, in the format given:

// The result of which be a list of a list of Resource values.
// Each Resource value list is constructed like this:
// [Required resource 1, ..., Required resource n, Result Resource]
// (We'll process this into a more useable format below)

IEnumerable<IEnumerable<Resource>> craftingInstructions = myInput
	//First get all separate lines
	.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
	//From each line, 
	.Select(
		line => line
			//... get an array of "4 BLABLA" strings...
			.Split([",", "=>"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			//... separate "4 BLABLA" into "4" and "BLABLA"							
			.Select(resourceString => resourceString.Split(" "))
			//... and convert it into a resource ...//name				//amount
			.Select(resourceValues => new Resource(resourceValues[1], ulong.Parse(resourceValues[0]) ) )
	);

// ** Part 1: Figure out how much ORE is required for 1 FUEL

// To do this we are going to work backwards from 1 FUEL to the amount of ORE,
// using a process described in the scenario_1/2.txt files in this folder. 
//
// The first thing we'll need for this is a reverse lookup table:

// Currently we have this: [Required resource 1, ..., Required resource n, Result Resource]
// But we want: output resource name => ([input resources], output resource) 

// This allows us to map a specific resource requirement to the input required and the output delivered.
// In theory we don't even need a full "output resource", just "output amount", but for debugging
// purposes I'll leave it in there:

Dictionary<string, ProductionRule> reverseLookupTable =
	craftingInstructions.ToDictionary(
		x => x.Last().name, 
		x => new ProductionRule (x.Take(x.Count()-1).ToList(), x.Last())
	);

ulong CalculateRequiredOreFor (Resource pResource)
{
	// As described in the scenarios we need to keep track of ...
	ulong oreRequired = 0;
	Dictionary<string, ulong> toCraft = new();
	Dictionary<string, ulong> inReserve = new();

	toCraft[pResource.name] = pResource.amount;

	while (toCraft.Count > 0)
	{
		var currentItem = toCraft.First();

		// Special case, we could have also said: IF there is no production rule...
		// But if the requirement is ORE, just delete it from the list and add it up to 
		// the total amount of ore required...

		if (currentItem.Key == "ORE")
		{
			oreRequired += currentItem.Value;
			toCraft.Remove(currentItem.Key);
		}

		// If we need to craft something that is not ORE, and we are still holding some of
		// those items in reserve, use those up first ...

		else if (inReserve.ContainsKey(currentItem.Key))
		{
			// How many should we remove? The minimum of what we need and what we've got
			ulong toRemove = ulong.Min(inReserve[currentItem.Key], currentItem.Value);
			inReserve[currentItem.Key] -= toRemove;
			toCraft[currentItem.Key] -= toRemove;

			if (inReserve[currentItem.Key] == 0) inReserve.Remove(currentItem.Key);
			if (toCraft[currentItem.Key] == 0) toCraft.Remove(currentItem.Key);
		}

		// We need something that we don't hold into reserve...
		else
		{
			// Get the production rule for this item
			ProductionRule rule = reverseLookupTable[currentItem.Key];

			// Find out how many times to run this specific rule
			ulong timesToRun = (ulong)Math.Ceiling((double)currentItem.Value / rule.output.amount);

			// After running this production rule, we'll have ...
			// (we shouldn't have to add to an existing stash, since we consume that first...)
			inReserve[currentItem.Key] = rule.output.amount * timesToRun;
			
			// And add all the requirements for that resource to the requirement list
			// this time, adding to what was already there
			foreach (Resource resource in rule.input)
			{
				toCraft[resource.name] = timesToRun * resource.amount + toCraft.GetValueOrDefault(resource.name,(ulong)0);
			}
		}
	}

	return oreRequired; 
}

Console.WriteLine("Part 1:" + CalculateRequiredOreFor(new Resource("FUEL", 1)));

// ** Part 2: How much fuel can we create with this much ore?

ulong ore = 1000000000000;

// Set up an initial guess:

ulong fuel = ore / CalculateRequiredOreFor(new Resource("FUEL", 1));

// Then perform a narrowing linear search:

ulong granularity = fuel; 
ulong result = 0;
int guesses = 0;

while (true) {
	result = CalculateRequiredOreFor(new Resource("FUEL", fuel));
	guesses++;

	if (result > ore)
	{
		if (granularity > 1)
		{
			fuel -= granularity;
			granularity /= 2;
			granularity = ulong.Max(1, granularity);
		}
		else
		{
			fuel -= granularity;
			break;
		}
	}

	fuel += granularity;
}

Console.WriteLine("Part 2:"+ fuel);
Console.WriteLine("Part 2 took " + guesses + " guesses.");
