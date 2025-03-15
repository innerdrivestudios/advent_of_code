//Solution for https://adventofcode.com/2017/day/3 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// ** Your input: a number from a spiral grid, for which you need to answer some questions
int myInput = int.Parse(args[0]);

// ** Your task:
// 1 - Calculate the Manhattan Distance of the given number from the center of a spiral grid (see below)
// 2 - Calculate the first number which is bigger than the given number

// There are different ways to approach this... for example see the excel_fun file in this folder.
// However, for part 2 it turned out that actually generating the spiral pattern is by far the easiest way.
//
// One interesting thing however is determining how big the grid for a given number has to be: 
// 
// 65	64	63	62	61	60	59	58	57
// 66	37	36	35	34	33	32	31	56
// 67	38	17	16	15	14	13	30	55
// 68	39	18	5	4	3	12	29	54
// 69	40	19	6	1	2	11	28	53
// 70	41	20	7	8	9	10	27	52
// 71	42	21	22	23	24	25	26	51
// 72	43	44	45	46	47	48	49	50
// 73	74	75	76	77	78	79	80	81
// 
// If you look at the outermost ring for example, we see a highest and lowest value:
// - 50 and 81
// 
// The square root of 49 is 7 (which is we can see is the correct amount of cells for a grid with numbers up to 49)
// The square root of 81 is 9 (which is we can see is the correct amount of cells for the grid with numbers up to 81)
// The square root of 50 is 7,071... so yeah... what do we do with this one:)
// Simple: Round up the result and if it is odd, add 1.

int size = (int)Math.Ceiling(Math.Sqrt(myInput));
if (size % 2 == 0) size++;

//Set up some directions and note we are going to the right first...
Directions<Vec2i> directions = new Directions<Vec2i>([new Vec2i(1, 0), new Vec2i(0, -1), new Vec2i(-1, 0), new Vec2i(0,1)]);
directions.index = 0;

//Some other common values we'll need for both parts of the puzzle... 
Vec2i centerPoint = (new Vec2i(size, size)) / 2;
Console.WriteLine("Part 1 - Manhattan Distance for input: " + GetManhattanDistanceForInput(myInput));
Console.WriteLine("Part 2 - Get first summed value bigger than input: " + GetFirstSummedValueLargerThanInput(myInput));

int GetManhattanDistanceForInput (int pInput)
{
    Grid<int> spiralGrid = new Grid<int>(size, size);
    Vec2i cellLocation = centerPoint;
    int cellValue = 1;

    spiralGrid[cellLocation] = cellValue++;

    //Move to the right, to start off it the right direction...
    cellLocation += directions.Current();

    while (spiralGrid.IsInside(cellLocation))
    {
        spiralGrid[cellLocation] = cellValue;

        if (cellValue == pInput)
        {
            return (cellLocation - centerPoint).ManhattanDistance();
        }

        //Look to the left of me...
        Vec2i leftOfMe = cellLocation + directions.Get(directions.index + 1);
        //If it is empty, turn left...
        if (spiralGrid[leftOfMe] == 0) directions.Next();
        //Move left
        cellLocation += directions.Current();
        cellValue++;
    }

    return -1;
}

int GetFirstSummedValueLargerThanInput (int pInput)
{
    Grid<int> spiralGrid = new Grid<int>(size, size);
    Vec2i cellLocation = centerPoint;
    spiralGrid[cellLocation] = 1;
    //Move to the right, to start off it the right direction...
    cellLocation += directions.Current();

    while (spiralGrid.IsInside(cellLocation))
    {
        spiralGrid[cellLocation] = GetValueSumOfNeighbours(spiralGrid, cellLocation);

        if (spiralGrid[cellLocation] > pInput)
        {
            return spiralGrid[cellLocation];
        }

        //Look to the left of me...
        Vec2i leftOfMe = cellLocation + directions.Get(directions.index + 1);
        //If it is empty, turn left...
        if (spiralGrid[leftOfMe] == 0) directions.Next();
        //Move left
        cellLocation += directions.Current();
    }

    return -1;
}

int GetValueSumOfNeighbours(Grid<int> pGrid, Vec2i pLocation)
{
    int sum = 0;

    for (int x = pLocation.X - 1; x <= pLocation.X + 1; x++)
    {
        for (int y = pLocation.Y - 1; y <= pLocation.Y + 1; y++)
        {
            sum += pGrid.IsInside(x, y) ? pGrid[x, y] : 0;
        }
    }

    return sum;
}

