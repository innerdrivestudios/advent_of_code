// Solution for https://adventofcode.com/2023/day/22 (Ctrl+Click in VS to follow link)

using Vec3i = Vec3<int>;
using Cublet = Vec3<int>;
using Beam = System.Collections.Generic.List<Vec3<int>>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of cube specs (start point to end point)

string[] myInput = File.ReadAllLines(args[0]);

List<(Vec3i start, Vec3i end)> beamSpecifications = myInput
    .Select (x => x.Splat(['~',',']).Select(int.Parse).ToArray())
    .Select (x => (new Vec3i(x[0], x[1], x[2]), new Vec3i(x[3], x[4], x[5])))
    .ToList ();

// Now we have all cube specs (start to end point)
// Let's convert each cube spec into a HashSet of points
// and do some sanity checking

List<Beam> beams = new();

// We'll keep track of this for later...
int maxX = 0;   
int maxY = 0;   
int maxZ = 0;   

foreach (var beamSpecification in beamSpecifications)
{
    Vec3i delta = (beamSpecification.end - beamSpecification.start);
    
    //sanity check 1: a brick is always extended in 1 direction only
    int zeroCount = 0;
    for (int i = 0; i < 3; i++) if (delta[i] == 0) zeroCount++;
    if (zeroCount < 2) throw new Exception("Unexpected");

    //sanity check 2: start xyz is always < end xyz
    if (delta.X + delta.Y + delta.Z < 0) throw new Exception("Unexpected");

    int steps = delta.ManhattanDistance();
    Vec3i step = delta.Sign();
    Vec3i start = beamSpecification.Item1;

    if (steps == 0) Console.WriteLine("@@@");

    Beam beam = new();
    for (int i = 0; i <= steps;i++)
    {
        Vec3i cublet = start + i * step;
        beam.Add (cublet);

        maxX = int.Max (maxX, cublet.X);
        maxY = int.Max (maxY, cublet.Y);
        maxZ = int.Max (maxZ, cublet.Z);
    }

    beams.Add (beam);
}

// Now we first need to "land" all pieces...
// Let's create a 2d plane array that tells us how high every xy has been filled up
// All pieces have positive coords...

Console.WriteLine("Max XYZ: " + (maxX, maxY, maxZ));

// By default everything is at (null,0)
// So we'll store the height of the highest beam at a certain position and a reference to the beam itself
int[,] heightMap = new int[maxX+1, maxY+1];

// And we are also going to store references to the beams themselves...
Beam[,,] beamsCache = new Beam[maxX+1, maxY+1, maxZ + 1];

// Now we'll check for grounded bricks, but we'll do it in a logical order, 
// sorting all beams on the Z of their first beam
beams.Sort((a, b) => a[0].Z - b[0].Z);

for (int i = 0; i < beams.Count; i++)
{
    SettleBeam (beams[i]);
}

void SettleBeam (Beam pBeam)
{
    //Let's get the delta of each beam to the beam below it
    int minDelta = int.MaxValue;
    
    foreach (Cublet cublet in pBeam)
    {
        minDelta = int.Min (minDelta, (cublet.Z - (heightMap[cublet.X, cublet.Y] + 1)));
    }

    Console.WriteLine("We can move " + string.Join (" ", pBeam) + " over a distance of " + minDelta + " z units");

    for (int i = 0; i < pBeam.Count; i++)
    {
        Vec3i cublet = pBeam[i];
        cublet.Z -= minDelta;
        pBeam[i] = cublet;

        heightMap[cublet.X, cublet.Y] = cublet.Z;
        beamsCache[cublet.X, cublet.Y, cublet.Z] = pBeam;
    }

    //Console.WriteLine("New position " + string.Join(" ", pBeam) + " over a distance of " + minDelta + " z units");
    //Console.ReadKey();
}

//Now that the beams have settled... check which beams:
//- are not supporting anything
//- are only supporting beams that are supported by more than 1 beam

// Returns the beams that are supported by (above) the given beam that are not null and NOT equal to that beam :) (tricky)
HashSet<Beam> GetSupportedBeams (Beam pBeam)
{
    HashSet<Beam> supportedBeams = new ();

    foreach (Cublet cublet in pBeam)
    {
        Beam supportedBeam = beamsCache[cublet.X, cublet.Y, cublet.Z+1];
        if (supportedBeam != null && supportedBeam != pBeam) supportedBeams.Add(supportedBeam);
    }

    return supportedBeams;
}

// Returns how many beams are supporting (below) the given beam
int GetSupportedByCount (Beam pBeam)
{
    HashSet<Beam> supportedBeams = new();

    foreach (Cublet cublet in pBeam)
    {
        Beam supportedBeam = beamsCache[cublet.X, cublet.Y, cublet.Z - 1];
        if (supportedBeam != null && supportedBeam != pBeam) supportedBeams.Add(supportedBeam);
    }

    return supportedBeams.Count;
}

int safeToDisIntegrate = 0;

for (int i = 0; i < beams.Count; i++)
{
    Beam beam = beams[i];

    HashSet<Beam> supportedBeams = GetSupportedBeams (beam);

    // If we are supporting no beam...
    if (supportedBeams.Count == 0)
    {
        safeToDisIntegrate++;
    }
    else // Or all the beams we are supporting are supported by at least 1 other...
    {
        bool allSupportedBeamsSupportedByMoreThan1 = true;

        foreach (Beam supportedBeam in supportedBeams)
        {
            if (GetSupportedByCount(supportedBeam) < 2)
            {
                allSupportedBeamsSupportedByMoreThan1 = false;
                break;
            }
        }

        if (allSupportedBeamsSupportedByMoreThan1) safeToDisIntegrate++;
    }
}

Console.WriteLine("Part 1: " + safeToDisIntegrate);
