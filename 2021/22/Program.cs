//Solution for https://adventofcode.com/2021/day/22 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;
using Vec3i = Vec3<int>;
using Box = (Vec3<int> c1, Vec3<int> c2);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Step 1: Parse the input ...

string[] myInput = File.ReadAllLines(args[0]);

// ** Part 1: Find the count of cubes that are on...

Regex instructionParser = new Regex(@"(on|off) x=(-?\d+)..(-?\d+),y=(-?\d+)..(-?\d+),z=(-?\d+)..(-?\d+)");
HashSet<Vec3i> on = new();

void ProcessInstruction (string pInstruction, int pRange)
{
    Match match = instructionParser.Match(pInstruction);
    if (!match.Success) throw new Exception("Could not parse:" + pInstruction);

    Func<Vec3i, bool> func = match.Groups[1].Value == "on" ? on.Add : on.Remove;

    int minX = int.Max(int.Parse(match.Groups[2].Value), -pRange);
    int maxX = int.Min(int.Parse(match.Groups[3].Value), pRange);
    int minY = int.Max(int.Parse(match.Groups[4].Value), -pRange);
    int maxY = int.Min(int.Parse(match.Groups[5].Value), pRange);
    int minZ = int.Max(int.Parse(match.Groups[6].Value), -pRange);
    int maxZ = int.Min(int.Parse(match.Groups[7].Value), pRange);

    for (int x = minX; x <= maxX; x++)
    {
        for (int y = minY; y <= maxY; y++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                func(new Vec3i(x, y, z));
            }
        }
    }
}

foreach (string instruction in myInput)
{
    ProcessInstruction(instruction, 50);
}

Console.WriteLine("Part 1: " +on.Count);

// ** Part 2: Same as part 1 but then to the extreme... OF COURSE IT IS :)

// So... storing Vec3 coordinates to mark the coordinates that are on is no longer feasible.
//
// Other approach:
// - We'll only store the coordinates of Axis Aligned Boxes that indicate regions that are ON.
//   This way the answer becomes a simple iterate-over-all-boxes-and-sum-their areas...
//
// This is not without issues of course, because the areas we are getting in our input data,
// although box shaped, might overlap. Which will definitely cause mis-counting if we count
// the overlapping areas twice or more.
// Similarly turning a big box on, and a smaller box within it off, etc...
//
// So how do we deal with that? 
//
// Idea 1:
// - start with an empty space, add the first on cube and its area 
// - when another cube is turned on, add its area but subtract the overlap area
// - now let's say we turn off a cube... what then? we need to subtract the overlap...
//   but what if it is overlapping with two cubes that where already overlapping...
// - this sounds like a debugging nightmare, apart from if it is even feasible...
//
// Idea 2:
// - again we start with an empty space and add the first on cube and its area...
// - but now when we process a new cube, we get all existing overlapping cubes and
//   we use the 6 planes defined by the added new cubes to slice those overlapping cubes.
// - imagine a BIG cube that is on, with a small cube that is OFF which fits completely within the big cube.
// - if we slide the BIG cube using the 6 side planes of the small cube, we COULD end up with 28 new cubes:
//   - 27 cubes from the original big cube being diced and sliced
//   - 1 cube that was the small cube being added
// - in practice if we keep track of what we've cut off (no longer overlapping) and the remaining area we still
//   have to slice we'll probably end up with less cubes then this.
// - in other words, we keep cutting off bits of any overlapping cube until the remainder is encompassed by the added cube.
//   Whatever remained is left is then deleted from the collection of ON cubes.
// - last but not least we need to look at the state of the added cube:
//      - if that cube was OFF we are done, 
//      - if that cube was ON we add it to our list...

// Time to start defining some helper methods...

bool Overlaps (Box pBoxA, Box pBoxB)
{
    // How do we test for overlap of a 3D AABB (Axis Aligned Bounding Box), with integer coordinates?
    //
    // Let's look at a 1D line first instead of an 2D or 3D AABB box.
    //
    // L1MinX ----------- L1MaxX
    //          L2MinX-------------L2MaxX
    //
    // If we move L1MinX to the right past L2MaxX we no longer overlap, so L1MinX <= L2MaxX
    // If we move L1MaxX to the left past L2MinX we also no longer overlap, L1MaxX >= L2MinX 
    // Note that if we do this from the perspective of L2 we get the same results.
    // (e.g. if we move L2MinX to the right past L1MaxX we no longer overlap, so L2MinX <= L1MaxX, which is what we already had).
    //
    // Ok nice, this is 1D, how about 2D or 3D?
    // This is a very simple extension, in order to overlap in 2 or 3 dimensions, the boxes need to overlap simultaneously on all axis.

    // In addition, the puzzle input is verified, to check that for each box c1 is less then c2 on all axis.

    return pBoxA.c1.X <= pBoxB.c2.X && pBoxA.c2.X >= pBoxB.c1.X &&
            pBoxA.c1.Y <= pBoxB.c2.Y && pBoxA.c2.Y >= pBoxB.c1.Y &&
            pBoxA.c1.Z <= pBoxB.c2.Z && pBoxA.c2.Z >= pBoxB.c1.Z;
}

