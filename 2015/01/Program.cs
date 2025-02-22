// Solution for https://adventofcode.com/2015/day/1 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of up and down instructions, represented by ( and )

string myInput = File.ReadAllText(args[0]);

// ** Part 1: Calculate the final floor you'll end up by following all up down instructions starting at floor 0

Console.WriteLine("Part 1 (Final floor):" + myInput.Sum(r => r == '(' ? 1 : -1));

// ** Part 2: Calculate the index of the char in the string where you hit the basement for the first time

// Follow the floor change instructions and exit when we hit the basement

int floor = 0;
int basementIndex = 1;

foreach (char c in myInput)
{
    //Alternative to part 1 to calculate -1 or 1 based on ( or )
    floor += 1 - (c - '(') * 2;

    //Exit the moment we hit the basement
    if (floor == -1) break;

    basementIndex++;
}

Console.WriteLine("Part 2 (First time basement hit):"   + basementIndex);

