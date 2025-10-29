//Solution for https://adventofcode.com/2018/day/24 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of battle specifications

string[] myInput = File.ReadAllLines(args[0]);

// ** Parse the input:

HashSet<Squad> infectionTeamSet = new ();
HashSet<Squad> immuneSystemTeamSet = new ();

HashSet<Squad> currentGroup = null;
Squad.Type currentType = Squad.Type.Immune;

foreach (string line in myInput)
{
    if (line.StartsWith("Immune")) { 
        currentGroup = immuneSystemTeamSet; 
        currentType = Squad.Type.Immune;
        continue; 
    }

    if (line.StartsWith("Infection")) { 
        currentGroup = infectionTeamSet; 
        currentType = Squad.Type.Infection;
        continue; 
    }

    if (string.IsNullOrEmpty(line)) continue;

    Squad squad = new Squad(line, currentType, currentGroup.Count+1);
    currentGroup.Add(squad);
}

// ** Part 1:

// Note: there is a fair amount of slow LINQ and datastructure conversion going on,
// but seeing the battle ran fast enough I decided to leave it this way.

(List<Squad> winningTeam, Squad.Type winningTeamType) RunBattle (int pBoost)
{
    List<Squad> allGroups = new();
    allGroups.AddRange(infectionTeamSet);
    allGroups.AddRange(immuneSystemTeamSet);

    // This is for part 2
    immuneSystemTeamSet.ToList().ForEach (x => { x.boost = pBoost; });
    allGroups.ForEach(x => x.Reset());

    while (true)
    {
        allGroups.ForEach(x => x.chosen = false);
        allGroups.ForEach(x => x.currentTarget = null);

        allGroups.Sort(OverallSquadSelectionOrder);

        //Little bit brute force for now ;)
        if (allGroups.Select(x => x.type).Distinct().Count() == 1) break;

        /*
        Console.WriteLine("Immune System:");
        foreach (Squad squad in immuneTeamList)
        {
            Console.WriteLine("Group " + squad.id + " contains " + squad.unitCount + " units");
        }

        Console.WriteLine("Infection:");
        foreach (Squad squad in infectionTeamList)
        {
            Console.WriteLine("Group " + squad.id + " contains " + squad.unitCount + " units");
        }

        Console.ReadKey();
        */

        int unitCountBeforeBattle = allGroups.Sum(x => x.unitCount);

        foreach (Squad squad in allGroups)
        {
            SelectTarget(squad);
        }

        allGroups.Sort(InitiativeSelectionOrder);

        foreach (Squad squad in allGroups)
        {
            Attack(squad);
        }

        allGroups = allGroups.Where(x => x.unitCount > 0).ToList();
        int unitCountAfterBattle = allGroups.Sum(x => x.unitCount);
        if (unitCountBeforeBattle == unitCountAfterBattle) return (null, Squad.Type.None);
    }

    return (allGroups, allGroups[0].type);
}

int OverallSquadSelectionOrder (Squad pSquadA, Squad pSquadB)
{
    if (pSquadA.effectivePower == pSquadB.effectivePower) return pSquadB.initiative - pSquadA.initiative;
    return pSquadB.effectivePower - pSquadA.effectivePower;
}

int InitiativeSelectionOrder (Squad pSquadA, Squad pSquadB)
{
    return pSquadB.initiative - pSquadA.initiative;
}

void SelectTarget (Squad pAttackingSquad)
{
    HashSet<Squad> opponents = immuneSystemTeamSet.Contains(pAttackingSquad) ? infectionTeamSet : immuneSystemTeamSet;
    List<Squad> targets = opponents.ToList ();

    int mostDamage = int.MinValue;
    Squad bestTarget = null;

    if (pAttackingSquad.unitCount == 0) return;

    foreach (Squad target in targets)
    {
        if (target.chosen) continue;

        int dmgDealt = GetDamageDealt(pAttackingSquad, target);

        //Console.WriteLine(pAttackingSquad.type + " group " + pAttackingSquad.id + " would deal defending group " + target.id + " " + dmgDealt + " damage");
        //Console.ReadKey();

        if (dmgDealt >= mostDamage && dmgDealt > 0)
        {
            if (dmgDealt > mostDamage)
            {
                mostDamage = dmgDealt;
                bestTarget = target;
            }
            else if (target.effectivePower >= bestTarget.effectivePower)
            {
                if (target.effectivePower > bestTarget.effectivePower)
                {
                    mostDamage = dmgDealt;
                    bestTarget = target;
                }
                else if(target.initiative > bestTarget.initiative)
                {
                    mostDamage = dmgDealt;
                    bestTarget = target;
                }
            }

        }
    }

    pAttackingSquad.currentTarget = bestTarget;
    if (bestTarget != null)
    {
        bestTarget.chosen = true;
        //Console.WriteLine(pAttackingSquad.type + " group " + pAttackingSquad.id + " chose " + bestTarget.id);
        //Console.ReadKey();
    }
}

int GetDamageDealt (Squad pAttackingSquad, Squad pDefendingSquad)
{
    int damageDealt = pAttackingSquad.effectivePower;

    if (pDefendingSquad.immunities.Contains(pAttackingSquad.damageType))
    {
        damageDealt *= 0;
    }
    else if (pDefendingSquad.weaknesses.Contains(pAttackingSquad.damageType))
    {
        damageDealt *= 2;
    }

    return damageDealt;
}

void Attack (Squad pAttackingSquad)
{
    if (pAttackingSquad.currentTarget == null) return;
    if (pAttackingSquad.unitCount == 0) return;

    int dmgDealt = GetDamageDealt (pAttackingSquad, pAttackingSquad.currentTarget);
    int unitsKilled = dmgDealt / pAttackingSquad.currentTarget.hitPointsPerUnit;
    pAttackingSquad.currentTarget.unitCount -= unitsKilled;

    /*
    Console.WriteLine(
        (immuneSystemTeamSet.Contains(pAttackingSquad) ? "Immune System":"Infection") + " group "+ pAttackingSquad.id +
        " attacks defending group " + pAttackingSquad.currentTarget.id + ", killing " + unitsKilled + " units"
     );

    Console.ReadKey();
    */
}

var battleResult = RunBattle(0);

Console.WriteLine("Part 1:" + battleResult.winningTeam.Sum(x => x.unitCount) + " Winning team:" + battleResult.winningTeamType);

Console.WriteLine();

// Linear search, binary search would be better but seems overkill here...

int boost = 0;
while (battleResult.winningTeamType != Squad.Type.Immune)
{
    Console.WriteLine("Running battle with boost:" + boost);
    battleResult = RunBattle(++boost);

    if (battleResult.winningTeamType != Squad.Type.None)
    {
        Console.WriteLine("Surviving units:" + battleResult.winningTeam.Sum(x => x.unitCount) + " Winning team:" + battleResult.winningTeamType);
    }
    else
    {
        Console.WriteLine("Impasse");
    }
}

Console.WriteLine("Part 2: Surviving units: " + battleResult.winningTeam.Sum(x => x.unitCount) + " Winning team: " + battleResult.winningTeamType);