//Solution for https://adventofcode.com/2017/day/10 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a sequence of lengths

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings("");
int[] stringLengths = myInput.Split(",").Select(int.Parse).ToArray();

// ** Part 1: Perform the knot hash method described in the puzzle

// Let's start by defining some helper methods

// Basically for any given list [a,b,c,d,e,f] this method iterates over the first half
// swapping all the elements in place with the last half
// (working forwards over the first, backwards over the second)
void Reverse(int[] pInputList, int pReverseLength, int pStartIndex)
{
    //Console.WriteLine("Reversing " + string.Join(',', pInputList) + " from " + pStartIndex + " for " + pReverseLength + " elements.");
    for (int i = 0; i < pReverseLength/2;i++)
    {
        Swap(pInputList, i + pStartIndex, pStartIndex + pReverseLength - i - 1);
    }
    //Console.WriteLine("Result: " + string.Join(',', pInputList));
    //Console.ReadKey();
}

void Swap (int[] pInputList, int pIndexA, int pIndexB)
{
    //wrap both elements around the end of the list
    pIndexA %= pInputList.Length; 
    pIndexB %= pInputList.Length;
    
    //basic swap
    int tmp = pInputList[pIndexA]; 
    pInputList[pIndexA] = pInputList[pIndexB];
    pInputList[pIndexB] = tmp;
}

// Performs the algorithm as requested (added repeat count for part 2)
void KnotHash (int[] pInputList, int[] pInputLengths, int pRepeatCount = 1)
{
    int skipSize = 0;
    int currentPosition = 0;

    for (int i = 0; i < pRepeatCount; i++)
    {
        foreach (int inputLength in pInputLengths)
        {
            if (inputLength > 1 && inputLength <= pInputList.Length)
            {
                Reverse(pInputList, inputLength, currentPosition);
            }

            currentPosition += inputLength + skipSize;
            skipSize++;
        }
    }
}

int[] standardList = Enumerable.Range(0, 256).ToArray();
KnotHash(standardList, stringLengths);

Console.WriteLine("Part 1: " + (standardList[0] * standardList[1]));

// ** Part 2:
// 1. Convert your input to ASCII codes and add 17, 31, 73, 47, 23
// 2. Run your algorithms for 64 rounds
// 3. Condense the hash into a 16 number long dense hash
// 4. Convert every number to a hexadecimal code append it

// 1. Convert your input to ASCII codes and add 17, 31, 73, 47, 23
standardList = Enumerable.Range(0, 256).ToArray();
int[] convertedList = myInput.Trim().Select(x => (int)x).Concat([17,31,73,47,23]).ToArray();

// 2. Run your algorithms for 64 rounds
KnotHash(standardList, convertedList, 64);

// 3. Condense the hash into a 16 number long dense hash AND
// 4. Convert every number to a hexadecimal code append it
// (we do both in 1 fell swoop)
int[][] chunks = standardList.Chunk(16).ToArray();

string hashString = "";
for (int i = 0;i < chunks.Length; i++)
{
    hashString += chunks[i].Aggregate((x, y) => x ^ y).ToString("x2"); //note the 2 (!!!) ($@#^$@(#!!)
}

Console.WriteLine("Part 2: " + hashString);

