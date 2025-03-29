// Solution for https://adventofcode.com/2023/day/9 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of lists of numbers

List<List<int>> myInput = 
    File.ReadAllLines(args[0])                                  //All seperate lines
    .Select (x => x.Split (" ").Select (int.Parse).ToList())   //Split on space and parsed as int[]
    .ToList();                                                 //Converted to an []

// ** Part 1: Detect all the next numbers in the given sequences and
// add them together according to the algorithm provided

// Note: after solving this the way I did below, I checked the Reddit forums and there seem to be some
// really smart tricks that will allow you to condense and optimise the code, but to be honest, this
// way it is very readable and still fast enough so I decided to leave it this way and NOT optimise it.
// Alternative approach to solve this all the way at the bottom...

int FindNextNumberInSequence(List<int> pSequence)
{
    List<List<int>> iterations = GenerateSequenceIterationsDownToAllZeroes (pSequence);
    //Now count back adding all the deduced elements to the previous sequence
    for (int i = iterations.Count - 2; i > 0; i--)
    {
        int lastOfCurrent = iterations[i].Last();
        int lastOfPrevious = iterations[i-1].Last();

        iterations[i - 1].Add(lastOfPrevious + lastOfCurrent);
    }

    return iterations.First().Last();
}

List<List<int>> GenerateSequenceIterationsDownToAllZeroes (List<int> pSequence)
{
    List<List<int>> iterations = new List<List<int>>() { new List<int>(pSequence) };

    //First generate all the sequences until the sequence is zero
    while (iterations.Last().Any(x => x != 0))
    {
        List<int> lastIteration = iterations.Last();
        List<int> nextIteration = new List<int>();

        for (int i = 0; i < lastIteration.Count - 1; i++)
        {
            nextIteration.Add(lastIteration[i + 1] - lastIteration[i]);
        }

        iterations.Add(nextIteration);
    }

    return iterations;
}

Console.WriteLine(
    "Part 1 - Total of next numbers in all sequences:" +
    myInput.Sum (x => FindNextNumberInSequence(x)));

// ** Part 2 - Do the same but backwards in time ...

Console.WriteLine(
    "Part 2 - Total of previous numbers in all sequences:" +
    myInput.Sum(x => FindNextNumberInSequence(x.Reverse<int>().ToList())));


// Optimised approach for those interested
// (based on this reddit post: https://www.reddit.com/r/adventofcode/comments/18ef027/2023_day_9_has_a_clever_trick_to_make_the_problem/)

// 1    3    6     10    15     21                   -> original
// 1    3    6     10    15     21      0            -> modified (appended 0)
//   2     3    4      5     6     -21
//      1     1    1      1    -27 
//        0     0     0    -28    
//           0     0   -28
//             0     -28
//               -28

//          -1 * -28 = 28
