// Solution for https://adventofcode.com/2023/day/3 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a grid with numbers and not numbers

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

string[] lines = myInput
    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

// ** Part 1 - Locate all numbers next to a symbol and sum them

// First locate all numbers and record enough data to do a collision check later

List<(int x, int y, int w, int number)> numbers = new();
List<(int x, int y, char symbol)> symbols = new();   

for (int i = 0; i < lines.Length; i++)
{
    Regex numberParser = new Regex(@"\d+");
    Regex symbolParser = new Regex(@"[^.0-9]");

    foreach (Match match in numberParser.Matches(lines[i]))
    {
        numbers.Add ( (match.Index, i, match.Length, int.Parse(match.Value)));
    }

    foreach (Match match in symbolParser.Matches(lines[i]))
    {
        symbols.Add((match.Index, i, match.Value[0]));
    }
}

// Now that we have all number info and symbol info, we can run a double collision loop removing any number not next to a symbol
// Make sure to loop backwards so we can remove numbers as we go

for (int i = numbers.Count - 1; i >= 0; i--)
{
    bool nextToSymbol = false;
    for (int j = 0; j < symbols.Count; j++)
    {
        var number = numbers[i];
        var symbol = symbols[j];

        if (
              (
                //sneaky bit, when we know the x, the width is always at least 1, so we don't add the extra + 1
                symbol.x >= number.x - 1 && symbol.x <= number.x + number.w &&
                symbol.y >= number.y - 1 && symbol.y <= number.y + 1
              )
        )
        {
            nextToSymbol = true;
            break;
        }
    }

    if (!nextToSymbol) numbers.RemoveAt(i);
}

Console.WriteLine("Part 1 - Sum of all numbers next to a symbol: " + numbers.Sum(x => x.number));

// ** Part 2 - Very similar to part 1, but now we need to find all * symbols next to at least to numbers, 
// get its gear ratio (the two numbers in question multiplied together) and add all those ratio's
// In other words, we need to flip the loops and gather all numbers for specific symbols

int gearRatioSum = 0;
List<int> adjacents = new List<int>();

for (int j = 0; j < symbols.Count; j++)
{
    if (symbols[j].symbol != '*') continue;
    adjacents.Clear();

    for (int i = 0; i < numbers.Count; i++)
    {
        var number = numbers[i];
        var symbol = symbols[j];

        if (
            //sneaky bit, when we know the x, the width is always at least 1, so we don't add the extra + 1
            symbol.x >= number.x - 1 && symbol.x <= number.x + number.w &&
            symbol.y >= number.y - 1 && symbol.y <= number.y + 1
        )
        adjacents.Add(number.number);
        if (adjacents.Count > 2) break;
    }

    if (adjacents.Count == 2)
    {
        gearRatioSum += adjacents[0] * adjacents[1];
    }
}

Console.WriteLine("Part 2 - Gear ratio sum: " + gearRatioSum);
