//Solution for https://adventofcode.com/2015/day/9 (Ctrl+Click in VS to follow link)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DistanceMap = System.Collections.Generic.Dictionary<(string locationA, string locationB), int>;

//Your input: a list of distances from place A to B
string myInput = "Tristram to AlphaCentauri = 34\r\nTristram to Snowdin = 100\r\nTristram to Tambi = 63\r\nTristram to Faerun = 108\r\nTristram to Norrath = 111\r\nTristram to Straylight = 89\r\nTristram to Arbre = 132\r\nAlphaCentauri to Snowdin = 4\r\nAlphaCentauri to Tambi = 79\r\nAlphaCentauri to Faerun = 44\r\nAlphaCentauri to Norrath = 147\r\nAlphaCentauri to Straylight = 133\r\nAlphaCentauri to Arbre = 74\r\nSnowdin to Tambi = 105\r\nSnowdin to Faerun = 95\r\nSnowdin to Norrath = 48\r\nSnowdin to Straylight = 88\r\nSnowdin to Arbre = 7\r\nTambi to Faerun = 68\r\nTambi to Norrath = 134\r\nTambi to Straylight = 107\r\nTambi to Arbre = 40\r\nFaerun to Norrath = 11\r\nFaerun to Straylight = 66\r\nFaerun to Arbre = 144\r\nNorrath to Straylight = 115\r\nNorrath to Arbre = 135\r\nStraylight to Arbre = 127";

//Your task: to calculate the shortest/longest routes that visit each given place once (aka the Travelling Salesman Problem)

//Main approach:
//	- Setup a bidirectional distance map from (locationA, locationB) / (locationB, locationA) to a distance between them
//	- Setup a list containing all the individual unique locations mapped in the distance map
//  - Calculate all different permutations (aka orderings aka routes) of this uniqueLocations map
//  - Process the different permutations/routes to get the answers we want
//    (Note: there are faster approaches, but they're overkill for the results required)

DistanceMap distanceMap =			FillDistanceMap(myInput);
List<string> uniqueLocations =		GetUniqueLocations(distanceMap);
List<List<string>> possibleRoutes = GetPermutations(uniqueLocations);

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

List<List<string>> GetPermutations (List<string> pList)
{
	//If there is only 1 element in the given list, we are done
	if (pList.Count == 1)
	{
		return new List<List<string>>() { pList };
	}
	else 
	{
        //For each element i in the list, get every permutations of the given list MINUS i
		//And add i back into each result... e.g. 1,2,3 -> add 1 to {2,3} & {3,2}, add 2 to {1,3} & {3,1}, etc

        List<List<string>> permutations = new List<List<string>>();

		for (int i = 0; i < pList.Count; i++)
		{
			List<string> subListToPermutate = new List<string>(pList);
			subListToPermutate.RemoveAt(i);

            List<List<string>> permutatedSubLists = GetPermutations(subListToPermutate);

			foreach (var subList in permutatedSubLists)
			{
				subList.Add(pList[i]);
				permutations.Add(subList);			
			}

		}

		return permutations;
	}
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