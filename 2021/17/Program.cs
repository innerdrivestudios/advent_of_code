//Solution for https://adventofcode.com/2021/day/17 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: an area through which a "physically" simulated projectile should pass, specified like e.g.:
//  target area: x=185..221, y=-122..-74

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings("");

Regex targetAreaParser = new Regex(@"target area: x=(-?\d+)..(-?\d+), y=(-?\d+)..(-?\d+)");
Match match = targetAreaParser.Match(myInput);

if (!match.Success) return;

int minX = int.Parse(match.Groups[1].Value);
int maxX = int.Parse(match.Groups[2].Value);
int minY = int.Parse(match.Groups[3].Value);
int maxY = int.Parse(match.Groups[4].Value);

Console.WriteLine($"Target area: x = {minX}..{maxX}, y = {minY}..{maxY}");

// ** Part 1:
// Find the initial velocity that causes the probe to reach the highest y position
// and still eventually be within the target area after any step.
// What is the highest y position it reaches on this trajectory?
//
// This is another exquisitely crafted puzzle that took me a while to figure out.
// I'll save you all the thinking errors, but here is what I realized after tinkering a bit...
//
// 1.   X & Y are independent variables (duh), so we can treat them as such.
//      In other words, part 1 makes you think you need to simulate stuff... but we don't (really).
// 
// 2.   What comes up, must come down. Normally with the same speed as you threw it with.
//      In other words, if you shoot something in the air starting at y = 0 with a initial y velocity of 10,
//      the projectile will come down with a velocity of -10 at y = 0
//
// 3.   After a projectile has come down to y=0 with a velocity of -startingVelocity,
//      it will decrease the velocity by 1 again, for the next simulation step.
//      In the previous example, this would mean yVelocity = -11 and the first position after y=0
//      (on the way down, assuming there is no ground, but we are flying through the air still), 
//      will be y = -11 (e.g. -(starting velocity + 1).

// OK, NOW for the actual challenge/puzzle... we are being asked about the MAX Y we can reach,
// while still passing through the target area. Let's assume we can figure out a x velocity that
// will take care of the x area part, we can safely ignore that for now, since as mentioned,
// X & Y are indepedent of one another.
//
// So, what is the HIGHEST starting y velocity that will still allow the projectile to pass 
// through the target y area? Well, looking at my target area: x=185..221, y=-122..-74
// we see the minimum y value of the target area is -122. 
// In other words IF our y velocity on the way down takes us from y=0 to y=-122 we are still JUST 
// with the target area, if our y velocity on the down would take us from y=0 to y=-123 we would miss it.
// Of course, a y velocity that would take us from y=0 to y=-100 or y=-74 is also valid, but automatically
// NOT the highest y starting velocity.
//
// SO our minY = -(starting velocity + 1)
// i.e. startingVelocity = -minY-1

int maxStartingYVelocity = -minY - 1;
Console.WriteLine("Starting velocity:" + maxStartingYVelocity);

// And then the question is, how high will we go with this starting y velocity...
// Which in our case is basically 121 + 120 + 119 + .. + 1

// We can calculate this in a for loop, but there is an easier way:
// Imagine we want to calculate 10+9+8+...+1, what we can do is:
//
// 10   +   9   +   8   +   7   +   ..  +   1
//  1   +   2   +   3   +   4   +       +   10
//
// (Basically write down the same sum in reverse)
// Now every column adds up to 11 ! and we do that 10 times,
// so the total will be 110 BUT this is TWICE the amount we were looking for,
// so 110/2 = 55.
//
// Or more generically the running sum (is that the name?) of a number n is
// n * (n+1) / 2

int maxYPosition = (maxStartingYVelocity * (maxStartingYVelocity + 1)) / 2;
Console.WriteLine("Part 1: " + maxYPosition);

// ** Part 2: Find ALL possible velocities that will still go through the target area...

// Observations:
// - There is a max velocity that still takes you through the target y area (calculated in part 1)
//   but there is also a min velocity that takes you through the target y area: minY-1
//   e.g. in our case of x=185..221, y=-122..-74 if our starting y velocity is -123 we would blow
//   past the starting area... since the first step would be y += -123 = -123 < -122 

// Given our min and max y velocities:
int minStartingYVelocity = minY - 1;

// We can simulate each trajectory to figure out whether this trajectory will pass through the target area (at end which step)

int SimulateYPosition (int pStartingYVelocity, int pTimeStep)
{
    // At time step pTimeStep we have a position of:
    // pStartingVelocity-0 + pStartingVelocity-1 + pStartingVelocity-2 + pStartingVelocity-...(pTimeStep-1)
    // i.e...
    return pTimeStep * pStartingYVelocity - pTimeStep * (pTimeStep - 1) / 2;
}

// So now we'll quickly test all possible y velocities to see in which timestep they might fall within the target
// area or not ...

List<(int yVel, int timestep)> possibleYVelocitiesAndTimeSteps = new ();

