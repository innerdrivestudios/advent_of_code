//Solution for https://adventofcode.com/2018/day/20 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: A "regular" expression describing a map :)

// ** Step 1: Parse the input 

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings("").Trim();

// Implement a helper method to walk the regular expression...
// Note that for now, we don't care about the type of doors we encounter

HashSet<Vec2i> Walk (string pInput)
{
    int index = 0;
    
    HashSet<Vec2i> walkableSpaces = new ();
    Vec2i currentPosition = new Vec2i(0, 0);
    walkableSpaces.Add (currentPosition);

    Stack<Vec2i> history = new Stack<Vec2i>();

    Dictionary<char, Vec2i> lookupTable =new() {
        {'N', new Vec2i(0, -1) },
        {'S', new Vec2i(0, 1) },
        {'W', new Vec2i(-1, 0) },
        {'E', new Vec2i(1, 0) },
    };

    while (index < pInput.Length)
    {
        char c = pInput[index];

        //If the char indicates movement take two steps...
        if (lookupTable.ContainsKey(c))
        {
            Vec2i direction = lookupTable[c];

            for (int i = 0; i < 2; i++)
            {
                currentPosition += direction;
                walkableSpaces.Add(currentPosition);
            }
        }
        //If we encounter a ( we need to be able to return to where we currently are
        else if (c == '(') {
            history.Push(currentPosition);
        }
        //If we encounter | we return to where we started when we encounted the ( but
        //we might need to do it multiple times so we only peek don't pop
        else if (c == '|')
        {
            currentPosition = history.Peek();
        }
        //All done with the current expression, pop where we were
        else if (c == ')')
        {
            currentPosition = history.Pop();
        }

        index++;
    }

    return walkableSpaces;
}

HashSet<Vec2i> walkableSpaces = Walk(myInput);

Vec2i min = Vec2i.Min(walkableSpaces);
Vec2i max = Vec2i.Max(walkableSpaces);

//The size is the max minus the min + (1,1) since coordinates are zero based,
//so the grid needs to be one bigger + (2,2) for the borders
Vec2i size = max - min + new Vec2i(3, 3);

//The offset is undoing the min plus (1,1) to skip the border
Vec2i offset = -min + new Vec2i(1,1);

Grid<char> area = new Grid<char> (size.X, size.Y);
area.Foreach(
    (position, value) =>
    {
        area[position] = walkableSpaces.Contains(position - offset)?'.':'#';
    }
);

//area.Print();

// ** Part 1: What is the walking distance to the furthest room?
// Note this is simply the furthest distance of a BFS search.

// Wrap our grid with an adapter that treats it like a simple graph, where . is walkable
GraphGridAdapter gga = new GraphGridAdapter(area, ['.']);
// Our start is 0,0, but we need to offset it again
Dictionary<Vec2i, long> costMap = SearchUtils.FlowMap<Vec2i>(gga, new Vec2i(0,0) + offset);

// So let's say our hallway is like this:
// 1 2 3 4 5 6 7
// Then our cost would be like this:
// 0 1 2 3 4 5 6
// But our doors would be like this:
// . | . | . | .
// In other words, we need to divide the end result (6 in this case) by 2 to get the amount of doors...

Console.WriteLine("Part 1:" + costMap.Values.Max() / 2);

// ** Part 2: NICE, but tricky :)

// So, we first need to filter out all rooms (since not all values in our flow map are rooms, there are also doors in there)
// Our original (0,0) has been moved in the grid, so we need to add the offset to everything, so we know we can look for even values 
// on both X and Y (rooms)
Dictionary<Vec2i, long> roomCostMap = costMap.Where(x => (x.Key.X + offset.X) % 2 == 0 && (x.Key.Y + offset.Y) % 2 == 0).ToDictionary();

// Then we take all rooms whose cost / 2 is >= 1000 like before and count them
Console.WriteLine("Part 2:" + roomCostMap.Values.Where (x => x/2 >= 1000).Count());