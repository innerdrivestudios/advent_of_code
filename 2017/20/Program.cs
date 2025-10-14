// Solution for https://adventofcode.com/2017/day/20 (Ctrl+Click in VS to follow link)

// Note: I was used ints at the start, until I reached part 2 :) 

using System.Text.RegularExpressions;
using Particle = (Vec3<double> p, Vec3<double> v, Vec3<double> a);
using Vec3d = Vec3<double>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: A list of particle specifications

string[] myInput = File.ReadAllLines(args[0]);

Regex particleParser = new Regex(@"p=<(-?\d+),(-?\d+),(-?\d+)>, v=<(-?\d+),(-?\d+),(-?\d+)>, a=<(-?\d+),(-?\d+),(-?\d+)>");

List<Particle> particles = new List<Particle>();

foreach (var particleSpecification in myInput)
{
    Match match = particleParser.Match(particleSpecification);
    particles.Add(
            new Particle(
                    new Vec3d(int.Parse (match.Groups[1].Value), int.Parse (match.Groups[2].Value), int.Parse (match.Groups[3].Value)),
                    new Vec3d(int.Parse (match.Groups[4].Value), int.Parse (match.Groups[5].Value), int.Parse (match.Groups[6].Value)),
                    new Vec3d(int.Parse (match.Groups[7].Value), int.Parse (match.Groups[8].Value), int.Parse (match.Groups[9].Value))
                )
        );
}

// Part 1: Which one is the slacker? :)

// In the long run, no matter the position and velocity values, when we simulate everything to infinity,
// the one with the lowest acceleration will be the closest to 0,0,0...

List<Particle> particlesCopy = new(particles);

particlesCopy.Sort ((a, b) => a.a.Magnitude().CompareTo (b.a.Magnitude()));

if (particlesCopy[0].a.Magnitude() == particlesCopy[1].a.Magnitude())
{
    Console.WriteLine("There are more than 1 particle with speed acceleration, more work is needed...");
    //Eg compare based on current position after both moving far away enough to pass the origin
}
else
{
    Console.WriteLine("Part 1:" + particles.IndexOf(particlesCopy[0]));
}

// ** Part 2: How many particles are left after ALL collisions are resolved?

// So I see two options here:
// * Brute force simulation until the particle count doesn't go down anymore for X timesteps 
// * Actually predict the timesteps where particles will intersect and only simulate and process those...

// For practice sake, let's try the latter one...

// Let's first try and come up with an equation for where a particle is at time t
// Basic newtonian mechanics for constant acceleration tell us:
// V(t) = V0 + A*t                      //Velocity at time t is Velocity at time = 0 + the acceleration * t
// P(t) = P0 + VaverageOverT * t        //Position at P(t) is the starting position + the average velocity over time t * t
// P(t) = P0 + (V0 + V(t)) * t / 2      //Same but rewritten
// P(t) = P0 + (V0 + V0 + A*t) * t /2   //Same but rewritten
// P(t) = P0 + V0 * t + A * t * t / 2   //Same but rewritten

// Now there is one catch... (which took me a long time to realize...)
// We add the acceleration to the velocity at the start of the timestep... ARGHGH

// In other words:

// V(t) = V0 + A*(t+1 !!!)                      //Velocity at time t is Velocity at time = 0 + the acceleration * (t+1)
// P(t) = P0 + VaverageOverT * t                //Same
// P(t) = P0 + (V0 + V(t)) * t / 2              //Same
// P(t) = P0 + (V0 + V0 + A*(t+1)) * t /2       //Different now due to the t+1 !!
// P(t) = P0 + V0 * t + A * (t+1) * t / 2       //Different now due to the t+1 !!

// Which is:
// P(t) = P0 + V0 * t + (A * t / 2) + (A * t * t / 2)

// In other words, just to test, we'll comment this out afterwards...

/* 
Vec3d PositionAtTimeT (Particle pParticle, int pT)
{
    return pParticle.p +  pParticle.v * pT + (pParticle.a/2) * pT + (pParticle.a/2) * pT * pT;
}

Particle p = originalParticles[0];

for (int i = 0; i < 500; i++)
{
    Console.WriteLine(p + "\t\t" + PositionAtTimeT(originalParticles[0], i));
    
    p.v += p.a;
    p.p += p.v;
}
*/

// Ok, so the equation for the position is this:
// P(t) = P0 + V0 * t + (A * t / 2) + (A * t * t / 2)

// If we rewrite that to a standard quadratic position equation, we get:
// P(t) = (A/2) * t * t + (A/2 + V0) * t + P0

/* 

// Test if everything still works:
 
Vec3d PositionAtTimeT (Particle pParticle, int pT)
{
    return (pParticle.a/2 * pT * pT) + (pParticle.v + pParticle.a / 2) * pT + pParticle.p;
}

Particle p = originalParticles[0];

for (int i = 0; i < 50000; i++)
{
    Vec3d predicted = PositionAtTimeT(originalParticles[0], i);

    Console.WriteLine(p + "\t\t" + predicted + "\t => " + (p.p == predicted));
    
    p.v += p.a;
    p.p += p.v;
}
*/

Vec3d PositionAtTimeT(Particle pParticle, int pT)
{
    return (pParticle.a / 2 * pT * pT) + (pParticle.v + pParticle.a / 2) * pT + pParticle.p;
}

