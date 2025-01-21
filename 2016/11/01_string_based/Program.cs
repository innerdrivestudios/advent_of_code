//Solution for https://adventofcode.com/2016/day/11 (Ctrl+Click in VS to follow link)

using System.Diagnostics;

////////////////////////////////////////// YOUR INPUT ///////////////////////////////////

// A list of floors and items they contain (different radio active generators and matching chips):
// - The first floor contains a thulium generator, a thulium-compatible microchip, a plutonium generator, and a strontium generator.
// - The second floor contains a plutonium-compatible microchip and a strontium-compatible microchip.
// - The third floor contains a promethium generator, a promethium-compatible microchip, a ruthenium generator, and a ruthenium-compatible microchip.
// - The fourth floor contains nothing relevant.

////////////////////////////////////////// YOUR TASKS ////////////////////////////////////

//  I - Calculate the minimum number of steps required to bring all of the objects to the fourth floor,
//      whle obeying the item "rules" set out before you.
//  II - Do the same for some additional objects :)

///////////////////////////////////////// APPROACH //////////////////////////////////////

// Read the puzzle description on the website first and then continue with everything below.

// The puzzle in question is kind of like a "Goat, Cabbage and the Wolf" puzzle:
//  - A farmer is on the left side of the river with a Goat, Cabbage and Wolf and needs to bring all of them across.
//    However you can't leave the Goat with the Cabbage, nor the Wolf with the Goat, etc, etc.
//
// Instead of Goats etc we're dealing with an elevator, generators and chips that do or don't combine :))

// The elevator in the problem description is key here because it defines the possible state transitions.

// Basically it answers the questions:
// - where are we (FloorStart)
// - where can we go (FloorEnd)
// - what can/should we bring along for the ride?

// Also note:
// - the elevator always moves only one floor at a time! So the delta Abs(Fstart - Fend) == always 1.
// - we need to bring at least 1, and at most 2 items
// - we can't leave or bring any floor in an invalid state (a state where we'd fry one of the chips)

// In other words: all possible NEXT states are determined by three things:
// - the "moves" you could make in theory from one floor to the next (the items you COULD bring)
// - what items you are left with on Fstart need to be "valid"
// - what items you end up with on Fend need to be "valid"

// The moves you can make in theory are constructed by going from left to right through the elements on a floor and
// - adding what you encounter as a possible "single" item move
// - for every "single" item move you go through all the possible items to the right of that

// So for example, if the elevator is on F1  and F1 contains: HG  HM LG  LM 
// we could in theory move:

// HG
// 	HG, HM
// 	HG, LG
// 	HG, LM
// HM
// 	HM, LG
// 	HM, LM
// LG
// 	LG, LM
// LM

// to the floor above or below the current floor.

// Each of those moves must leave you with a valid Fstart and Fend, otherwise you skip the move.
// (States can be tested on the fly or cached for optimization).

// Note that the order of elements don't matter, so we can just represent them with a HashSet<string>

// So what is a invalid state?
// Any state that has AT LEAST 1 generator, where there is AT LEAST 1 chip without its own generator.
// (Rationale: is that generator A will fry chip B if chip B is not powered by generator B)

///////////////////////////////////////// REPRESENTING THE PUZZLE STATE /////////////////////////////

// Basically what we want to be able to do is:
// - Represent the puzzle state
// - Be to get the "possible moves" for that state
// - Be able to get an altered (cloned) puzzle state by making a certain move (aka "expanding" a puzzle state)
// 
// Ideally we would also be able to score our puzzle states to prioritize which puzzle states to expand first
// (we can incorporate this score as a cost in our search algorithm)
//
// In theory I think we could so something smart with representing (HG, HM) (LG, LM) etc per floor as sets of 2 bits...
// but I chose to go with something more readable instead (might do the other version afterwards).
//
// Update: bit based version can be found in the same parent folder as this solution.

// First let's mimick the test data:

// F4   .  .  .  .  
// F3   .  .  LG . 
// F2   HG .  .  .
// F1 E .  HM .  LM

RTGFacility testFacility = new RTGFacility();
testFacility.AddFloor(["HM", "LM"]);
testFacility.AddFloor(["HG"]);
testFacility.AddFloor(["LG"]);
testFacility.AddFloor([]);

Console.WriteLine("Test input - Moves required:" + RunDijkstra(testFacility));

// The first floor contains a thulium generator, a thulium-compatible microchip, a plutonium generator, and a strontium generator.
// The second floor contains a plutonium-compatible microchip and a strontium-compatible microchip.
// The third floor contains a promethium generator, a promethium-compatible microchip, a ruthenium generator, and a ruthenium-compatible microchip.
// The fourth floor contains nothing relevant.

RTGFacility part1Facility = new RTGFacility();
part1Facility.AddFloor(["TG", "TM", "PG", "SG"]);
part1Facility.AddFloor(["PM", "SM"]);
part1Facility.AddFloor(["pG", "pM", "RG", "RM"]);   //p is promethium, P is plutonium :)
part1Facility.AddFloor([]);

Console.WriteLine("Part 1 - Moves required:" + RunDijkstra(part1Facility));

RTGFacility part2Facility = new RTGFacility();
part2Facility.AddFloor(["TG", "TM", "PG", "SG", "EG", "EM", "DG", "DM"]);
part2Facility.AddFloor(["PM", "SM"]);
part2Facility.AddFloor(["pG", "pM", "RG", "RM"]);   //p is promethium, P is plutonium :)
part2Facility.AddFloor([]);

Console.WriteLine("Part 2 - Moves required:" + RunDijkstra(part2Facility));

int RunDijkstra (RTGFacility pInputFacility)
{
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    
    Console.WriteLine("Dijkstra started...");

    PriorityQueue<RTGFacility, int> todoList = new();
    todoList.Enqueue(pInputFacility, 0);

    HashSet<string> visited = new HashSet<string>();

    while (todoList.Count > 0)
    {
        RTGFacility current = todoList.Dequeue();
        //Slow... but can't thing of a faster way using this setup...
        visited.Add(current.ToString());

        if (current.IsSolution())
        {
            Console.WriteLine("Dijkstra completed in ..." + (stopwatch.ElapsedMilliseconds/1000f) +" seconds...");
            return current.movesMade;
        }

        List<Move> possibleMoves = current.GetPossibleMoves();

        for (int i = 0; i < possibleMoves.Count; i++)
        {
            RTGFacility newState = current.MakeMove(possibleMoves[i]);
            if (!visited.Contains(newState.ToString()))
            {
                //GetRating is basically A*, it combines the current cost and estimate
                todoList.Enqueue(newState, newState.GetRating());
            }
        }
    }

    return -1;
}
