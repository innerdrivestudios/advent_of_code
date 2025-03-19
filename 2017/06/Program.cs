//Solution for https://adventofcode.com/2017/day/6 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: there are sixteen memory banks;
// each memory bank can hold any number of blocks.
// The goal of the reallocation routine is to balance the blocks between the
// memory banks.

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings("");

int[] banks = myInput
    .Split("\t", StringSplitOptions.RemoveEmptyEntries)
    .Select(int.Parse)
    .ToArray();

// ** Part 1:
// Count how many cycles are required before we encounter a identical bank configuration
HashSet<string> visited = new HashSet<string>();
Console.WriteLine("Part 1 - Cycles: " + GetCycleCount(banks).ToString());

// ** Part 2: The same but now from identical state to identical state
// In other words: less cycles will be required.

visited.Clear();
visited.Add(GetStringRep(banks));
Console.WriteLine("Part 2 - Cycle size:" + GetCycleCount(banks).ToString());

int GetIndexOfFullestBank(int[] pBanks)
{
  int indexOfFullestBank = -1;
  int num = int.MinValue;
  for (int index = 0; index < pBanks.Length; ++index)
  {
    int pBank = pBanks[index];
    if (pBank > num)
    {
      indexOfFullestBank = index;
      num = pBank;
    }
  }
  return indexOfFullestBank;
}

void Rearrange(int[] pBanks)
{
  int index = GetIndexOfFullestBank(pBanks);
  int pBank = pBanks[index];
  pBanks[index] = 0;
  for (; pBank > 0; --pBank)
  {
    index = (index + 1) % pBanks.Length;
    ++pBanks[index];
  }
}

string GetStringRep(int[] pBanks) => string.Join<int>("-", pBanks);

int GetCycleCount(int[] pBanks)
{
  int cycleCount = 0;
  do
  {
    Rearrange(banks);
    ++cycleCount;
  }
  while (visited.Add(GetStringRep(banks)));
  return cycleCount;
}
