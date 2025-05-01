//Solution for https://adventofcode.com/2024/day/13 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;
using Vec2l = Vec2<long>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of every machine's button behavior and prize location, e.g.
// Button A: X + 94, Y + 34
// Button B: X + 22, Y + 67
// Prize: X = 8400, Y = 5400

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings(Environment.NewLine);

Regex regex = new Regex(@"Button A: X\+(\d+), Y\+(\d+)\r\nButton B: X\+(\d+), Y\+(\d+)\r\nPrize: X=(\d+), Y=(\d+)");
MatchCollection matchCollection = regex.Matches(myInput);

// ** Part 1: What are the fewest tokens you would have to spend to win all possible prizes?

double tokens = 0;

// It costs 3 tokens to push the A button and 1 to push the B button
Vec2l costs = new Vec2l(3, 1);

// Process all machines!
foreach (Match match in matchCollection)
{
	Machine m =
		new Machine(
			new Vec2l(
				long.Parse(match.Groups[1].Value),
				long.Parse(match.Groups[2].Value)
			),
			new Vec2l(
				long.Parse(match.Groups[3].Value),
				long.Parse(match.Groups[4].Value)
			),
			new Vec2l(
				long.Parse(match.Groups[5].Value),
				long.Parse(match.Groups[6].Value)
			)
		);
	Vec2l move = m.FindCheapestMoveCombo (costs);
	if (move.Magnitude() > 0) tokens += move * costs;
}

Console.WriteLine("Part 1:" + tokens);

// ** Part 2: We need to do the same thing but now for a price that is located at a position,
//    which is a 10000000000000 higher at both the x and y axis

tokens = 0;

// Process all machines again...
foreach (Match match in matchCollection)
{
    Machine m =
        new Machine(
            new Vec2l(
                long.Parse(match.Groups[1].Value),
                long.Parse(match.Groups[2].Value)
            ),
            new Vec2l(
                long.Parse(match.Groups[3].Value),
                long.Parse(match.Groups[4].Value)
            ),
            new Vec2l(
                long.Parse(match.Groups[5].Value) + 10000000000000,
                long.Parse(match.Groups[6].Value) + 10000000000000
            )
        );

    // This won't work anymore, since it is too slow!
    // Vec2l move = m.FindCheapestMoveCombo(costs);
    
    // So we'll use ...
    //Vec2l move = m.FindCheapestMoveComboOptimized(costs);
    // Or 
    Vec2l move = m.FindCheapestMoveComboOptimizedAlternative();

    if (move.Magnitude() > 0) tokens += move * costs;
}

Console.WriteLine("Part 2:" + tokens);
