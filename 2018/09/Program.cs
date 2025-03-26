//Solution for https://adventofcode.com/2018/day/9 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a number of players and an amount of marbles

using System.Diagnostics;
using System.Text.RegularExpressions;

// ** Step 1: parse the input

string myInput = File.ReadAllText(args[0]);
Regex inputParser = new Regex(@"(\d+) players; last marble is worth (\d+) points");
Match match = inputParser.Match(myInput);

int playerAmount = int.Parse(match.Groups[1].Value);
int lastMarble = int.Parse(match.Groups[2].Value);

Console.WriteLine($"{playerAmount} players; last marble is worth {lastMarble} points");

// ** Part 1 implementation using a regular list

Stopwatch stopwatch = new Stopwatch(); 
stopwatch.Start();
Part1 part1 = new Part1();
Console.WriteLine("Part 1:"+part1.Run(playerAmount, lastMarble));
Console.WriteLine("Computed in " + stopwatch.ElapsedMilliseconds + " ms.");

// ** Part 2 implementation using a custom circular list

// First rerun same challenge as before to see if we implemented it correctly
Part2 part2 = new Part2();
stopwatch.Restart();
Console.WriteLine("Part 2:" + part2.Run(playerAmount, lastMarble));
Console.WriteLine("Computed in " + stopwatch.ElapsedMilliseconds + " ms.");

// Now run the actual challenge
stopwatch.Restart();
Console.WriteLine("Part 2:" + part2.Run(playerAmount, lastMarble * 100));
Console.WriteLine("Computed in " + stopwatch.ElapsedMilliseconds + " ms.");
