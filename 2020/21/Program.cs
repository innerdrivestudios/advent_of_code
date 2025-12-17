// Solution for https://adventofcode.com/2020/day/21 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: an array of strings describing tile data (an id + newline separated hash tags)

string[] myInput = File.ReadAllLines(args[0]);

// See the worked example:

Dictionary<string, HashSet<string>> allergenToIngredientMap = new();
List<string> allIngredients = new();

foreach (var line in myInput)
{
	string[] lineParts = line.Split(" (contains ");
	string[] ingredients = lineParts[0].Split(" ", StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries);
	string[] allergens = lineParts[1].Split([" ", ",", ")",], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

	//store every allergen mapped to a list of ingredients
	foreach (string allergen in allergens)
	{
		Filter(allergen, ingredients);
	}

	allIngredients.AddRange(ingredients);
}

void Filter (string pAllergen, string[] pIngredients)
{
	// take the intersection of all ingredients that might contain a certain allergen
	if (allergenToIngredientMap.ContainsKey(pAllergen))
	{
		allergenToIngredientMap[pAllergen] = allergenToIngredientMap[pAllergen].Intersect(pIngredients).ToHashSet();
	}
	else
	{
		allergenToIngredientMap[pAllergen] = pIngredients.ToHashSet();
	}
}


HashSet<string> mappedIngredients =  new HashSet<string>();
foreach (var pair in allergenToIngredientMap)
{
	mappedIngredients.UnionWith(pair.Value);
}

Console.WriteLine("Part 1:" + allIngredients.Count (x => !mappedIngredients.Contains(x)));




foreach (var kv in allergenToIngredientMap)
{
	Console.WriteLine(kv.Key + " " + string.Join (",", kv.Value));
}

// Now order allergens in order of how many ingredients they might match:
// And while we have not deducted all ingredients, take every deducted set and use it
// to reduce the set of options for the rest...

List<(string allergen, HashSet<string> ingredients)> orderedAllergens = allergenToIngredientMap.Select (x => (x.Key, x.Value)).ToList ();

while (orderedAllergens.Any(x => x.ingredients.Count != 1))
{
	orderedAllergens.Sort((a,b) => (a.ingredients.Count - b.ingredients.Count));
	for (int i = 0; i < orderedAllergens.Count-1; i++)
	{
		for (int j = i+1; j < orderedAllergens.Count; j++)
		{
			if (orderedAllergens[i].ingredients.Count > 1) continue;

			var element = orderedAllergens[j];
			element.ingredients = element.ingredients.Except(orderedAllergens[i].ingredients).ToHashSet();
			orderedAllergens[j] = element;
		}
	}
}

// At this point every allergen is mapped to a single ingredient...
// Sort everything by allergen as requested and print the ingredients as a comma separated list

Console.WriteLine();
orderedAllergens.Sort((a, b) => a.allergen.CompareTo(b.allergen));

foreach (var kv in orderedAllergens)
{
	Console.WriteLine(kv.allergen + " " + string.Join(",", kv.ingredients));
}

Console.WriteLine("Part 2:" + string.Join(",", orderedAllergens.Select(x => x.ingredients.First())));