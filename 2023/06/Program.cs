// Solution for https://adventofcode.com/2023/day/6 (Ctrl+Click in VS to follow link)

using Race = (long time, long distance);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of race times and race distances to beat

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Parse the input into something useable:

string[] inputLines = myInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
string[] times = inputLines[0].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
string[] distances = inputLines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

List<Race> raceData = new();

for (long i = 1; i < times.Length; i++)
{
	raceData.Add((long.Parse(times[i]), long.Parse(distances[i])));
}

// Part 1: Check how many different options we have for holding down the button and still win the race

// Given the example, the question & answer is based on these variables:
// - race time
// - distance to beat
// - buttonDownTime

// speed = buttonDownTime
// raceTimeLeft = inputTime - buttonDownTime
// distanceTravelled = raceTimeLeft * speed
// 
// And the question is for which buttonDownTimes:
// distanceTravelled > distanceToBeat
//
// In other words:
// speed * raceTimeLeft > distanceToBeat =>
// buttonDownTime * (inputTime - buttonDownTime) > distanceToBeat =>
// inputTime * buttonDownTime - buttonDownTime ^ 2 > distanceToBeat =>
// 
// If we replace buttonDownTime with x we get:
// -x^2 + inputTime * x - distanceToBeat > 0

// This is basically a quadratic equation which we can solve using the ABC formula.
// Also if you graph this, you see an inverse parabolic graph (a mountain)

long GetAmountOfDifferentTimesThatWillMakeYouWin (Race pRace)
{
	// Because of the huge numbers, it is very important to use double's instead of floats
	// to prevent severe rounding errors...

	// -x^2 + inputTime * x - distanceToBeat > 0

	double a = -1;
	double b = pRace.time;
	double c = -pRace.distance;

	//float x = (-b +- sqrt (b^2 - 4ac))/2a

	double discriminant = b * b - 4 * a * c;

	if (discriminant < 0) return -1;

	double xMax = (double)(-b - Math.Sqrt(discriminant)) / (2 * a);
	double xMin = (double)(-b + Math.Sqrt(discriminant)) / (2 * a);

	// A bit of boundary magic to make the count work out ;)
	return (long)(Math.Ceiling(xMax) - Math.Floor(xMin)) - 1;
}

long total = 1;
foreach (var race in raceData)
{
    total *= GetAmountOfDifferentTimesThatWillMakeYouWin(race);
}

Console.WriteLine("Part 1 - " + total);

// Part 2 - Bad kerning issue ;)

string time = "";
string distance = "";
for (long i = 1; i < times.Length;i++)
{
	time += times[i];
	distance += distances[i];
}

Race theOneRace = (long.Parse(time), long.Parse(distance));

// 34454850

Console.WriteLine("Part 2 - " + GetAmountOfDifferentTimesThatWillMakeYouWin(theOneRace));


