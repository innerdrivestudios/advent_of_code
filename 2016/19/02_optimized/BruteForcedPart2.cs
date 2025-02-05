using Elf = int;

class BruteForcedPart2
{
    public static int GetSurvivingElf(int pElvesCount)
    {
        List<Elf> elves = new List<Elf>();

        for (int i = 0; i < pElvesCount; i++)
        {
            elves.Add(i + 1);
        }

        // Now keep stealing gifts from opposite elf until there is one elf left

        int elfIndex = 0;

        while (elves.Count != 1)
        {
            // To get the opposite elf we need to skip half of the remaining elves ourselves excluded

            Elf currentElf = elves[elfIndex];

            int nextIndex = (elfIndex + (elves.Count / 2)) % elves.Count;
            Elf nextElf = elves[nextIndex];

            //Console.WriteLine(currentElf + " steals from " + nextElf + " " + string.Join(",", elves));
            elves.RemoveAt(nextIndex);

            //This is a sneaky one, if we are removing an elf before or on us, we need to reduce the current elf index!
            if (nextIndex <= elfIndex) elfIndex--;

            //Then we increase it again...
            elfIndex++;
            if (elfIndex >= elves.Count) elfIndex = 0;
        }

        return elves[0];
    }
}