// When we overlap, we need to cut, but ONLY if we are not fully enveloping the cube we are overlapping with,
// since in that case, we can simply remove that WHOLE target cube and optionally replace it with ourselves (if we are on).

// We approach this similarly to Overlaps:
//
//          L1MinX-------------L1MaxX
//              L2MinX --- L2MaxX
//
// where L1 contains L2 if L2MinX >= L1MinX && L2MaxX <= L1MaxX

bool Contains(Box pOuter, Box pInner)
{
    return pInner.c1.X >= pOuter.c1.X && pInner.c2.X <= pOuter.c2.X &&
           pInner.c1.Y >= pOuter.c1.Y && pInner.c2.Y <= pOuter.c2.Y &&
           pInner.c1.Z >= pOuter.c1.Z && pInner.c2.Z <= pOuter.c2.Z;
}

// Now we need to define 6 plane cutting methods.
// In theory there might be someway to generalize this to a generic plane that can cut boxes whatever way you please, 
// but in practice defining 6 methods (since most of it is almost copy paste anyway) is easier to handle.

// The RightSideSlice takes the max X of the cutting box as a parameter and returns both the remainder on the left,
// and the part that we cut off on the right. This method should only be called IF our cutting box has an overlap with
// the box to cut. The Remainder has to be checked again for overlap/envelopment, the sliced off part can be added back 
// to the set of on cubes.
// This method returns true IF the pBoxToCut was actually sliced.

bool LeftSideSlice(Box pCuttingBox, Box pBoxToCut, out Box pRemainder, out Box pSlicedOff)
{
    pRemainder = pBoxToCut;
    pSlicedOff = default;

    if (pCuttingBox.c1.X < pBoxToCut.c1.X || pCuttingBox.c1.X > pBoxToCut.c2.X) return false;

    // Start with setting both of them to the input...
    pSlicedOff = pBoxToCut;

    // Now adjust the left side of the remainder... inclusive...
    pRemainder.c1.X = pCuttingBox.c1.X;
    // And the right side of the sliced off part... exclusive...
    pSlicedOff.c2.X = pCuttingBox.c1.X - 1;

    return true;
}

bool RightSideSlice (Box pCuttingBox, Box pBoxToCut, out Box pRemainder, out Box pSlicedOff)
{
    pRemainder = pBoxToCut;
    pSlicedOff = default;

    if (pCuttingBox.c2.X < pBoxToCut.c1.X || pCuttingBox.c2.X > pBoxToCut.c2.X) return false;

    // Start with setting both of them to the input...
    pSlicedOff = pBoxToCut;

    // Now adjust the right side of the remainder... inclusive...
    pRemainder.c2.X = pCuttingBox.c2.X;
    // And the left side of the sliced off part... exclusive...
    pSlicedOff.c1.X = pCuttingBox.c2.X + 1;

    return true;
}


bool BottomSideSlice(Box pCuttingBox, Box pBoxToCut, out Box pRemainder, out Box pSlicedOff)
{
    pRemainder = pBoxToCut;
    pSlicedOff = default;

    if (pCuttingBox.c1.Y < pBoxToCut.c1.Y || pCuttingBox.c1.Y > pBoxToCut.c2.Y) return false;

    pSlicedOff = pBoxToCut;

    // Remainder keeps the upper part (inclusive)
    pRemainder.c1.Y = pCuttingBox.c1.Y;
    // Sliced-off part is below (exclusive)
    pSlicedOff.c2.Y = pCuttingBox.c1.Y - 1;

    return true;
}

bool TopSideSlice(Box pCuttingBox, Box pBoxToCut, out Box pRemainder, out Box pSlicedOff)
{
    pRemainder = pBoxToCut;
    pSlicedOff = default;

    if (pCuttingBox.c2.Y < pBoxToCut.c1.Y || pCuttingBox.c2.Y > pBoxToCut.c2.Y) return false;

    pSlicedOff = pBoxToCut;

    // Remainder keeps the lower part (inclusive)
    pRemainder.c2.Y = pCuttingBox.c2.Y;
    // Sliced-off part is above (exclusive)
    pSlicedOff.c1.Y = pCuttingBox.c2.Y + 1;

    return true;
}

