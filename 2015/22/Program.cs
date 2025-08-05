//Solution for https://adventofcode.com/2015/day/22 (Ctrl+Click in VS to follow link)

//Contrary to most other challenges, I decided to first try and go full blown OO on this.
//Even though it might be possible to finish this challenge with dynamic tuples etc, can't imagine it will be very readable.
//Little bit verbose, but pretty fast in the end.

//Setup the basic given data:

using System.Diagnostics;

Player player = new Player(50, 0, 500);
player.AddSpell(new MagicMissile());
player.AddSpell(new Drain());
player.AddSpell(new Shield());
player.AddSpell(new Poison());
player.AddSpell(new Recharge());

Boss boss = new Boss (51, 0, 9);

Battle battle = new Battle(player, boss);

//First thing I did was build an interactive version to play around for testing...
//RunInteractiveBattle();

Console.WriteLine("");

Stopwatch stopwatch = Stopwatch.StartNew();

//Second step, try a brute force approach, which works ok-ish, but still fails occasionally (numbers are too high)...

Console.WriteLine("Brute force through a 10000 random battles");
Console.WriteLine("============================================");

Console.WriteLine("Mana needed with 0 additional player dmg per turn:");
RunBruteForcePuzzleSolver(10000, 0);
Console.WriteLine("Solved in: "+stopwatch.ElapsedMilliseconds + " milliseconds");
stopwatch.Restart();

Console.WriteLine("Mana needed with 1 additional player dmg per turn:");
RunBruteForcePuzzleSolver(10000, 1);
Console.WriteLine("Solved in: " + stopwatch.ElapsedMilliseconds + " milliseconds");
stopwatch.Restart();

Console.WriteLine("");

//3rd try is a charm: Dijkstra approach, which is way faster and more accurate...

Console.WriteLine("Dijkstra:");
Console.WriteLine("============================================");
Console.WriteLine("Mana needed with 0 additional player dmg per turn:");
RunDijkstraPuzzleSolver(0);
Console.WriteLine("Solved in: " + stopwatch.ElapsedMilliseconds + " milliseconds");
stopwatch.Restart();

Console.WriteLine("Mana needed with 1 additional player dmg per turn:");
RunDijkstraPuzzleSolver(1);
Console.WriteLine("Solved in: " + stopwatch.ElapsedMilliseconds + " milliseconds");
stopwatch.Restart();

/*
void RunInteractiveBattle()
{
    //Make sure different battles don't interfere with each other
    battle = battle.Clone();

    Log.WriteLine("********************************************");
    Log.WriteLine("************** NEW FIGHT STARTED ***********");
    Log.WriteLine("********************************************");
    Log.WriteLine();

    while (battle.GetBattleStatus() == Battle.Winner.Undecided)
    {
        //Run a single turn where the player's part is an interactive casting sequence
        battle.RunSingleTurn(battle.InteractivePlayerCastingSequence);
    }

    Console.WriteLine("\nWinner of the fight: " + battle.GetBattleStatus());
}
*/

void RunBruteForcePuzzleSolver (int pBattleCount, int pDamagePerTurn)
{
    int manaSpent = int.MaxValue;
    battle.extraDamagePerTurn = pDamagePerTurn;
    Log.enabled = false;

    int assumeLowestManaAfterTurn = pBattleCount;
    int currentTurn = 0;

    while (currentTurn < assumeLowestManaAfterTurn)
    {
        //Clone the basic fight so we can rerun it a zillion times
        Battle battleClone = battle.Clone();

        Log.WriteLine("********************************************");
        Log.WriteLine("************** NEW FIGHT STARTED ***********");
        Log.WriteLine("********************************************");
        Log.WriteLine();

        while (battleClone.GetBattleStatus() == Battle.Winner.Undecided)
        {
            //Run a single turn where the player's part is an auto random casting sequence
            battleClone.RunSingleTurn(battleClone.AutoRandomCastingSequence);
        }

        Log.WriteLine("\nWinner of the fight: " + battleClone.GetBattleStatus());

        //If the player won, store this as the new value to beat, and reset the current turn count, 
        //so we can play another x games to try and beat this value
        if (battleClone.GetBattleStatus() == Battle.Winner.Player && battleClone.totalManaSpent < manaSpent)
        {
            manaSpent = battleClone.totalManaSpent;
            currentTurn = 0;
        }

        currentTurn++;
    }

    Console.WriteLine("Mana:" + manaSpent);
}


void RunDijkstraPuzzleSolver (int pDamagePerTurn)
{
    Log.enabled = false;
    
    PriorityQueue <Battle, int> priorityQueue = new PriorityQueue <Battle, int>();

    Battle startBattle = battle.Clone();
    startBattle.extraDamagePerTurn = pDamagePerTurn;

    priorityQueue.Enqueue(startBattle, startBattle.totalManaSpent);

    //Will use the battle to generate a unique string to avoid testing duplicate states
    HashSet<string> visited = new HashSet<string>();

    while (priorityQueue.Count > 0)
    {
        Battle currentBattle = priorityQueue.Dequeue();
        visited.Add(currentBattle.ToString());

        if (currentBattle.GetBattleStatus() == Battle.Winner.Player)
        {
            Console.WriteLine("Mana:" + currentBattle.totalManaSpent);
            break;
        }
        else if (currentBattle.GetBattleStatus() == Battle.Winner.Undecided)
        {
            //we need to make the player cast all possible spells at the exact right moment (after applying spell effects...)
            //which is hard, since it is in the RunSingleTurn loop, so we'll need to try them all...

            for (int i = 0; i < currentBattle.player.spellCount; i++)
            {
                Battle newBattle = currentBattle.Clone();
                newBattle.RunSingleTurn(() => newBattle.ForceSpellCast(i));

                string uniqueID = newBattle.ToString();

                //This was my first approach, just looking at the totalManaSpent...

                //if (!visited.Contains(uniqueID) && newBattle.GetBattleStatus() != Battle.Winner.Boss && newBattle.totalManaSpent < lowest) { 
                //    priorityQueue.Enqueue(newBattle, newBattle.totalManaSpent);
                //    visited.Add(uniqueID);
                //}

                //But then I realized, we'd like to prioritize lower boss health as well... 
                
                if (!visited.Contains(uniqueID) && newBattle.GetBattleStatus() != Battle.Winner.Boss) 
                { 
                    priorityQueue.Enqueue(newBattle, newBattle.totalManaSpent * newBattle.boss.health);
                    visited.Add(uniqueID);
                }
            }
        }
    }
}


