// Solution for https://adventofcode.com/2022/day/7 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of semi linux instructions

using System.IO;

string myInput = File.ReadAllText(args[0]);

string[] consoleLog = myInput
	.ReplaceLineEndings(Environment.NewLine)
	.Split (Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
	.ToArray();

// ** Step 1: Process the whole console log and build the directory tree.

// What are some of our options for building the directory tree?
// We could use a dictionary from string to fileinfo (type, size, name, List<string> children)
// But this might result in conflicts, if unrelated nested directories have duplicate names.

// Instead we'll implement a Node class that can have child nodes and a parent node,
// so that we can create a node structure, while moving up and down the directory hierarchy
// based on the commands we are "inputting", and the results the log is outputting...

// Static method to get started with a single root node ...
TreeNode current = TreeNode.GetRoot();

// Now parse the whole console log ...

foreach (string consoleLine in consoleLog)
{
	if (consoleLine.StartsWith("$ cd "))
	{
		string cdArgument = consoleLine.Substring("$ cd ".Length);
		current = current.GetNode(cdArgument);
	}
	else if (consoleLine.StartsWith("$ ls"))
	{
		//ignore ...
	}
	else if (consoleLine.StartsWith("dir"))
	{
		string directory = consoleLine.Substring("dir ".Length);
		current.CreateNode(directory);
	}
	else // the only option left
	{
		string[] fileParts = consoleLine.Split(" ");

		int fileSize = int.Parse(fileParts[0]);
		string fileName = fileParts[1];

		current.CreateNode(fileName, fileSize);
	}
}

// Now that we've parsed the whole console log, we can print the tree to see if we did it correctly
// current.GetNode("/").Print();

// ** Part 1 - Find all of the directories with a total size of at most 100000,
// then calculate the sum of their total sizes.

TreeNode root = current.GetNode("/");
root.CalculateDirectorySizes();

Console.WriteLine("Part 1 - Requested sum: " + root.TotalSumOfAllDirectoriesBelow (100000));

// ** Part 2 -
// - Find a directory to delete so that we have at 30000000 free space.
// - The total space available to the file system is 70000000

int totalDiscSpace = 70000000;
int totalSpaceRequired = 30000000;
int totalSpaceTaken = root.size;
int totalSpaceFree = totalDiscSpace - totalSpaceTaken;
int spaceToFreeUp = totalSpaceRequired - totalSpaceFree;

// Console.WriteLine("Part 2 - Space left: " + totalSpaceFree);
// Console.WriteLine("Part 2 - Space to delete: " + spaceToFreeUp);

TreeNode directoryToDelete = root.FindSmallestDirectorySized(spaceToFreeUp);
Console.WriteLine("Part 2 - Directory size to delete:" + directoryToDelete.size);