// If we can create this equation for Particle 1, then we can also create it for Particle 2, eg we get
// (slightly rewritten for legibility):

// P1(t) = (P1.a / 2) * t * t + (P1.a / 2 + P1.v0) * t + P1.p0
// P2(t) = (P2.a / 2) * t * t + (P2.a / 2 + P2.v0) * t + P2.p0

// And now we want to know for each particle, whether it collides with another particle
// which basically means there is a t for which P1(t) is P2(t)...
// In other words, there is a t for which Pdelta (t) = P1(t) - P2(t) = 0

// We can do this with the ABC formula, where looking at:
// Pdelta(t) = (Pdelta.a / 2) * t * t + (Pdelta.a / 2 + Pdelta.v0) * t + Pdelta.p0
// A = Pdelta.a / 2
// B = Pdelta.a / 2 + Pdelta.v0
// C = Pdelta.p0

// Of course the added difficulty here is that we are working with vectors, 
// so we need to check all 3 components and they have to result in a matching time.
// For each dimension, the abc formula can either give us:
// - a single time (the particles collide once)
// - two times (the particle trajectories overlap and they collide twice)
// - no time (they don't collide at all)
// - abc could be all zero -> the trajectories overlap

// Let's try and put this into practice:
// NOTE: I am blatantly ignoring floating/double point issues, mostly trying to see if
// all of this would work without it. Due to the specific nature of the input, it actually
// does while it can be a great cause of errors. There is definite room for improvement here.
// Since the approach worked and gave the correct answer AND the fact that we could skip this whole
// step and just brute force it, I moved on to the next puzzle... 

int GetCollisionTime (Particle pParticleA, Particle pParticleB)
{
    Vec3d A = (pParticleA.a - pParticleB.a) / 2;
    Vec3d B = A + (pParticleA.v - pParticleB.v);
    Vec3d C = pParticleA.p - pParticleB.p;

    List<int> collisionTimes = null;

    for (int i = 0; i < 3; i++)
    {
        double Ae = A[i];
        double Be = B[i];
        double Ce = C[i];

        // If all components are zero, we always align on this plane
        if (Ae == 0 && Be == 0 && Ce == 0) continue;

        double discriminant = Be * Be - 4 * Ae * Ce;

        if (discriminant < 0)
        {
            return -1;  // we will never match for this plane
        }

        if (discriminant > 0)
        {
            double collTime1 = (-Be - Math.Sqrt(discriminant)) / (2 * Ae);
            double collTime2 = (-Be + Math.Sqrt(discriminant)) / (2 * Ae);

            //Console.WriteLine(collTime1 + " " + collTime2);

            // If we didn't have any time yet, just add both time frames ...
            if (collisionTimes == null)
            {
                collisionTimes = [(int)collTime1, (int)collTime2];
            }
            else //If we came up with a different time then already in the list remove it from the list
            {
                List<int> newCollisionTimes = [(int)collTime1, (int) collTime2];
                collisionTimes = collisionTimes.Intersect(newCollisionTimes).ToList();

                if (collisionTimes.Count == 0) return -1;
            }
        }
        else
        {
            double collTime1 = -Be / (2 * Ae);

            //Console.WriteLine(collTime1);

            // If we didn't have any time yet, just add both time frames ...
            if (collisionTimes == null)
            {
                collisionTimes = [(int)collTime1];
            }
            else //If we came up with a different time then already in the list remove it from the list
            {
                List<int> newCollisionTimes = [(int)collTime1];
                collisionTimes = collisionTimes.Intersect(newCollisionTimes).ToList();

                if (collisionTimes.Count == 0) return -1;
            }
        }
    }

    if (collisionTimes.Count == 0)
    {
        return -1;
    }
    else
    {
        collisionTimes.Sort();
        return collisionTimes[0];
    }
}

// Now run through all combinations once to find out what the difference collision times are ...

HashSet<int> collisionTimes = new ();

for (int i = 0; i < particles.Count - 1; i++)
{
    for (int j = i + 1; j < particles.Count; j++)
    {
        int c = GetCollisionTime(particles[i], particles[j]);
        
        if (c > -1)
        {
            collisionTimes.Add(c);
        }

    }
}

List<int> orderedCollisionTimes = collisionTimes.ToList();
orderedCollisionTimes.Sort();

// Now we have exactly all collision times to simulate IN ORDER...

// Map particle to number of particles at that position
Dictionary<Vec3d, int> positions = new();

foreach (int timeStep in orderedCollisionTimes)
{
    positions.Clear();

    // Just simulate and count collisions...
    for (int i = 0; i < particles.Count; i++)
    {
        Vec3d posAtT = PositionAtTimeT(particles[i], timeStep);
        if (!positions.ContainsKey(posAtT)) positions.Add(posAtT, 1);
        else positions[posAtT]++;
    }

    // Simulate again and remove if collision count > 1
    for (int i = particles.Count - 1; i >= 0; i--)
    {
        Vec3d posAtT = PositionAtTimeT(particles[i], timeStep);
        if (positions[posAtT] > 1) particles.RemoveAt(i);
    }
}

Console.WriteLine("Part 2:" + particles.Count);