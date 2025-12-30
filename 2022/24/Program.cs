// Solution for https://adventofcode.com/2022/day/24 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;
using Blizzard = (Vec2<int> position, Vec2<int> velocity);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

//** Your input: a map describing a valley with blizzards

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

Grid<char> map = new Grid<char>(myInput, Environment.NewLine);

//For creating the blizzards from the map
Dictionary<char, Vec2i> char2VelocityMap = new()
{
    {'>', new Vec2i(1,0) },
    {'^', new Vec2i(0,-1) },
    {'<', new Vec2i(-1,0) },
    {'v', new Vec2i(0,1) }
};

//For debugging
Dictionary<Vec2i, char> velocity2CharMap = new()
{
    {new Vec2i(1,0) , '>'},
    {new Vec2i(0,-1), '^' },
    {new Vec2i(-1,0), '<' },
    {new Vec2i(0,1) , 'v'}
};

// Look up the blizzards...
List<Blizzard> blizzards = new();

map.Foreach(
    (pos, value) =>
    {
        if (!char2VelocityMap.ContainsKey(map[pos])) return;

        blizzards.Add((pos, char2VelocityMap[map[pos]]));
        //clear it out for the next step in the process...
        map[pos] = '.';
    }
);

// Now determine after how many minutes the world will repeat:
// we need to look at the movement distance over x and y in the map without the borders...
int lcm = NumberUtil.LCM (map.width-2, map.height-2);

// And now we'll create this many copies of the grid/world, while simulating each step of the way
// This will give us a very fast look up table for when we actually start moving through the valley...

// For debugging purposes we'll replicate the original setup:

Grid<char>[] states = new Grid<char>[lcm];

for (int i = 0; i < lcm; i++)
{
    //Empty map clone...
    states[i] = map.Clone();

    for (int j = 0; j < blizzards.Count; j++)
    {
        Blizzard blizzard = blizzards[j];

        if (states[i][blizzard.position] == '.')
        {
            states[i][blizzard.position] = velocity2CharMap[blizzard.velocity];
        }
        else
        {
            states[i][blizzard.position] = 'M';
        }

        // Simulate da blizzzzz
        blizzard.position += blizzard.velocity;
        blizzard.position.X = NumberUtil.Mod (blizzard.position.X - 1, map.width - 2) + 1;
        blizzard.position.Y = NumberUtil.Mod (blizzard.position.Y - 1, map.height - 2) + 1;

        blizzards[j] = blizzard;
    }

    Console.Clear();
    states[i].Print();
    Console.ReadKey();
}

// Now we have all of our states per time step and can start the search!

Vec2i start = new Vec2i (1, 0);
Vec2i end = new Vec2i (map.width-2, map.height-1);
//map[start] = 'S';
//map[end] = 'E';
//map.Print("");

int GetFewestMinutesToReachTheGoal(int pStartTime, Vec2i pStart, Vec2i pEnd)
{
    PriorityQueue<Vec2i, int> queue = new();
    queue.Enqueue(pStart, 0);

    Vec2i[] directions = [new(1, 0), new(0, 1), new(-1, 0), new (0,-1), new (0,0)];

    HashSet<string> visited = new();
    visited.Add("" + pStart + 0);

    while (queue.Count > 0)
    {
        queue.TryPeek(out Vec2i current, out int cost);
        queue.Dequeue();
        //subtract one since 1 is the first minute...
        if (current == pEnd) return cost-1;

        foreach (Vec2i direction in directions)
        {
            Vec2i newPosition = current + direction;
            int newCost = cost + 1;

            Grid<char> currentMap = states[(cost + pStartTime) %  states.Length];
            if (!currentMap.IsInside(newPosition) || currentMap[newPosition] != '.') continue;

            string key = "" + newPosition + newCost;
            if (visited.Contains(key)) continue;

            visited.Add(key);
            queue.Enqueue(newPosition, newCost);
        }

    }

    return -1;
}

int toEnd = GetFewestMinutesToReachTheGoal(0, start, end);

Console.WriteLine("Part 1: " + toEnd);

int backToStart = GetFewestMinutesToReachTheGoal(toEnd, end, start);
int backToEnd = GetFewestMinutesToReachTheGoal(toEnd + backToStart, start, end);

//Console.WriteLine("Part 2: " + backToStart);
//Console.WriteLine("Part 2: " + backToEnd);

Console.WriteLine("Part 2: " + (toEnd + backToStart + backToEnd));
