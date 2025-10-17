// Solution for https://adventofcode.com/2015/day/21 (Ctrl+Click in VS to follow link)

using Gear = (int cost, int damage, int armor);
using Stats = (int health, int damage, int armor);
// Would love to define using Loadout = (Gear, Gear, Gear, Gear) here as well, but we can't in top level C# :)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: some stats (hitpoints, damage and armor)

string[] myInput = File.ReadAllLines(args[0]);
Dictionary<string, int> stats = myInput
	.Select (x => x.Split([": "], StringSplitOptions.RemoveEmptyEntries))
	.ToDictionary(x => x[0], x => int.Parse(x[1]));

// Apart from the input, we need to define some tables:

// You must use one weapon
// (Note that the actual names of the weapons don't matter)
List<Gear> weapons = new List<Gear> ()
{
	//Cost   //Dmg    //Armor
	( 8,     4,       0),
	(10,     5,       0),
	(25,     6,       0),
	(40,     7,       0),
	(74,     8,       0),
};

// Armor is optional
// (Note that the actual names of the armor don't matter)
List<Gear> armor = new List<Gear>()
{
	//Cost   //Dmg    //Armor
	(  0 ,     0,       0),
	(  13,     0,       1),
	(  31,     0,       2),
	(  53,     0,       3),
	(  75,     0,       4),
	( 102,     0,       5)
};

// Rings are optional, but each ring is available only once
// (Note that the actual names of the rings don't matter)
List<Gear> rings = new List<Gear>()
{
	(  0,     0,       0),
	( 25,     1,       0),
	( 50,     2,       0),
	(100,     3,       0),
	( 20,     0,       1),
	( 40,     0,       2),
	( 80,     0,       3)
};

// Your tasks: Calculate the cheapest gear to win the fight (Part 1) and the most expensive gear to lose the fight (Part 2)

// To get started, we'll first generate all possible gear configurations:
List<(Gear weapon, Gear armor, Gear ring1, Gear ring2)> gearConfigurations = GenerateAllPossibleGearConfigurations();

List<(Gear weapon, Gear armor, Gear ring1, Gear ring2)> GenerateAllPossibleGearConfigurations() {
	var gearConfigurations = new List<(Gear weapon, Gear armor, Gear ring1, Gear ring2)>();

	//every weapon
	for (int w = 0; w < weapons.Count; w++)
	{
		//every piece of armor including no armor
		for (int a = 0; a < armor.Count; a++)
		{
			//for each combination of weapons and armor we have the option of using no rings at all
			gearConfigurations.Add((weapons[w], armor[a], rings[0], rings[0]));

			//followed by every combi of weapon and armor plus every allowed unique ring combination
			for (int r1 = 0; r1 < rings.Count - 1; r1++)
			{
				for (int r2 = r1 + 1; r2 < rings.Count; r2++)
				{
					gearConfigurations.Add((weapons[w], armor[a], rings[r1], rings[r2]));
				}
			}
		}
	}

	return gearConfigurations;
}

// Then we'll define some helper functions to get the stats for a loadout

Stats GetStats((Gear weapon, Gear armor, Gear ring1, Gear ring2) pLoadout, int pHealth)
{
    Stats myStats;
    myStats.armor = pLoadout.weapon.armor + pLoadout.armor.armor + pLoadout.ring1.armor + pLoadout.ring2.armor;
    myStats.damage = pLoadout.weapon.damage + pLoadout.armor.damage + pLoadout.ring1.damage + pLoadout.ring2.damage;
    myStats.health = pHealth;
    return myStats;
}

// The cost for a load out

int GetLoadoutCost((Gear weapon, Gear armor, Gear ring1, Gear ring2) pLoadout)
{
    return pLoadout.weapon.cost + pLoadout.armor.cost + pLoadout.ring1.cost + pLoadout.ring2.cost;
}

// And a method to run the battle according to the rules of the game
bool RunBattle(Stats pPlayer, Stats pBoss)
{
    while (true)
    {
        int playerDmgDealt = Math.Max(1, pPlayer.damage - pBoss.armor);
        pBoss.health -= playerDmgDealt;
        //Console.WriteLine($"- The player deals {pPlayer.damage} - {pBoss.armor} = {playerDmgDealt} damage; the boss goes down to {pBoss.health} hit points.");
        if (pBoss.health <= 0) return true;

        int bossDmgDealth = Math.Max(1, pBoss.damage - pPlayer.armor);
        pPlayer.health -= bossDmgDealth;
        //Console.WriteLine($"- The boss deals {pBoss.damage} - {pPlayer.armor} = {bossDmgDealth} damage; the player goes down to {pPlayer.health} hit points.");
        if (pPlayer.health <= 0) return false;
    }
}

// ** Part 1:

void Part1_CheapestGearToWinTheFight()
{
    Stats bossStats = (stats["Hit Points"], stats["Damage"], stats["Armor"]);

    //Sort all configurations based on their cost from cheapest to most expensive
    gearConfigurations.Sort((a, b) => GetLoadoutCost(a).CompareTo(GetLoadoutCost(b)));

	//RunBattle return false if we lose, true if we win, so while not winning pick the next gear
    int gearIndex = 0;
    while (!RunBattle(GetStats(gearConfigurations[gearIndex], 100), bossStats)) gearIndex++;

    Console.WriteLine("Part 1: Won the battle for a cost of " + GetLoadoutCost(gearConfigurations[gearIndex]));
}

Part1_CheapestGearToWinTheFight();


// ** Part 2:

Part2_MostExpensiveGearToLoseTheFight();

void Part2_MostExpensiveGearToLoseTheFight()
{
    Stats bossStats = (stats["Hit Points"], stats["Damage"], stats["Armor"]);

    //sort all configurations based on their cost from most expensive to cheapest
    gearConfigurations.Sort((a, b) => GetLoadoutCost(b).CompareTo(GetLoadoutCost(a)));

    //RunBattle return false if we lose, true if we win, so while not losing pick the next gear
    int gearIndex = 0;
    while (RunBattle(GetStats(gearConfigurations[gearIndex], 100), bossStats)) gearIndex++;

    Console.WriteLine("Part 2: Lost the battle for a cost of " + GetLoadoutCost(gearConfigurations[gearIndex]));
}