for (int yVel = minStartingYVelocity; yVel <= maxStartingYVelocity; yVel++)
{
    int time = 0;

    while (true)
    {
        int yPos = SimulateYPosition(yVel, time);
        
        if (yPos >= minY && yPos <= maxY)
        {
            possibleYVelocitiesAndTimeSteps.Add((yVel, time));
        }
        //if we go past the end stop ...
        else if (yPos < minY) break;

        time++;
    }
}

// Now let's see if there is something similar we can do for the X positions...
// 
// The puzzle description says:
//
// "The probe's x position increases by its x velocity.
// ...
// Due to drag, the probe's x velocity changes by 1 toward the value 0; that is,
// - it decreases by 1 if it is greater than 0,
// - increases by 1 if it is less than 0,
// - or does not change if it is already 0."

// Nice bit of misdirection right there, since the x velocity will never be < 0 :)

// The x velocity works a lot like the y velocity, we start at xVelocity and then the next
// frame we have xVel-1, xVel-2 etc, however the xVel can never goes BELOW zero (unlike our yVelocity).

// To establish the valid min and max starting velocities for X we need to figure out:
// - the min x velocity that will still allow us to reach minX (185 in our case)
// - the max x velocity that will still allow us to be within maxX (221 in our case)

// How far can we get with speed X at most?
// maxDistance = xSpeed * (xSpeed+1) / 2
// In other words if the minimum distance we need to achieve is xMin we can say:
// xMin = xSpeed * (xSpeed+1) / 2 ->
// xMin * 2 = xSpeed * (xSpeed+1)
// xMin * 2 = xSpeed^2 + xSpeed
// xSpeed^2 + xSpeed - 2*xMin = 0
//
// Let's apply the ABC formula:

int a = 1;
int b = 1;
int c = -2 * minX;

int xVel1 = (int)Math.Ceiling((-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a));
int xVel2 = (int)Math.Ceiling((-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a));

int minStartingXVelocity = int.Max(xVel1, xVel2);
Console.WriteLine("Min x velocity:" + minStartingXVelocity);
Console.WriteLine("Max x Distance with this velocity:" + (minStartingXVelocity * (minStartingXVelocity + 1) / 2));

int maxStartingXVelocity = maxX;
Console.WriteLine("Max x velocity:" + maxStartingXVelocity);
Console.WriteLine("Min x Distance with this velocity:" + maxStartingXVelocity);

// Simulating the x position is done similarly to simulating the y position,
// with the difference that the x velocity never goes < 0
// In other words we need to clamp the timestep to where the x velocity would go below 0:

int SimulateXPosition(int pStartingXVelocity, int pTimeStep)
{
    //If x velocity is 5, we can only simulate 
    pTimeStep = int.Min(pStartingXVelocity, pTimeStep);
    return pTimeStep * pStartingXVelocity - pTimeStep * (pTimeStep - 1) / 2;
}

// But this gives us another challenge when we want to find out ALL valid starting x velocities 
// and their timesteps... if the end position is valid when the x velocity is 0 at timestep t
// the end position will be valid at ALL timesteps t+n after that as well! 
// In other words, a naive loop would never end, without a time limit...
// But we KNOW the time limit, that is the max time step we could find for the y velocities:

int maxTimeStep = possibleYVelocitiesAndTimeSteps.Max(x => x.timestep);

List<(int xVel, int timestep)> possibleXVelocitiesAndTimeSteps = new();

for (int xVel = minStartingXVelocity; xVel <= maxStartingXVelocity; xVel++)
{
    int time = 0;

    while (time <= maxTimeStep)
    {
        int xPos = SimulateXPosition(xVel, time);

        if (xPos >= minX && xPos <= maxX)
        {
            possibleXVelocitiesAndTimeSteps.Add((xVel, time));
        }
        //if we go past the end ...
        else if (xPos > maxX) break;

        time++;
    }
}

// Now we have all valid y velocities and their timesteps at which they are in the area
// And we have all valid x velocities and their timesteps at which they are in the area

// What we'll do now is get all distinct time steps for the intersection of the x and y timesteps and
// then we'll loop over those timesteps, checking how many valid x and y velocities we have for those timesteps

HashSet<int> distinctTimeSteps = possibleXVelocitiesAndTimeSteps.Select(x => x.timestep)
                              .Intersect(possibleYVelocitiesAndTimeSteps.Select(x => x.timestep))
                              .ToHashSet();

// This could be improved with a reverse lookup table (timestep to count), but it should be fast enough as it is:

HashSet<(int x, int y)> valid = new();

foreach (var timestep in distinctTimeSteps)
{
    var validXTimeSteps = possibleXVelocitiesAndTimeSteps.Where(x => x.timestep == timestep).ToList();
    var validYTimeSteps = possibleYVelocitiesAndTimeSteps.Where(x => x.timestep == timestep).ToList();

    //We cannot simply multiply the amount of valid x vel and y vel for a certain time steps, 
    //since multiple velocities might be valid at different timesteps, meaning we'd count the 
    //same velocity twice
    foreach (var validXTimeStep in validXTimeSteps)
    {
        foreach (var validYTimeStep in validYTimeSteps)
        {
            valid.Add((validXTimeStep.xVel, validYTimeStep.yVel));
        }
    }
}

Console.WriteLine("Part 2: " + valid.Count);


