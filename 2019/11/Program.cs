// Solution for https://adventofcode.com/2019/day/11 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of program lines that represent opcode and parameters

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings("");

// This puzzle reuses the existing IntCode computer from day 9,
// which reuses the IntCode computer from day 5,
// which reuses the existing IntCode computer from day 2 :)

// In other words, good opportunity to take those existing
// IntCode computer setups and document them some more.
//
// Previous IntCode computers:
// https://adventofcode.com/2019/day/2
// https://adventofcode.com/2019/day/5
// https://adventofcode.com/2019/day/9

// For this I decided to try and refactor the IntCode computer
// from day 9 into separate classes using a command pattern like setup.

// ** Part 1: 
RobotIO robot = new RobotIO();
IntCodeComputer computer = new IntCodeComputer(myInput, robot);
computer.Run();

Console.WriteLine("Part 1 - Painted panel count:" + robot.GetPaintedPanelCount());

// ** Part 2:
robot.Reset();
robot.SetValue(1);
computer.Run();
Console.WriteLine("Part 2 - Retrieving painted grid:");
robot.GetPaintedPanel().Print();