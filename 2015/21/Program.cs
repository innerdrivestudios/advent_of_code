//Solution for https://adventofcode.com/2015/day/21 (Ctrl+Click in VS to follow link)

using Gear = (int cost, int damage, int armor);
using Stats = (int health, int damage, int armor);

//Would love to define using Loadout = (Gear, Gear, Gear, Gear) here, but we can't in top level C# :)

//Your input: a whole list of gear available from the gear shop

/*
Weapons: Cost Damage  Armor
Dagger        8     4       0
Shortsword   10     5       0
Warhammer    25     6       0
Longsword    40     7       0
Greataxe     74     8       0
*/

//You must use one weapon
List<Gear> weapons = new List<Gear> ()
{
	( 8,     4,       0),
	(10,     5,       0),
	(25,     6,       0),
	(40,     7,       0),
	(74,     8,       0),
};

/*
Armor:      Cost Damage  Armor
NoArmor      0      0       0
Leather      13     0       1
Chainmail    31     0       2
Splintmail   53     0       3
Bandedmail   75     0       4
Platemail   102     0       5
*/

//Armor is optional
List<Gear> armor = new List<Gear>()
{
	(  0 ,     0,       0),
	(  13,     0,       1),
	(  31,     0,       2),
	(  53,     0,       3),
	(  75,     0,       4),
	( 102,     0,       5)
};

/*
Rings:      Cost Damage  Armor
NoRing		  0     0       0
Damage +1    25     1       0
Damage +2    50     2       0
Damage +3   100     3       0
Defense +1   20     0       1
Defense +2   40     0       2
Defense +3   80     0       3
*/

//Rings are optional, but each ring is available only once
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

//Your task: calculate the cheapest gear to win the fight and the most expensive gear to lose the fight :)

//Generate all possible gear configurations
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

Part1_CheapestGearToWinTheFight();
Part2_MostExpensiveGearToLoseTheFight();

void Part1_CheapestGearToWinTheFight()
{
    //Boss stats
    //Hit Points: 103
    //Damage: 9
    //Armor: 2
    Stats bossStats = (103, 9, 2);

    //Sort all configurations based on their cost from cheapest to most expensive
    gearConfigurations.Sort((a, b) => GetLoadoutCost(a).CompareTo(GetLoadoutCost(b)));

	//RunBattle return false if we lose, true if we win, so while not winning pick the next gear
    int gearIndex = 0;
    while (!RunBattle(GetStats(gearConfigurations[gearIndex], 100), bossStats)) gearIndex++;

    Console.WriteLine("Won the battle for a cost of " + GetLoadoutCost(gearConfigurations[gearIndex]));
}

void Part2_MostExpensiveGearToLoseTheFight()
{
    //Boss stats
    //Hit Points: 103
    //Damage: 9
    //Armor: 2
    Stats bossStats = (103, 9, 2);

    //sort all configurations based on their cost from most expensive to cheapest
    gearConfigurations.Sort((a, b) => GetLoadoutCost(b).CompareTo(GetLoadoutCost(a)));

    //RunBattle return false if we lose, true if we win, so while not losing pick the next gear
    int gearIndex = 0;
    while (RunBattle(GetStats(gearConfigurations[gearIndex], 100), bossStats)) gearIndex++;

    Console.WriteLine("Lost the battle for a cost of " + GetLoadoutCost(gearConfigurations[gearIndex]));
}

Stats GetStats ((Gear weapon, Gear armor, Gear ring1, Gear ring2) pLoadout, int pHealth)
{
	Stats myStats;
	myStats.armor = pLoadout.weapon.armor+pLoadout.armor.armor+pLoadout.ring1.armor + pLoadout.ring2.armor;
	myStats.damage = pLoadout.weapon.damage + pLoadout.armor.damage + pLoadout.ring1.damage + pLoadout.ring2.damage;
	myStats.health = pHealth;
	return myStats;
}

int GetLoadoutCost ((Gear weapon, Gear armor, Gear ring1, Gear ring2) pLoadout)
{
	return pLoadout.weapon.cost + pLoadout.armor.cost + pLoadout.ring1.cost + pLoadout.ring2.cost;
}

//Run the battle according to the rules of the game
bool RunBattle (Stats pPlayer, Stats pBoss)
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

