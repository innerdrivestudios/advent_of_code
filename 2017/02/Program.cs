// Solution for https://adventofcode.com/2017/day/2 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: lines with sequences of numbers, e.g. like this:

// 5 1 9 5
// 7 5 3
// 2 4 6 8

// But then more :) :

string myInput = File.ReadAllText(args[0]);

// Who said LINQ is unreadable??
int[][] numbers = myInput
    //Get all the separate strings (with numbers separated by tabs) by splitting on \r\n
    .Split ("\r\n", StringSplitOptions.RemoveEmptyEntries)	
	//Split each of those lines on a \t char and convert the results to an array of integers
	.Select (x => x.Split ("\t", StringSplitOptions.RemoveEmptyEntries).Select (int.Parse).ToArray())
	//Convert all those integers arrays to an array
	.ToArray ();

// ** Part 1: Calculate the sum of the deltas between the min and max of each line and add all of them

Console.WriteLine("Part 1 - Checksum: " + numbers.Select (x => x.Max() - x.Min()).Sum());

// * Part 2: find the only two numbers in each row where one evenly divides the other
// divide them, and add up each line's result

Console.WriteLine ("Part 2 - Alternative checksum: " + GetAlternativeChecksum(numbers));

int GetAlternativeChecksum(int[][] pNumbers)
{
    int total = 0;

    //get a line with numbers
    foreach (int[] numberList in pNumbers)
    {
        //compare all the numbers in the list back and forth
        for (int j = 0; j < numberList.Length; j++)
        {
            for (int k = 0; k < numberList.Length; k++)
            {
                if (j == k) continue;

                //use int rounding to figure out if numbers divide evenly, 
                //the next calculations will only work if there are no digits after the comma
                int factor = numberList[j] / numberList[k];
                if (factor * numberList[k] == numberList[j]) total += factor;
            }
        }
    }

    return total;
}




