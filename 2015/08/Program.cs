//Solution for https://adventofcode.com/2015/day/8 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of weirdly formatted/escaped strings

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

string[] codedStrings = myInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

// Your task: to unescape/escape the given strings and count the changes
// in the amount of chars used to express the strings

Part1_UnescapeDifferences(codedStrings);
Part2_EscapeDifferences(codedStrings);
Console.ReadKey();

void Part1_UnescapeDifferences(string[] pInput)
{
    int totalDifference = 0;

    foreach (string codedString in codedStrings)
    {
        //option 1: transform the strings and see how many chars we save
        string processedLine = codedString.Substring(1, codedString.Length - 2);        //take out the first and last quotes
        processedLine = processedLine.Replace("\\\"", "\"");                            //replace any escaped " with the actual "
        processedLine = processedLine.Replace("\\\\", "\\");                            //replace any escaped \ with the actual
        processedLine = Regex.Replace(processedLine, @"\\x[0-9a-fA-F]{2}", "x");        //replace any \xAA sequence with a single char

        totalDifference += (codedString.Length - processedLine.Length);
    }

    Console.WriteLine("Part 1:"+totalDifference);
}

void Part2_EscapeDifferences(string[] pInput)
{
    int totalDifference = 0;

    foreach (string codedString in codedStrings)
    {
        //option 2: if we just follow the rules of what the string WOULD become
        //we don't even have to actually process the strings to count the changes
        totalDifference += codedString.Count(a => a == '\"'); //every " (note " is escaped as well) becomes \"
        totalDifference += codedString.Count(a => a == '\\'); //every \ (note \ is escaped as well) becomes \\
        totalDifference += 2;                               //every start and end " becomes \"
    }

    Console.WriteLine("Part 2:"+totalDifference);
}



