// Solution for https://adventofcode.com/2022/day/9 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: ....

(char dir, int count)[] myInput = File.ReadAllLines(args[0])      //Get the whole file as a string array
    .Select (x => x.Split(" "))                         //turn every line into a string []
    .Select (x => (x[0][0], int.Parse(x[1])))           //convert every string[] into a (char,int)
    .ToArray ();

// ** Part 1: Where we start doesn't matter, we only need the position count, not the positions themselves

Vec2i head = new Vec2i(0, 0);
Vec2i tail = new Vec2i(0, 0);

string directionsString = "LRUD";
Vec2i[] directions = [ new (-1,0), new (1,0), new (0,1), new (0,-1) ];
HashSet<Vec2i> uniquePositions = new HashSet<Vec2i> ();
uniquePositions.Add (tail);

foreach (var step in myInput)
{
    Vec2i direction = directions[directionsString.IndexOf(step.dir)];
    for (int i = 0; i < step.count; i++)
    {
        head += direction;

        //This is basically some kind of verlet physics where we satisfy a constraint
        //(which is that the manhattan distance must be 1)

        Vec2i delta = head - tail;                                  //Get the delta
        if (Math.Max(Math.Abs(delta.X), Math.Abs(delta.Y)) == 2)    //If the manhattan distance is not 1
        {
            delta.X = Math.Sign(delta.X);                           //Adjust it, so it becomes 1
            delta.Y = Math.Sign(delta.Y);
            tail += delta;
            uniquePositions.Add(tail);
        }
    }
}

Console.WriteLine("Part 1 - Unique position count:" + uniquePositions.Count);

// ** Part 2: Same thing but now with more knots in the rope, so we'll generalize the setup
// (which we could have used for Part 1 as well...)

int GetUniqueTailPositionCountForARopeOfLength(int pKnotCount)
{
    Vec2i[] knotPositions = Enumerable.Repeat(new Vec2i(0, 0), pKnotCount).ToArray();

    HashSet<Vec2i> uniqueTailPositions = new HashSet<Vec2i> ();

    foreach (var step in myInput)
    {
        Vec2i direction = directions[directionsString.IndexOf(step.dir)];
        
        for (int i = 0; i < step.count; i++)
        {
            //update the head...
            knotPositions[0] += direction;

            //and let the rest follow
            for (int j = 1; j < knotPositions.Length; j++)
            {
                knotPositions[j] = FollowTarget(knotPositions[j-1], knotPositions[j]);
            }

            uniqueTailPositions.Add(knotPositions.Last());
        }

    }

    return uniqueTailPositions.Count;
}
 
Vec2i FollowTarget (Vec2i pTarget, Vec2i pCurrent)
{
    Vec2i delta = pTarget - pCurrent;

    if (Math.Max(Math.Abs(delta.X), Math.Abs(delta.Y)) == 2)
    {
        delta.X = Math.Sign(delta.X);
        delta.Y = Math.Sign(delta.Y);
        pCurrent += delta;
    }

    return pCurrent;
}

Console.WriteLine(
    "Part 2 - Unique position count:" +
    GetUniqueTailPositionCountForARopeOfLength(10)
);
