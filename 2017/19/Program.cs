//Solution for https://adventofcode.com/2017/day/19 (Ctrl+Click in VS to follow link)

using System.Drawing;
using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a kind of trainline scheme

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();
Grid<char> trainLine = new Grid<char>(myInput, Environment.NewLine);

// Find the starting point which is supposed to be in the top row

Vec2i currentPosition = new Vec2i(0, 0);

trainLine.ForeachRegion(
    new Rectangle(0, 0, trainLine.width,1) ,
    (pos, value) =>
    {
        if (value == '|') currentPosition = pos;
    }
);

Console.WriteLine("Start position is:" + currentPosition);

// Setup the directions of travel and the starting direction

Directions<Vec2i> directions = new Directions<Vec2i>([new(1, 0), new(0, 1), new(-1, 0), new(0, -1)]);
directions.index = 1;

// ** Part 1 & 2: Just walk the trainline and collect data about the path...

(string sequence, int stepCount) RideTheTrain ()
{
    string sequence = "";
    int stepCount = 1;

    while (true)
    {
        currentPosition = RideOneStep (currentPosition);
        if (char.IsAsciiLetterUpper(trainLine[currentPosition])) sequence += trainLine[currentPosition];
        if (char.IsWhiteSpace(trainLine[currentPosition])) break;

        stepCount++;
    }

    return (sequence, stepCount);
}

Vec2i RideOneStep (Vec2i pCurrentPosition)
{
    Vec2i newPosition = pCurrentPosition + directions.Current();

    //Do I need to turn left or right?
    Vec2i leftPosition = newPosition + directions.Get (directions.index-1);
    Vec2i rightPosition = newPosition + directions.Get (directions.index+1);

    //If we are at a crossing this will cause us to turn left and right :) eg --> straight on
    if (trainLine.IsInside (leftPosition) && trainLine[leftPosition] != ' ') directions.index--;
    if (trainLine.IsInside (rightPosition) && trainLine[rightPosition] != ' ') directions.index++;

    return newPosition;
}

var walkResult = RideTheTrain();

Console.WriteLine("Part 1:" + walkResult.sequence);
Console.WriteLine("Part 2:" + walkResult.stepCount);

