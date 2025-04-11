// Solution for https://adventofcode.com/2020/day/11 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a grid

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// For those of you that didn't recognize the challenge, this is essentially a modified
// version of Conway's Game of Life. The key thing here is that "rules are applied to every seat simultaneously".
// In other words, we need to use two grids, one for the lookup one for the final result...

Vec2i[] directions = [new(-1, -1), new(0, -1), new(1, -1), new(-1, 0), new(1, 0), new(-1, 1), new(0, 1), new(1, 1)];

int SimulatePart1 (string pSeatingArrangement)
{
    Grid<char> current = new Grid<char>(myInput, Environment.NewLine);
    Grid<char> next = new Grid<char>(current.width, current.height);
    
    int lastTotalOccupiedSeatCount = -1;

    while (true)
    {
        /**
        current.Print();
        Console.WriteLine();
        next.Print();
        Console.ReadKey();
        Console.Clear();
        /**/

        //Simulate one iteration
        int totalOccupiedSeatCount = 0;

        for (int x = 0; x < current.width; x++)
        {
            for (int y = 0; y < current.height; y++)
            {
                int occupiedNeighbouringSeats = GetOccupiedSeatCountPart1(current, new Vec2i(x, y));

                if (current[x,y] == 'L' && occupiedNeighbouringSeats == 0)
                {
                    next[x, y] = '#';
                }
                else if (current[x, y] == '#' && occupiedNeighbouringSeats >= 4)
                {
                    next[x, y] = 'L';
                }
                else
                {
                    next[x,y] = current[x, y];
                }

                if (next[x,y]== '#') totalOccupiedSeatCount++;
            }
        }

        //Swap the grids around for the next round...
        Grid<char> tmp = current;
        current = next;
        next = tmp;

        if (totalOccupiedSeatCount == lastTotalOccupiedSeatCount) break;
        lastTotalOccupiedSeatCount = totalOccupiedSeatCount;
    }

    return lastTotalOccupiedSeatCount;
}

int GetOccupiedSeatCountPart1 (Grid<char> pGrid, Vec2i pPosition)
{
    int occupiedSeatCount = 0;

    foreach (Vec2i direction in directions)
    {
        Vec2i position = pPosition + direction;
        if (pGrid.IsInside(position) && pGrid[position] == '#') { occupiedSeatCount++; }
    }
    //Console.WriteLine(occupiedSeatCount);
    return occupiedSeatCount;
}

Console.WriteLine("Part 1 - Occupied seat count:" + SimulatePart1(myInput));

// ** Part 2: Same but different (more complicated ;))

int SimulatePart2(string pSeatingArrangement)
{
    Grid<char> current = new Grid<char>(myInput, Environment.NewLine);
    Grid<char> next = new Grid<char>(current.width, current.height);

    int lastTotalOccupiedSeatCount = -1;

    while (true)
    {
        //Simulate one iteration
        int totalOccupiedSeatCount = 0;

        for (int x = 0; x < current.width; x++)
        {
            for (int y = 0; y < current.height; y++)
            {
                int occupiedNeighbouringSeats = GetOccupiedSeatCountPart2(current, new Vec2i(x, y));

                if (current[x, y] == 'L' && occupiedNeighbouringSeats == 0)
                {
                    next[x, y] = '#';
                }
                else if (current[x, y] == '#' && occupiedNeighbouringSeats >= 5)
                {
                    next[x, y] = 'L';
                }
                else
                {
                    next[x, y] = current[x, y];
                }

                if (next[x, y] == '#') totalOccupiedSeatCount++;
            }
        }

        //Swap the grids around for the next round...
        Grid<char> tmp = current;
        current = next;
        next = tmp;

        if (totalOccupiedSeatCount == lastTotalOccupiedSeatCount) break;
        lastTotalOccupiedSeatCount = totalOccupiedSeatCount;
    }

    return lastTotalOccupiedSeatCount;
}

int GetOccupiedSeatCountPart2(Grid<char> pGrid, Vec2i pPosition)
{
    int occupiedSeatCount = 0;

    foreach (Vec2i direction in directions)
    {
        Vec2i position = pPosition + direction;
        while (pGrid.IsInside(position) && pGrid[position] == '.') position += direction;
        if (pGrid.IsInside(position) && pGrid[position] == '#') { occupiedSeatCount++; }
    }
    //Console.WriteLine(occupiedSeatCount);
    return occupiedSeatCount;
}

Console.WriteLine("Part 2 - Occupied seat count:" + SimulatePart2(myInput));
