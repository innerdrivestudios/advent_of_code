// Solution for https://adventofcode.com/2016/day/2 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// ** Your input: a keypad and a bunch of Up Down Left Right instructions

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

//- Keypad for the first part...
string keypadInputPart1 = "1 2 3\r\n4 5 6\r\n7 8 9";

//- Keypad for the second part (note I've replaced the spaces with . to simplify the code
string keypadInputPart2 = ". . 1 . .\r\n. 2 3 4 .\r\n5 6 7 8 9\r\n. A B C .\r\n. . D . .";

//- The actual instructions to follow on the keypad starting from the center
//- Little bit hard to see, but these are sequences of DLRU with \r\n in between to indicate where the instructions
//  for each part of the code ends... e.g. let's say these split into 5 lines, then we'll have a code made up out of 5 characters
string testInput = "ULL\r\nRRDDD\r\nLURDL\r\nUUUUD\r\n";
string myInput = File.ReadAllText(args[0]);

//Your task: figure out the bath room code by finding which instructions make you "stuck" on a button for a single "frame"
Console.WriteLine("Test input code = " + FindCode(keypadInputPart1, '5', testInput));
Console.WriteLine("Part 1 code = " + FindCode(keypadInputPart1, '5', myInput));
Console.WriteLine("Part 2 code = " + FindCode(keypadInputPart2, '5', myInput));

string FindCode(string pKeypadToUse, char pStartingChar, string pInstructions)
{
    Grid<char> keypad = new Grid<char>(pKeypadToUse, "\r\n", " ");

    //Find the starting char
    Vec2i currentPosition = new Vec2i();
    keypad.Foreach((position, keypadChar) => { if (keypadChar == pStartingChar) currentPosition = position;});
    
    //Set up a lookup string with matching Vec2i directions
    string possibleInstructions = "DLRU";
    Vec2i[] directions = { new Vec2i(0, 1), new Vec2i(-1, 0), new Vec2i(1, 0), new Vec2i(0, -1) };

    string code = "";

    string[] instructionLines = 
        pInstructions
        .ReplaceLineEndings(Environment.NewLine)
        .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

    //For each line ...
    foreach (string instructionLine in instructionLines)
    {
        //... go through all the instructions in that line, ending up at a specific position ...
        foreach (char instruction in instructionLine)
        {
            Vec2i newDirection = directions[possibleInstructions.IndexOf(instruction)];
            Vec2i newPosition = currentPosition + newDirection;

            if (keypad.IsInside(newPosition) && keypad[newPosition] != '.')
            {
                currentPosition = newPosition;
            }
        }

        //... using that position as the next part of the code
        code += keypad[currentPosition];
    }

    return code;
}
