// Solution for https://adventofcode.com/2018/day/7 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of dependencies forming a directed graph

using System.Text.RegularExpressions;

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** First we need to parse the input and convert the input into a directed graph
// How can we store a directed graph? Simply using a Dictionary from node to List<node>

// Theoretically, using that graph we can find which steps need to be completed before the next...
// and also which steps another step depends on, but in practice it is easier to 
// maintain another dictionary for that, in other words we'll have to dictionaries...

Dictionary<char, List<char>> enables = new();		//opens up new nodes
Dictionary<char, List<char>> dependsOn = new();     //tells us what needs to be completed first

// Now we simply run through all given lines first and fill up these tables

// First define a helper method to clean up our code:

List<char> GetNodeList (Dictionary<char, List<char>> pTable, char pNode) 
{
	if (!pTable.TryGetValue(pNode, out List<char> nodeList)) { 
		pTable [pNode] = nodeList = new (); 
	}
	return nodeList;
}

Regex stepParser = new Regex(@"Step (.) must be finished before step (.) can begin.");
string[] steps = myInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

foreach (string step in steps)
{
	Match match = stepParser.Match(step);
	//Value is a string, Value[0] its first char...
	GetNodeList(enables, match.Groups[1].Value[0]).Add(match.Groups[2].Value[0]);
	GetNodeList(dependsOn, match.Groups[2].Value[0]).Add(match.Groups[1].Value[0]);
}

// How can we find the root node?
// Well, there must be a single node that enables other nodes, but doesn't depends on anything...

List<char> roots = enables.Keys.Where(x => !dependsOn.ContainsKey(x)).ToList();
roots.Sort();
Console.WriteLine("Root count:" + roots.Count + " => " + string.Concat(roots));

// Whoops ... multiple roots :)

// So... what is the actual path we choose?
// Basically, every node in roots can lead to a new set of nodes as long as all their dependencies
// are visited first. Given a set of nodes whose dependencies have been visited,
// we need to follow them in order to build up a path, while there are still nodes to explore...
// (Note that we kinda lazily capture some global variables in the process...)

string GetPath ()
{
	HashSet<char> visited = new();
	PriorityQueue<char, int> todo = new();
	string pathSoFar = "";

	// We want chars to be picked in order, so we'll just queue them based on their char code
	foreach (char c in roots) todo.Enqueue(c, c);

	while (todo.Count > 0)
	{
		char current = todo.Dequeue();
		visited.Add(current);
		pathSoFar += current;

		// No new nodes that are enabled by this one? Cool, we're done adding nodes...
		if (!enables.ContainsKey(current)) continue;

		List<char> newOptions = enables[current];

		// If there are new nodes, the question is: can we already add them for processing?
		// That depends on whether we've already visited all their dependencies...

		foreach (char newOption in newOptions)
		{
			if (visited.Contains(newOption)) continue;

			// Get the dependencies
			List<char> dependencies = dependsOn[newOption];

			// And check if all dependencies are already visited :)
			if (dependencies.Intersect(visited).Count() == dependencies.Count) todo.Enqueue(newOption, newOption);
		}
	}

	return pathSoFar;
}

Console.WriteLine("Part 1 - Path: " + GetPath());

// ** Part 2 - With 5 workers, 60 seconds to queue a step + its char value (A=1--Z=26)...
// How long does it takes to process all steps?
// Note that in the example C only takes 3 seconds, but in the actual example C would take 63 seconds...
//
// In other words ... it seems like we need to modify the initial path searching/finding algorithm
// to take into account the work being done as we move from step to step ...

// For example, as we queue work, we can say:
// - work to be done ... -> dictionary of (char, time required)
// - work in progress ... -> dictionary of max 5 items currently being processed...
// - every time an item is done being processed we can continue the search/find algorithm
// - we use a dictionary because this allows us to easily map the chars in the original algorithm 
//   to the items that need to be processed...

int GetPathTime()
{
	int timeSoFar = 0;

	HashSet<char> visited = new();
	PriorityQueue<char, int> todo = new();

	// We want chars to be picked in order, so we'll just queue them based on their char code
	foreach (char c in roots) todo.Enqueue(c, c);

	// This is an addition to the previous part, we'll have a "queue" items need to go through 
	// before we consider them done ...
	Dictionary<char, int> processingQueue = new();

	// We changed this to "while there are still items left on the todo list or items to process from the queue"
	while (todo.Count > 0 || processingQueue.Count > 0)
	{
		// first add items from the todo list to the processing queue if there is space available
		// we've got space for a max of 5 workers ...

		while (processingQueue.Count < 5 && todo.Count > 0)
		{
			char current = todo.Dequeue();

			// This is the time equation..
			int timeTaken = current - 'A' + 1 + 60;
			processingQueue[current] = timeTaken;
		}

		// Now we will process all items in the queue, one tick at a time
		List<char> itemsToProcess = processingQueue.Keys.ToList();

		foreach (var item in itemsToProcess)
		{
			processingQueue[item]--;

			if (processingQueue[item] == 0)
			{
				processingQueue.Remove(item);

				// This is also a (tricky) change from part 1, items are now considered done, 
				// AFTER they've been through the processing queue

				visited.Add(item);
				
				// This is all the old code from part 1, now only triggered by an item being processed...

				// No new nodes that are enabled by this one? Cool, we're done adding nodes...
				if (!enables.ContainsKey(item)) continue;

				List<char> newOptions = enables[item];

				// If there are new nodes, the question is: can we already add them for processing?
				// That depends on whether we've already visited all their dependencies...

				foreach (char newOption in newOptions)
				{
					if (visited.Contains(newOption)) continue;

					// Get the dependencies
					List<char> dependencies = dependsOn[newOption];

					// And check if all dependencies are already visited :)
					if (dependencies.Intersect(visited).Count() == dependencies.Count) todo.Enqueue(newOption, newOption);
				}
			}
		}

		// Every tick/time through the loop increase the processing time by one
		// Another tricky, we can't just sum the times for all items being processed, 
		// since they are being processed in parallel

		timeSoFar++;
	}

	return timeSoFar;
}

Console.WriteLine("Part 2 - Path time: " + GetPathTime());