bool BackSideSlice(Box pCuttingBox, Box pBoxToCut, out Box pRemainder, out Box pSlicedOff)
{
    pRemainder = pBoxToCut;
    pSlicedOff = default;

    if (pCuttingBox.c1.Z < pBoxToCut.c1.Z || pCuttingBox.c1.Z > pBoxToCut.c2.Z) return false;

    pSlicedOff = pBoxToCut;

    // Remainder keeps the front part (inclusive)
    pRemainder.c1.Z = pCuttingBox.c1.Z;
    // Sliced-off part is behind (exclusive)
    pSlicedOff.c2.Z = pCuttingBox.c1.Z - 1;

    return true;
}

bool ForwardSideSlice(Box pCuttingBox, Box pBoxToCut, out Box pRemainder, out Box pSlicedOff)
{
    pRemainder = pBoxToCut;
    pSlicedOff = default;

    if (pCuttingBox.c2.Z < pBoxToCut.c1.Z || pCuttingBox.c2.Z > pBoxToCut.c2.Z) return false;

    pSlicedOff = pBoxToCut;

    // Remainder keeps the back part (inclusive)
    pRemainder.c2.Z = pCuttingBox.c2.Z;
    // Sliced-off part is in front (exclusive)
    pSlicedOff.c1.Z = pCuttingBox.c2.Z + 1;

    return true;
}


List<SliceMethod> sliceMethods = [RightSideSlice, LeftSideSlice, BottomSideSlice, TopSideSlice, BackSideSlice, ForwardSideSlice];


// With all these methods defined we can start defining our main loop...

List<Box> onBoxes = new List<Box>();

foreach (string instruction in myInput)
{
    Match match = instructionParser.Match(instruction);
    if (!match.Success) throw new Exception("Could not parse:" + instruction);

    bool onBox = match.Groups[1].Value == "on";

    int minX = int.Parse(match.Groups[2].Value);
    int maxX = int.Parse(match.Groups[3].Value);
    int minY = int.Parse(match.Groups[4].Value);
    int maxY = int.Parse(match.Groups[5].Value);
    int minZ = int.Parse(match.Groups[6].Value);
    int maxZ = int.Parse(match.Groups[7].Value);

    // verify something that our approach depends on:
    if (minX > maxX || minY > maxY || minZ > maxZ) throw new Exception("Expected different ordering on xyz axis");

    Box newBox = new Box(new Vec3i (minX, minY, minZ), new Vec3i(maxX,maxY,maxZ));

    Merge(newBox, onBox);
}

// To merge a new box in to what we already have:
// - we assume that PRIOR to merging NO boxes are overlapping
// - we'll iterate backwards over every box and do the overlap/contains and subsequently slicing thing...
void Merge(Box pNewBox, bool pOn)
{
    // Test each existing on box for overlap and if there is one, process the merge/slicing etc
    for (int i = onBoxes.Count - 1; i >= 0; i--)
    {
        if (Overlaps(pNewBox, onBoxes[i]))
        {
            //What ever happens during the processing of the overlap will definitely destroy the old box...
            ProcessOverlap(pNewBox, onBoxes[i]);
            onBoxes.RemoveAt(i);
        }
    }

    // When we are done we need to decide what to do with the new box... which is quite simple:
    if (pOn) onBoxes.Add(pNewBox);
}

void ProcessOverlap (Box pNewBox, Box pExistingBox)
{
    // What we will do while processing the overlap is cutting off pieces of the existing box until
    // All cut off pieces are added back into the onBoxes list, the remainder is not.

    Box remainder = pExistingBox;
    Box slicedOff;

    //using each slide method, slice off what we can, add it to the boxes that are on,
    //until the remainder is contained within our new box.
    foreach (SliceMethod sliceMethod in sliceMethods)
    {
        if (sliceMethod (pNewBox, remainder, out remainder, out slicedOff))
        {

            onBoxes.Add(slicedOff);
            if (Contains(pNewBox, remainder)) return;
        }
    }
}

// After processing this we have a list with all on boxes... 

long Area (Box pBox)
{
    Vec3i boxSize = pBox.c2 - pBox.c1;
    // size is inclusive...
    return 1L * (boxSize.X + 1) * (boxSize.Y + 1) * (boxSize.Z + 1);
}

Console.WriteLine("Part 2: " + onBoxes.Sum (Area));


// Has to be below everything for top level c# statements... 

delegate bool SliceMethod(Box cuttingBox, Box boxToCut, out Box remainder, out Box slicedoff);