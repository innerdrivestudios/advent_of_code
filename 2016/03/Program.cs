//Solution for https://adventofcode.com/2016/day/3 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a sequence of triangle side length specifications

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

List<int[]> triangles =
    myInput.
    Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
    .Select(
        line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries)  // Split each line into numbers
        .Select(int.Parse)                                              // Convert to integer array
        .ToArray()
    )   
    .ToList();                                                          // Convert to list

Console.WriteLine("Part 1 - Number of possible triangles row based:" + GetNrOfPossibleTrianglesRowBased (triangles));
Console.WriteLine("Part 2 - Number of possible triangles column based:" + GetNrOfPossibleTrianglesColumnBased(triangles));
Console.ReadKey();

int GetNrOfPossibleTrianglesRowBased(List<int[]> pTriangles)
{
    return pTriangles.Where(
        triangle => triangle[0] < triangle[1] + triangle[2] &&
                    triangle[1] < triangle[0] + triangle[2] &&
                    triangle[2] < triangle[0] + triangle[1]
    ).Count();
}

int GetNrOfPossibleTrianglesColumnBased(List<int[]> pTriangles)
{
    //Apparently we've been doing it wrong in part 1 :)

    List<int[]> actualTriangles = new();
    
    //Go through all rows in sets of 3 (note the i = i + 3)
    for (int i = 0; i < pTriangles.Count; i = i+3)
    {
        //Interpret each column in the 3 row block of data as a triangle and add it to the actual triangle list
        for (int j = 0; j < 3; j++)
        {
            actualTriangles.Add([pTriangles[i][j], pTriangles[i + 1][j], pTriangles[i + 2][j]]);
        }
    }

    //Use part 1 again...
    return GetNrOfPossibleTrianglesRowBased(actualTriangles);
}


