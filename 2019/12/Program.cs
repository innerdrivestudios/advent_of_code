// Solution for https://adventofcode.com/2019/day/12 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of positions for moons

// First parse all the input ...

string[] myInput = File.ReadAllLines(args[0]);

Regex positionParser = new Regex(@"<x=(-?\d+), y=(-?\d+), z=(-?\d+)>");

List<Planet> planets = new List<Planet>();

foreach (string input in myInput)
{
	Match match = positionParser.Match(input);
    Console.WriteLine("Parsing:" + input + " => " + match.Success);

    planets.Add(
        new Planet(
            new Vec3<long> (
                long.Parse(match.Groups[1].Value),
                long.Parse(match.Groups[2].Value),
                long.Parse(match.Groups[3].Value)
            )
        )
    );
}

// ** Part 1: Run the simulation for a 1000 steps and print the total energy
// of the whole system...

// First define some helper methods...

void SimulatePlanets()
{
    for (int i = 0; i < planets.Count; i++)
    {
        for (int j = 0; j < planets.Count; j++)
        {
            planets[i].UpdateVelocity(planets[j]);
        }
    }

    foreach (Planet planet in planets)
    {
        planet.Simulate();
    }
}

// Now run the simulation....

for (int i = 0; i < 1000; i++)
{
    SimulatePlanets();
}

// And print the result...
Console.WriteLine("Part 1: " + planets.Sum (x => x.GetTotalEnergy()));

// Part 2: Find out when the system repeats itself.
// This one had me stumped for quite a while, until a reddit hint
// sent me in the right direction. (Note to self: don't check reddit,
// solving the puzzle really IS less fun after that).
// The hint in question: look at each axis separate, since the axis
// don't influence each other.
// I was originally thinking in the same direction looking at when
// a single planet repeats, but of course since the planets influence
// each other, this doesn't tell you anything...

// GetFrequency returns how many iterations it takes to be in the
// same position / velocity state as we started

ulong GetFrequency (int pAxis)
{
    foreach (Planet planet in planets) planet.Reset();

    ulong steps = 0;

    //lookup table for all position and velocity X OR Y OR Z
    //(we only look at 1 component at a time!)
    HashSet<(long, long, long, long, long, long, long, long)> visited = new();

    while (true)
    {
        SimulatePlanets();

        var key = (
                planets[0].position[pAxis],
                planets[1].position[pAxis],
                planets[2].position[pAxis],
                planets[3].position[pAxis],
                planets[0].velocity[pAxis],
                planets[1].velocity[pAxis],
                planets[2].velocity[pAxis],
                planets[3].velocity[pAxis]
         );
        //Console.WriteLine(key);
        if (!visited.Add(key)) break;

        steps++;
    }
    return steps;
}

Console.WriteLine("Starting calculation of part 2...");

// Now for all three axis get the frequency:
ulong[] frequencies = new ulong[3];

for(int i = 0; i < frequencies.Length; i++)
{
    Console.WriteLine("Calculating frequency for axis "+i);
    frequencies[i] = GetFrequency(i);
}

// Now we have the separate frequencies, but what do we do with them?
// Imagine we have an animation consisting of 2 sub animations,
// and both animation lengths are prime numbers, e.g.:
// - 3, 5
// 
// If we look at where they all repeat:
// 3 ->  6,  9, 12, 15, ...  
// 5 -> 10, 15, ... 
//
// So after 15 frames both animations have repeated.
// (This is also called the LCM, the Least Common Multiple)

// But what if the numbers are not prime (as in our case?)
// or if 2 numbers are the same,
// or if we have multiple numbers?

// e.g 5 & 5?
// Is the answer 5*5?
// No, because the animation will already repeat at 5
// (The LCM is 5)

// The LCM is defined as LCM (a,b) = a * b / GCD (a,b)
// For multiple numbers LCM (a,b,c) it is LCM (LCM(a,b), c)
// (For more info also see gcd_vs_lcm.txt)

// In other words, first we'll define the GCD (using the Euclidean method):

ulong GCD (ulong a, ulong b)
{
    while (b != 0)
    {
        ulong temp = (ulong)b;
        b = a % b;
        a = temp;
    }

    return a;
}

// Then the LCM
ulong LCM (ulong a, ulong b)
{
    //changed order of a*b/gcd to a/gcd*b to prevent overflow
    //in more cases even though it isn't strictly needed here
    return a / GCD(a, b) * b;
}

// And calculate the result:
ulong result = LCM(LCM(frequencies[0], frequencies[1]), frequencies[2]);

Console.WriteLine("Part 2: "+result);

