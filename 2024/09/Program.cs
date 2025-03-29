//Solution for https://adventofcode.com/2024/day/9 (Ctrl+Click in VS to follow link)

using System.Diagnostics;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a sequence of digits like "7951869951.... etc representing a file system
// on which we need to run some operations

string myInput = File.ReadAllText(args[0]);
int[] myInputAsInts = myInput.Select((x) => x-'0').ToArray();

// ** Part 1:
// - Map out the compressed format to a disc map describing where files are and where free space is
// - Move fileblocks to the left most free space
// - Calculate the new checksum

List<int> discLayout = new List<int>();
List<int> emptyBlockIndex = new List<int>();

CreateDiscLayout();
MoveBlocksPart1();

Console.WriteLine("Part 1: " + CalculateChecksum());

// Step 1 Create the whole disc layout ...

void CreateDiscLayout ()
{
	discLayout.Clear();
	emptyBlockIndex.Clear();

    int currentId = 0;
    int inputElement = 0;

    while (inputElement < myInputAsInts.Length)
    {
        //Toggle between elements on the disk and ....
        if (inputElement % 2 == 0)
        {
            for (int i = 0; i < myInputAsInts[inputElement]; i++)
            {
                discLayout.Add(currentId);
            }
            currentId++;
        }
        else //blocks of free space
        {
            for (int i = 0; i < myInputAsInts[inputElement]; i++)
            {
                emptyBlockIndex.Add(discLayout.Count);
                discLayout.Add(-1);
            }
        }
        inputElement++;
    }
}


// Step 2 - Now we know the exact id's of every file blocks and where all the free spaces are
// and we can start moving blocks from the end of the disc to the first free space etc

void MoveBlocksPart1()
{
	int endIndex = discLayout.Count - 1;

	while (emptyBlockIndex.Count > 0 && emptyBlockIndex[0] <= endIndex)
	{
        //Move block from the end to the empty block index at the start
        discLayout[emptyBlockIndex[0]] = discLayout[endIndex];
        //Clear the empty block
        emptyBlockIndex.RemoveAt(0);
        //And mark moved block as empty
        discLayout[endIndex] = -1;

		//Skip any empty blocks at the end
		while (discLayout[endIndex] == -1) endIndex--;
	}
}

// Step 3 Calculate the new checksum

long CalculateChecksum()
{

	long total = 0;
	for (int i = 0; i < discLayout.Count; i++)
	{
		if (discLayout[i] < 0) continue;
		total += discLayout[i] * i;
	}

	return total;
}

// ** Part 2: Use another method of compressing space...

CreateDiscLayout();
MoveBlocksPart2();
Console.WriteLine("Part 2: " + CalculateChecksum());

void MoveBlocksPart2()
{
	//first gather all sequential free blocks, from the start of the emptyblock index list
	List<FreeBlock> freeBlocks = new List<FreeBlock>();

	while (emptyBlockIndex.Count > 0)
	{
		int freeBlockStartIndex = emptyBlockIndex[0];
		int freeBlockEndIndex = freeBlockStartIndex;
		emptyBlockIndex.RemoveAt(0);

		while (emptyBlockIndex.Count > 0 && emptyBlockIndex[0] == freeBlockEndIndex + 1)
		{
			freeBlockEndIndex++;
			emptyBlockIndex.RemoveAt(0);
		}

		freeBlocks.Add(new FreeBlock(freeBlockStartIndex, freeBlockEndIndex));
	}

	//now start at the end looking for blocks of numbers (files) we can move...
	int endIndex = discLayout.Count - 1;

	while (endIndex > 0)
	{
		//if it is not a number (BUT A FREEEE MAN), skip it...
		int currentFileID = discLayout[endIndex];
		if (currentFileID == -1)
		{
			endIndex--;
			continue;
		}

		//find the length of the file/block we need to move
		int blockLength = 1;
		while (endIndex - blockLength > 0 && discLayout[endIndex - blockLength] == currentFileID) blockLength++;

		//find the first free block that matches
		FreeBlock freeBlock = null;
		for (int i = 0; i < freeBlocks.Count; i++)
		{
			if (freeBlocks[i].size >= blockLength)
			{
				freeBlock = freeBlocks[i];
				break;
			}
		}

		//if there is no freeblock or the start index is past the block we want to move, skip the whole block
		if (freeBlock == null || freeBlock.startIndex > endIndex)
		{
			endIndex -= blockLength;
		}
		else
		{
			//otherwise move the whole block to the freeblock, since all the file ids are the same
			//we can do + i and - i interchangeably
			for (int i = 0; i < blockLength; i++)
			{
				discLayout[freeBlock.startIndex + i] = discLayout[endIndex - i];
				discLayout[endIndex - i] = -1;
			}

			//move the end index past the block length
			endIndex -= blockLength;
			//and update the freeblock start
			freeBlock.startIndex += blockLength;
			//but if its size is 0 or negative now we remove it
			if (freeBlock.size <= 0) freeBlocks.Remove(freeBlock);
		}
	}
}


