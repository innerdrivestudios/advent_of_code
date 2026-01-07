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

// Just to be sure sort all the beams specs on the first z:
beamSpecifications.Sort((a, b) => a.start.Z - b.start.Z);

// Now we have all beam specs (start to end point)
// Let's convert each beam spec into a collection of cublets (separate points)
// and do some sanity checking

// We'll keep track of this for later...
int maxX = 0;
int maxY = 0;
int maxZ = 0;
List<Beam> beams = new();

// Converts all beams SPECS (start, end) to actual beams (a collection of vec3i from start to end), 
// a bit lazy, instead of simply resetting the beams to an original position, 
// we kind of reparse the input
// (for part 1 this doesn't matter, since it needs to happen only once, but for part 2
// we repeat this over and over again)

void ResetBeams ()
{
    beams.Clear();

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

        Beam beam = new();
        for (int i = 0; i <= steps; i++)
        {
            Vec3i cublet = start + i * step;
            beam.Add(cublet);

            maxX = int.Max(maxX, cublet.X);
            maxY = int.Max(maxY, cublet.Y);
            maxZ = int.Max(maxZ, cublet.Z);
        }

        beams.Add(beam);
    }
}

ResetBeams();

// Now we first need to "land" all pieces...
// We'll use several data structures for that to speed things up...

// So we'll store the height of the highest beam at a certain position xy position
int[,] heightMap;

// And we are also going to store references to the beams themselves at each xyz after landing
Beam[,,] beamsCache;

// Now simulate all beams falling, optionally skipping a beam entirely (for part 2)
// And return a list of where each first cublet of a beam landed (also for part 2)
Cublet[] SimulateBeamsFalling(int pBeamToSkip = -1)
{
    // Initialize all the data structures...
    heightMap = new int[maxX + 1, maxY + 1];
    beamsCache = new Beam[maxX + 1, maxY + 1, maxZ + 1];

    // In preparation of part 2 we'll keep track of the position of the first cublet of each beam,
    // so we can check for differences later on...
    Cublet[] firstCubletPosition = new Cublet[beams.Count];

    // Now we'll check for grounded bricks, but we'll do it in a logical order, 
    // sorting all beams on the Z of their first beam

    for (int i = 0; i < beams.Count; i++)
    {
        if (pBeamToSkip < 0 || pBeamToSkip != i) SettleBeam(i);
    }

    void SettleBeam(int pBeamIndex)
    {
        Beam beam = beams[pBeamIndex];

        //Let's get the delta of each beam to the beam below it
        int minDelta = int.MaxValue;

        foreach (Cublet cublet in beam)
        {
            minDelta = int.Min(minDelta, (cublet.Z - (heightMap[cublet.X, cublet.Y] + 1)));
        }

        //Console.WriteLine("We can move " + string.Join (" ", beam) + " over a distance of " + minDelta + " z units");

        for (int i = 0; i < beam.Count; i++)
        {
            Vec3i cublet = beam[i];
            cublet.Z -= minDelta;
            beam[i] = cublet;

            heightMap[cublet.X, cublet.Y] = cublet.Z;
            beamsCache[cublet.X, cublet.Y, cublet.Z] = beam;
        }

        firstCubletPosition[pBeamIndex] = beam[0];

        //Console.WriteLine("New position " + string.Join(" ", beam) + " over a distance of " + minDelta + " z units");
        //Console.ReadKey();
    }

    return firstCubletPosition;
}

// Keep track of this for part 2...
Cublet[] originalSimulation = SimulateBeamsFalling();

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

// ** Part 2... for part two I'm going to rerun the simulation,
// checking how many blocks end up at a different position 
// then the original simulation if we just completely skip blocks...

int blocksThatFell = 0;

for (int i = 0; i < beams.Count;i++)
{
    ResetBeams();
    Cublet[] newPositions = SimulateBeamsFalling(i);

    // We start checking from AFTER the beam that we skipped in the simulation
    for (int j = i + 1; j < newPositions.Length; j++)
    {
        if (newPositions[j] != originalSimulation[j]) blocksThatFell++;
    }
}

Console.WriteLine("Part 2: " + blocksThatFell);
