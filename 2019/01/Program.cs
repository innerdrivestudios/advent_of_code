//Solution for https://adventofcode.com/2019/day/1 (Ctrl+Click in VS to follow link)

// Your input: a list of masses for a bunch of objects
using System.Reflection.Metadata.Ecma335;

string myInput = "90014\r\n136811\r\n76785\r\n52456\r\n100165\r\n145455\r\n139492\r\n147364\r\n132728\r\n148120\r\n125346\r\n70660\r\n93908\r\n65560\r\n117553\r\n82640\r\n102895\r\n52255\r\n92105\r\n131486\r\n108400\r\n50206\r\n143776\r\n125140\r\n85110\r\n99560\r\n132357\r\n114882\r\n54894\r\n97524\r\n92970\r\n96947\r\n90800\r\n77099\r\n105103\r\n66349\r\n88495\r\n105036\r\n141694\r\n125727\r\n84853\r\n138364\r\n65577\r\n148012\r\n79944\r\n96503\r\n119701\r\n66221\r\n72469\r\n93647\r\n78767\r\n56419\r\n53435\r\n77682\r\n122753\r\n144944\r\n54835\r\n57744\r\n131886\r\n101510\r\n113730\r\n94631\r\n132978\r\n132739\r\n64250\r\n125158\r\n89069\r\n112371\r\n95739\r\n93349\r\n78558\r\n135082\r\n132015\r\n144682\r\n62515\r\n59722\r\n70175\r\n82703\r\n65827\r\n78405\r\n125701\r\n94987\r\n70914\r\n62543\r\n130058\r\n83997\r\n133749\r\n62224\r\n116328\r\n120760\r\n118160\r\n76755\r\n64521\r\n109956\r\n113248\r\n141473\r\n100546\r\n74991\r\n53223\r\n147635\r\n";

//Your task:
// Part 1 - Find out how much fuel is required to launch these objects...

// Fuel required to launch a given module is based on its mass.
// Specifically, to find the fuel required for a module,
// take its mass, divide by three, round down, and subtract 2

long[] masses = myInput
	.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
	.Select(long.Parse)
	.ToArray();

long fuelRequired = masses.Sum( x => (x / 3) - 2 );

Console.WriteLine ("Part 1 - Fuel required to launch all objects: " + fuelRequired);

// Part 2 -
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
