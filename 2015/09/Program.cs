//Solution for https://adventofcode.com/2015/day/9 (Ctrl+Click in VS to follow link)

using DistanceMap = System.Collections.Generic.Dictionary<(string locationA, string locationB), int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of distances from place A to B e.g. Tristram to AlphaCentauri = 34

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Your task: to calculate the shortest/longest routes that visit each given place once (aka the Travelling Salesman Problem)

// Main approach:
//	- Setup a bidirectional distance map from (locationA, locationB) / (locationB, locationA) to a distance between them
//	- Setup a list containing all the individual unique locations mapped in the distance map
//  - Calculate all different permutations (aka orderings aka routes) of this uniqueLocations map
//  - Process the different permutations/routes to get the answers we want
//    (Note: there are faster approaches, but they're overkill for the results required)

DistanceMap distanceMap =			FillDistanceMap(myInput);
List<string> uniqueLocations =		GetUniqueLocations(distanceMap);
List<List<string>> possibleRoutes = uniqueLocations.GetPermutations();

FindShortestAndLongestRoutes (distanceMap, possibleRoutes, out int shortestRoute, out int longestRoute);

Console.WriteLine("Part 1 (Shortest route): " + shortestRoute);
Console.WriteLine("Part 2 (Longest route):  " + longestRoute);

Console.ReadKey();

DistanceMap FillDistanceMap(string pInput)
{
    StringReader reader = new StringReader(pInput);
  
	DistanceMap distanceMap = new DistanceMap();

    string line;

    while ((line = reader.ReadLine()) != null)
    {
        string[] parts = line.Split(new string[] { " = ", " to " }, StringSplitOptions.None);
        distanceMap[(parts[0], parts[1])] = int.Parse(parts[2]);	//A => B
        distanceMap[(parts[1], parts[0])] = int.Parse(parts[2]);	//B => A
    }

    return distanceMap;
}

List<string> GetUniqueLocations(DistanceMap pDistanceMap)
{
	//Use hashset to gather unique values
	HashSet<string> locations = new HashSet<string>();

	foreach (var locationPair in pDistanceMap.Keys)
	{
		//only need either A or B since the map is bidirectional
		locations.Add(locationPair.locationA);
	}

	//Return something that has order in it so we can use it to generate permutations
	return locations.ToList();
}

void FindShortestAndLongestRoutes(DistanceMap pDistanceMap, List<List<string>> pPossibleRoutes, out int pShortestRoute, out int pLongestRoute)
{
	pShortestRoute = int.MaxValue;	
	pLongestRoute = int.MinValue;	
    
	foreach (var route in pPossibleRoutes)
	{
		int distance = GetDistance (pDistanceMap, route);
		pShortestRoute = Math.Min (pShortestRoute, distance);
		pLongestRoute = Math.Max (pLongestRoute, distance);
	}
}

int GetDistance(DistanceMap pDistanceMap, List<string> pRoute)
{
    int distance = 0;

	for (int i = 0; i < pRoute.Count - 1;i++)
	{
		distance += pDistanceMap[(pRoute[i], pRoute[i + 1])];
	}

	return distance;
}