//Solution for https://adventofcode.com/2021/day/20 (Ctrl+Click in VS to follow link)

using System.Collections.Generic;
using PixelSet = (bool lit, System.Collections.Generic.HashSet<Vec2<int>> pixels);
using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a lookup table for pixels and an image that needs to be improved

// * Step 1: Parse the input and convert it into a faster format

string[] myInput = File.ReadAllText(args[0]).ReplaceLineEndings().Split (Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

// Parse the lookup table into a boolean array that tells us whether index x should light up or not
bool[] lookupTable = myInput[0].ReplaceLineEndings("").Select (x => x == '#').ToArray();

// Parse the image into a HashSet of pixels since we need to keep track of an "infinite" space...
// using the grid class as an intermediate parser
string imageData = myInput[1];
Grid<char> image = new Grid<char>(imageData, Environment.NewLine);

PixelSet pixelSet = new(true, new());
image.Foreach((pos, value) => { if (value == '#') pixelSet.pixels.Add(pos); });

// ** Part 1: Apply the imaging enhancement algorithm twice and count how many pixels are lit

// So the issue we have here, is that if index 0 of the lookup table is 1,
// every infinite unlit pixel will light up after one iteration and since infinite can't be registered,
// we would need to switch to counting the unlit pixels, until we iterate again and we have to switch
// back to counting the lit pixels etc etc. (Assuming index 511 turns everything off again ;)).
//
// Theoretically we could have a lookup table where both index 0 and 511 turn pixels on, 
// but in both the test case and our own case it is either or.

// In other words by default the darkness is infinite and we register the lit pixels,
// however after one iteration, the light is infinite and we should register the dark pixels.

//pLit          - are we registering the lit pixels?
//pInputPixels  - what are the pixels?
//pFlip         - are we flipping pLit for the result set?
PixelSet iterate(PixelSet pInput, bool pFlip)
{
    PixelSet result = (pInput.lit ^ pFlip, new());

    // Get the bounds of the current set of pixels, and offset those bounds by 1,1
    // since every pixel looks one pixel around itself
    Vec2i min = Vec2i.Min(pInput.pixels) - new Vec2i(1, 1);
    Vec2i max = Vec2i.Max(pInput.pixels) + new Vec2i(1, 1);

    // Process every pixel in the provided image
    for (int posY = min.Y; posY <= max.Y; posY++)
    {
        for (int posX = min.X; posX <= max.X; posX++)
        {
            int index = 0;

            for (int y = posY - 1; y <= posY + 1; y++)
            {
                for (int x = posX - 1; x <= posX + 1; x++)
                {
                    //always begin by making room for another bit
                    index <<= 1;

                    //Let's say input pixels contains an xy.
                    //If pLit == true, this pixel describes a lit pixel and we should NOT flip it
                    //If pLit == false, this pixel describes an unlit pixel and we should flip it
                    //because we only want to use pixels NOT in the set then
                    index |= (pInput.pixels.Contains(new Vec2i(x, y)) ^ !pInput.lit) ? 1 : 0;
                }
            }

            //So now we have a lookup value that will tell us whether pos should be lit or not
            //If it is lit and we are registering lit pixels, we should add it, and vice versa
            if (lookupTable[index] ^ !result.lit) result.pixels.Add(new Vec2i(posX, posY));
        }
    }

    return result;
}

// ** Part 1: initial setup 

for (int i = 0; i < 2; i++)
{
    pixelSet = iterate(pixelSet, lookupTable[0]);
}

Console.WriteLine("Part 1: " + pixelSet.pixels.Count);

// ** Part 2: rest

for (int i = 2; i < 50; i++)
{
    pixelSet = iterate(pixelSet, lookupTable[0]);
}

Console.WriteLine("Part 2: " + pixelSet.pixels.Count);
