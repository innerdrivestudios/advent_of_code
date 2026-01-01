// Solution for https://adventofcode.com/2022/day/19 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of blueprints...

string[] myInput = File.ReadAllLines(args[0]);

List<Blueprint> blueprints = new();

foreach (string input in myInput)
{
    blueprints.Add(new Blueprint(input));
}

// ** Part 1 : Figure out which blueprint would maximize the number of opened geodes
// after 24 minutes by figuring out which robots to build and when to build them.

long GetMaxOpenGeodeCount (Blueprint pBlueprint, int pMaxTime)
{
    //Enqueue the default state in which we don't have anything except one ore collecting robot...
    PriorityQueue<State, int> queue = new();
    queue.Enqueue(new State(pBlueprint, new ResourceDefinition(0,0,0,0), 1,0,0,0,0), 0);

    HashSet<string> visited = new HashSet<string>();

    int max = 0;

    while (queue.Count > 0)
    {
        State state = queue.Dequeue();

        //if (bestState != null && state.IsWorseThan(bestState))
        //{
           // Console.WriteLine("Skipped worse state");
        //    continue;
        //}
       // bestState = state;

        if (state.time >= pMaxTime)
        {
            if (state.inventory.geode > max)
            {
               // Console.WriteLine(state.inventory.geode);
                max = state.inventory.geode;
            }

            continue;
        }

        // Get the state on our list... simulate another minute of its life, without modifying the old state...
        State newState = state;

        IEnumerable<State> possibleNextStates = newState.GetPossibleNextStates();
        foreach (State nextState in possibleNextStates)
        {
            nextState.SimulateOneMinute();

            if (visited.Contains(nextState.ToString()))
            {
                continue;
            }
            visited.Add(nextState.ToString());

            //if (nextState.IsWorseThan(bestState)) continue;

            queue.Enqueue(nextState, nextState.GetScore());
            //Console.WriteLine(nextState);
        }

       // Console.ReadKey();


    }

    return max;
}

// Not too happy with the performance yet, but at least it gives the correct answers...
Console.WriteLine("Please wait until all calculations complete (which takes several minutes at least)");

long total = 0;

foreach (Blueprint blueprint in blueprints)
{
    total += blueprint.id * GetMaxOpenGeodeCount(blueprint, 24);
    //Console.WriteLine(total);
}

Console.WriteLine("Part 1: " + total);


long part2 = GetMaxOpenGeodeCount(blueprints[0], 32) * GetMaxOpenGeodeCount(blueprints[1], 32) * GetMaxOpenGeodeCount(blueprints[2], 32);
Console.WriteLine("Part 2: " + part2);
