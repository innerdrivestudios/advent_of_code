// Solution for https://adventofcode.com/2017/day/7 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of up and down instructions, represented by ( and )

using System.Text.RegularExpressions;

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

string[] lines = myInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

// ** Part 1: Build a dependency tree and find the root

// Some helper methods:

// Parse a line into the root, weight and child nodes (if any)
// Note we don't need the weight yet for part 1...

(string root, int weight, List<string> children) ParseLine (string pLine)
{
	string[] values = pLine.Split("->", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

	// Everything on the left side before the (xx)
	string[] leftValues = values[0].Split(" ");
	string root = leftValues[0];
	int weight = int.Parse(Regex.Match(leftValues[1], @"(\d+)").Groups[1].Value);

    List<string> children = null;
	
	// Everything on the right side after the -> if there is anything
	if (values.Length == 2)
	{
		children = new (values[1].Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
	}

	return (root, weight, children);
}

// For every child node, store the parent so we can use this info to walk up the tree later
Dictionary<string, string> parents = new();

foreach (string line in lines)
{
	var parentChildData = ParseLine(line);

	// Store the parent for every child node

	if (parentChildData.children != null) { 
		foreach (string child in parentChildData.children)
		{
			parents[child] = parentChildData.root;
		}
	}
}

// Now we know all the parents for all the nodes ...
// So we just pick A child node, doesn't matter which, all nodes lead up the tree:

string treeRoot = parents.Keys.ToList()[0];
while (parents.ContainsKey(treeRoot)) treeRoot = parents[treeRoot];

Console.WriteLine("Part 1 - The root:" + treeRoot);

// ** Part 2 - The tree is unbalanced oh noes! Which node has the wrong weight and what should its weight be?

// SO apparently, our parsing mechanism wasn't sufficient to handle part 2 ;).
// What information do we need for this part?
// - How heavy a node is...
// - What children a node has...

// We can keep two separate dictionaries for that:
Dictionary<string, List<string>> childMap = new();
Dictionary<string, int> weightMap = new();

foreach (string line in lines)
{
	var parentChildData = ParseLine(line);

	if (parentChildData.children != null)
	{
		if (!childMap.TryGetValue(parentChildData.root, out var children))
		{
			childMap[parentChildData.root] = children = new List<string>();
		}
		children.AddRange(parentChildData.children);
	}

	weightMap[parentChildData.root] = parentChildData.weight;
}

// So now we can easily find all children and we know all weights...
// But how do we find the requested info? (Which node is wrong and what its weight should be?)

// What we need to do is:
// - check all children of a given node to see if they are all the same weight or not
// - there are different options:
//  - yes
//		-> then the given node is the problem and we need to compare it with its siblings to find the difference
//  - no and there is one deviating node
//		-> repeat the process for that node
//  - no and there are more than one deviating nodes
//		-> exception, we can't fix this ...

void FindBrokenNodeWeight (string pNode, int pSiblingWeight = 0, int pDepth = 0)
{
	// If we don't have any children, there is nothing to investigate, so we are done ...
	if (childMap.TryGetValue(pNode, out var children)) {

		// Use a map from total weight to node names so we can find out whether we are dealing
		// with any deviations...
		Dictionary<int, List<string>> weightToNodesMap = new();

		// Map the total weight of each child to the child name
		for (int i = 0; i < children.Count; i++)
		{
			int weight = GetRecursiveTotalChildWeight(children[i]);

			// Make sure we have a weight entry to map to the child
			if (!weightToNodesMap.TryGetValue(weight, out var nodes))
			{
				weightToNodesMap[weight] = nodes = new();
			} 

			nodes.Add(children[i]);
		}

		// Now that the map is complete, check our situation:

		// We don't have any child weight deviations, so the current node must be the issue
		if (weightToNodesMap.Count == 1)
		{
            Console.WriteLine("I am the problem child:" + pNode);

			// But what weight should it have? 
			// Well whatever our siblings all way, without the weight of our own children...
			KeyValuePair<int, List<string>> singleWeightEntry = weightToNodesMap.First();
			// In this situation, each child has the same weight, so all children together weigh...
			int childrenWeight = singleWeightEntry.Key * singleWeightEntry.Value.Count;
			// Which means that to match the full weight of our siblings, we should weigh:
            Console.WriteLine("And my weight should have been: " + (pSiblingWeight - childrenWeight));
        }
		// There is a single deviation...
		else if (weightToNodesMap.Count == 2)
		{
			// If there is no match for this, we'll automatically get an exception, 
			// but basically we are looking for the single node that has a deviating weight
			string problemNode = weightToNodesMap.Where(x => x.Value.Count == 1).First().Value[0];
			// And at the same time we need to know what the weight should have been...
			int siblingWeight = weightToNodesMap.Where(x => x.Value.Count != 1).First().Key;
			Console.WriteLine("We need to investigate: " + problemNode);
			FindBrokenNodeWeight(problemNode, siblingWeight);
		}
		else
		{
			throw new Exception("We can't solve this...");
		}

	}
}

// This calculates the weight a node including all its children ...
int GetRecursiveTotalChildWeight (string pRoot)
{
	int weight = weightMap[pRoot];

	if (childMap.TryGetValue(pRoot, out var children))
	{
		foreach (var child in children)
		{
			weight += GetRecursiveTotalChildWeight(child);
		}
	}

	return weight;
}

Console.WriteLine();
Console.WriteLine("Part 2 - Find the broken child ;):");
FindBrokenNodeWeight(treeRoot);


