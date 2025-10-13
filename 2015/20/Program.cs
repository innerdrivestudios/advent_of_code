// Solution for https://adventofcode.com/2015/day/20 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input will be used by going to Debug/Debug Properties
// and specifying the value as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable.

// ** Your input: the number of gifts we want to have at a specific house

int myInput = int.Parse(args[0]);

// ** Part 1: Given the weird behaviour of the present elves (see day description),
// figure out what the first house is to have more than the given amount of presents

int Part1_LowestHouseNumberToReceiveThisManyPresents(int pInput)
{
    // What is the upper limit of houses to check? 

    // Each elf delivers 10 times its own number to each house number of which the elf number is a factor,
    // e.g. Elf 1 delivers 10 packages at House 1, 2, 3, 4, 5
    //      Elf 2 delivers 20 packages at House 2, 4, 6, 8, 10
    // Etc
    //
    // So elf (pInput/10) delivers the required amount of presents at house pInput BUT....
    // we are probably done WAY before that because of all the other elves also dropping presents there
    int[] houses = new int[(pInput / 10)];

    // Now for each elf... (turns out this is actually extremely fast...)
    for (int elf = 1; elf < houses.Length; elf++)
    {
        //make the elf drops its presents at each house
        for (int houseToVisit = elf; houseToVisit < houses.Length; houseToVisit += elf)
        {
            houses[houseToVisit] += elf * 10;
        }

        //if the current house the last elf visited has more then the requested input we are done!
        if (houses[elf] > myInput) return elf;
    }

    return -1;
}

Console.WriteLine("Part 1: " + Part1_LowestHouseNumberToReceiveThisManyPresents(myInput));

// ** Part 2: Same request but with different numbers...

int Part2_SameRequestButWithDifferentNumbers(int pInput)
{
    int[] houses = new int[pInput / 11];
    
    for (int elf = 1; elf < houses.Length; elf++)
    {
        for (
            int houseToVisit = elf, housesVisited = 0; 
            houseToVisit < houses.Length && housesVisited < 50; 
            houseToVisit += elf, housesVisited++
        )
        {
            houses[houseToVisit] += elf * 11;
        }

        //if the current house the last elf visited has more then the requested input we are done!
        if (houses[elf] > myInput) return elf;
    }

    return -1;
}

Console.WriteLine("Part 2: "+ Part2_SameRequestButWithDifferentNumbers(myInput));


