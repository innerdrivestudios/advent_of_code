//Solution for https://adventofcode.com/2015/day/20 (Ctrl+Click in VS to follow link)

//Your input: the number of gifts we want to have at a specific house
int myInput = 29000000;

//Your task: given the weird behaviour of the present elves (see day description),
//figure out what the first house is to have more than the given amount of presents

Console.WriteLine(Part1_LowestHouseNumberToReceiveThisManyPresents(myInput));
Console.WriteLine(Part2_SameRequestButWithDifferentNumbers(myInput));

int Part1_LowestHouseNumberToReceiveThisManyPresents(int pInput)
{
    //what is the upper limit of houses to check? 
    //each elf delivers 10 times its own number so elf (pInput/10) delivers the required amount of presents at house pInput
    //BUT.... we are probably done WAY before that because of all the other elves also dropping presents
    int[] houses = new int[(pInput / 10)];

    //now for each elf...
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


