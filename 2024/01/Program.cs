// Solution for https://adventofcode.com/2024/day/1 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of numbers pairs

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// Your task: calculate difference between certain number pairs

// Part 1 :

// Pair up the smallest number in the left list with the smallest number in the right list,
// then the second-smallest left number with the second-smallest right number, and so on.

// Within each pair, figure out how far apart the two numbers are;
// you'll need to add up all of those distances.

string[] splitInput = myInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

List<int> left = new List<int>();
List<int> right = new List<int>();

foreach (string line in splitInput)
{
	string[] pairs = line.Split("   ", StringSplitOptions.None);
	left.Add(int.Parse(pairs[0]));
	right.Add(int.Parse(pairs[1]));
}

left.Sort();
right.Sort();

int result = 0;

for(int i = 0; i < left.Count; i++)
{
	result += Math.Abs(left[i] - right[i]);
}

Console.WriteLine("Part 1 - Total difference over all ordered number pairs: " + result);

// Part 2:

// This time, you'll need to figure out exactly how often each number from the left list appears
// in the right list. Calculate a total similarity score by adding up each number in the left list
// after multiplying it by the number of times that number appears in the right list.

result = 0;

for (int i = 0; i < left.Count; i++)
{
	int count = right.Count(x => x == left[i]);
	result += count * left[i];
}

Console.WriteLine("Part 2 - Appearance count: " + result);