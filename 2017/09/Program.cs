//Solution for https://adventofcode.com/2017/day/9 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a horrible jumble of text that needs to be parsed ;)

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings("");

// ** Part 1 & 2:
//
// - Part 1: Count/score the amount of groups...
// - Part 2: Count how much garbage we have...
//
// In other words, the solution below is a little bit more complicated than JUST part 1, 
// since after completing part 1, I refactored it to include the solution for part 2 as well.
// Everything related to part 1 uses the result.score field
// Everything related to part 2 uses the result.garbageCount field

(int score, int garbageCount) ParseStream (string pInput)
{
    // We scan through the string as we are parsing it, using a changeable index
    int index = 0;
	(int score, int garbageCount) result = ParseGroup(pInput, ref index, 0);
    return result;
}

(int score, int garbageCount) ParseGroup (string pInput, ref int pStartIndex, int pDepth = 0)
{
    //Local result for this method call
	(int score, int garbageCount) result = (0,0);
    
    if (pInput[pStartIndex] != '{') throw new InvalidDataException("Was expecting { token");

    pStartIndex++;

    while (pInput[pStartIndex] != '}')
    {
        //after opening brace we can get a group or garbage, separated by ,
        if (pInput[pStartIndex] == '{')
        {
            //get the results for any nested groups and add their score to our own
            var nestedGroupResult = ParseGroup(pInput, ref pStartIndex, pDepth + 1);
            result.score += nestedGroupResult.score;
            result.garbageCount += nestedGroupResult.garbageCount;
        }
        else //only update the garbage count results
        {
            var localResult = ParseGarbage(pInput, ref pStartIndex, pDepth + 1);
            result.garbageCount += localResult;
        }

        //if there is a comma, simply consume it
        //(note we don't exclude parsing invalid strings, we assume string is in the correct format)
        if (pInput[pStartIndex] == ',') pStartIndex++;
    }

    if (pInput[pStartIndex] != '}') throw new InvalidDataException("Was expecting } token");

    //for every group (e.g. 1 call to ParseGroup, the call at this level)
    //we score points based on the current group depth
    result.score += (pDepth + 1);

    pStartIndex++;

    return result;
}

int ParseGarbage (string pInput, ref int pStartIndex, int pDepth = 0)
{
    int garbageCount = 0;

    if (pInput[pStartIndex] != '<') throw new InvalidDataException("Was expecting < token");

    pStartIndex++;

    while (pInput[pStartIndex] != '>')
    {
        //Skip ! + 1 char, without scoring
        if (pInput[pStartIndex] == '!')
        {
            pStartIndex++;
            pStartIndex++;
        }
        else //Skip 1 char and score it as garbage
        {
            pStartIndex++;
            garbageCount++;
        }
    }

    if (pInput[pStartIndex] != '>') throw new InvalidDataException("Was expecting } token");

    pStartIndex++;

    return garbageCount;
}

(int score, int garbageCount) = ParseStream(myInput);

Console.WriteLine("Part 1:" + score);
Console.WriteLine("Part 2:" + garbageCount);