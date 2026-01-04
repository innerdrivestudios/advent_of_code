// Solution for https://adventofcode.com/2023/day/23 (Ctrl+Click in VS to follow link)

using Hailstone = (Vec3<double> start, Vec3<double> velocity);
using Vec3d = Vec3<double>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: hailstone trails

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

List<Hailstone> hailstones = myInput
    .Splat([",", "@", Environment.NewLine])
    .Select(double.Parse)
    .Chunk(6)
    .Select(x => (new Vec3d(x[0], x[1], x[2]), new Vec3d(x[3], x[4], x[5])))
    .ToList();

Console.WriteLine();

int collideCount = 0;

bool Intersect (Hailstone pHailStoneA, Hailstone pHailStoneB, out Vec3d pHailStoneBPosition)
{
    pHailStoneBPosition = new Vec3d();

    // Line 1: hailstoneA.position = hailstoneA.start + t1 * hailstoneA.velocity
    // Line 2: hailstoneB.position = hailstoneB.start + t2 * hailstoneB.velocity
    //
    // Are these ever equal? Is there a t1 or t2 for which:
    // hailstoneA.start + t1 * hailstoneA.velocity = hailstoneB.start + t2 * hailstoneB.velocity
    //
    // For brevity:
    // hailstoneA.start     = A0 = (Ax0, Ay0)
    // hailstoneA.velocity  = AV = (Avx, Avy)
    // hailstoneB.start     = B0 = (Bx0, By0)
    // hailstoneB.velocity  = BV = (Bvx, Bvy)
    // 
    // Is there a t1 or t2 for which:
    // A0​+t1​Av​=B0​+t2​Bv​
    //
    // Meaning a t1 or t2 for which:
    // Ax0+t1​Avx=Bx0+t2​Bvx
    // Ay0+t1​Avy=By0+t2​Bvy
    //
    // So if we solve for t1 we get:
    // t1​Avx=(Bx0-Ax0)+t2​Bvx  => t1 = ((Bx0-Ax0)+t2​Bvx)/Avx
    // t1​Avy=(By0-Ay0)+t2​Bvy  => t1 = ((By0-Ay0)+t2​Bvy)/Avy
    //
    // Equalling t1:
    // ((Bx0-Ax0)+t2​Bvx)/Avx = ((By0-Ay0)+t2​Bvy)/Avy
    // ((Bx0-Ax0)+t2​Bvx)*Avy = ((By0-Ay0)+t2​Bvy)*Avx
    // (Bx0-Ax0)*Avy+t2​Bvx*Avy = (By0-Ay0)*Avx+t2​Bvy*Avx
    // 
    // Isolating t2:
    // (Bx0-Ax0)*Avy - (By0-Ay0)*Avx = +t2​Bvy*Avx - t2​Bvx*Avy
    // (Bx0-Ax0)*Avy - (By0-Ay0)*Avx = t2​ (Bvy*Avx - ​Bvx*Avy)
    // t2 = (Bx0-Ax0)*Avy - (By0-Ay0)*Avx / (Bvy*Avx - ​Bvx*Avy)
    //
    // In code:
    double Ax0 = pHailStoneA.start.X;
    double Ay0 = pHailStoneA.start.Y;
    double Bx0 = pHailStoneB.start.X;
    double By0 = pHailStoneB.start.Y;
    double Avx = pHailStoneA.velocity.X;
    double Avy = pHailStoneA.velocity.Y;
    double Bvx = pHailStoneB.velocity.X;
    double Bvy = pHailStoneB.velocity.Y;
    
    double denominator = Bvy * Avx - Bvx * Avy;
    if (denominator == 0) return false;

    double numeratorT2 = (Bx0 - Ax0) * Avy - (By0 - Ay0) * Avx;
    if (numeratorT2 == 0) return false;

    double t2 =  numeratorT2 / denominator;

    if (t2 < 0) return false;

    //Similary we can also deduct t1
    double numeratorT1 = (Bx0 - Ax0) * Bvy - (By0 - Ay0) * Bvx;
    if (numeratorT1 == 0) return false;

    double t1 = numeratorT1/ denominator;
    if (t1 < 0) return false;

    pHailStoneBPosition = pHailStoneB.start + t2 * pHailStoneB.velocity;

    return true;
}

long minCoord = 200000000000000;
long maxCoord = 400000000000000;
int count = 0;

for (int i = 0; i < hailstones.Count - 1; i++)
{
    for (int j = i + 1; j < hailstones.Count; j++)
    {
        if (Intersect(hailstones[i], hailstones[j], out Vec3d intersection))
        {
            if (intersection.X >= minCoord && intersection.X <= maxCoord &&
                intersection.Y >= minCoord && intersection.Y <= maxCoord)
            {
                //Console.WriteLine(hailstones[i] + "  " + hailstones[j]);
                //Console.WriteLine(intersection.X + "  " + intersection.Y);
                count++;
            }
            //Console.WriteLine(hailstones[i] + " " + hailstones[j] + " at " + intersection);
        }
    }
}

Console.WriteLine("Part 1: " + count);

//31588 too high