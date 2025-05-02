//Solution for https://adventofcode.com/2018/day/14 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input will be used by going to Debug/Debug Properties
// and specifying a value as a command line argument, e.g. 123456 
// This value will be passed to the built-in args[0] variable

// ** Your input: 

int myInput = int.Parse(args[0]);

// ** Part 1: Get the 10 scores after the nr of recipes in your puzzle input.

List<int> recipes = [3,7];
int elf1Index = 0;  
int elf2Index = 1;

void Bake()
{
    int newRecipe = recipes[elf1Index] + recipes[elf2Index];

    int recipe1 = newRecipe % 10;
    int recipe2 = (newRecipe / 10) % 10;

    if (recipe2 > 0) recipes.Add(recipe2);
    recipes.Add(recipe1);

    elf1Index = (elf1Index + 1 + recipes[elf1Index]) % recipes.Count;
    elf2Index = (elf2Index + 1 + recipes[elf2Index]) % recipes.Count;
}

void BakeUntilWeHaveXRecipes (int pRecipeCount)
{
    while (recipes.Count < pRecipeCount)
    {
        Bake();
    }
}

BakeUntilWeHaveXRecipes(myInput+10);

Console.WriteLine("Part 1 - Test after 9:" + string.Concat(recipes.GetRange(9, 10)));
Console.WriteLine("Part 1 - Test after 5:" + string.Concat(recipes.GetRange(5, 10)));
Console.WriteLine("Part 1 - Test after 18:" + string.Concat(recipes.GetRange(18, 10)));
Console.WriteLine("Part 1 - Test after 2018:" + string.Concat(recipes.GetRange(2018, 10)));
Console.WriteLine("Part 1 - Test after myInput:" + string.Concat(recipes.GetRange(myInput, 10)));

// Part 2 : How many recipes are there on our scoreboard before string.Concat(recipes(i,myInput.ToString().Lenght) == myInput?

// First gonna try and simply bruteforce this is a slow but simple way...

string inputAsString = myInput.ToString();

Console.WriteLine("\nLooking for:" + inputAsString);

int before = 0;

while (true) {
    string stringFound = string.Concat(recipes.GetRange(before, inputAsString.Length));
    if (stringFound == inputAsString) break;

    before++;
    while (before + inputAsString.Length > recipes.Count) Bake();
}

Console.WriteLine("Part 2: "+before);

// This was clearly fast enough... but can we speed it up a little bit with some minor adjustments?

Console.WriteLine("\nLooking for:" + inputAsString);

int[] valuesToLookFor = inputAsString.Select (x => x -'0').ToArray ();

before = 0;

while (true)
{
    bool allFound = true;
    for (int i = 0; i < valuesToLookFor.Length; i++)
    {
        if (valuesToLookFor[i] != recipes[before+i]) { allFound = false; break; }
    }

    if (allFound) break;
    before++;
    while (before + valuesToLookFor.Length > recipes.Count) Bake();
}

Console.WriteLine("Part 2 Optimized:" + before);
