// Solution for https://adventofcode.com/2019/day/1 (Ctrl+Click in VS to follow link)

// ** Your input: a list of masses for a bunch of objects

long[] masses = ParseUtils.FileToNumbers<long>(args[0], Environment.NewLine);

// ** Part 1 - Find out how much fuel is required to launch these objects...

// Fuel required to launch a given module is based on its mass.
// Specifically, to find the fuel required for a module,
// take its mass, divide by three, round down, and subtract 2

long fuelRequired = masses.Sum( x => (x / 3) - 2 );
Console.WriteLine ("Part 1 - Fuel required to launch all objects: " + fuelRequired);

// ** Part 2 -

// Fuel itself requires fuel just like a module -
// take its mass, divide by three, round down, and subtract 2.
// However, that fuel also requires fuel, and that fuel requires fuel, and so on.
// Any mass that would require negative fuel should instead be treated as if it requires zero fuel;
// the remaining mass, if any, is instead handled by wishing really hard,
// which has no mass and is outside the scope of this calculation.

// So, for each module mass, calculate its fuel and add it to the total.
// Then, treat the fuel amount you just calculated as the input mass and repeat the process,
// continuing until a fuel requirement is zero or negative. 

long fuelRequiredIncludingFuel = masses.Sum(x => CalculateFuelRequirements(x));

long CalculateFuelRequirements (long pInputMass)
{
	long baseFuel = (pInputMass / 3) - 2;

	if (baseFuel < 0) return 0;

	return baseFuel + CalculateFuelRequirements(baseFuel);
} 

Console.WriteLine("Part 2 - Fuel required to launch all objects including their full: " + fuelRequiredIncludingFuel);

Console.ReadKey();
