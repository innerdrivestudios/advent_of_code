// Solution for https://adventofcode.com/2019/day/13 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of program lines that represent opcode and parameters

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings("");

// This puzzle reuses the existing IntCode computer from day 11,
// which reuses the IntCode computer from day 9,
// which reuses the IntCode computer from day 5,
// which reuses the existing IntCode computer from day 2 :)

// Previous IntCode computers:
// https://adventofcode.com/2019/day/2
// https://adventofcode.com/2019/day/5
// https://adventofcode.com/2019/day/9
// https://adventofcode.com/2019/day/11

// ** Part 1: 
ArcadeMachineV1_0 arcadeMachineV1 = new ArcadeMachineV1_0();
IntCodeComputer computerV1 = new IntCodeComputer(myInput, arcadeMachineV1);
computerV1.Run();

Console.WriteLine("Part 1 - Block count:" + arcadeMachineV1.GetTileCount(TileType.Block));
Console.WriteLine("Screensize:" + arcadeMachineV1.GetBounds());

// ** Part 2:
var bounds = arcadeMachineV1.GetBounds();
ArcadeMachineV2_0 arcadeMachineV2 = new ArcadeMachineV2_0(bounds.Item1, bounds.Item2, true);
IntCodeComputer computerV2 = new IntCodeComputer(myInput, arcadeMachineV2);
computerV2.OverwriteMemory(0, 2);
computerV2.Run();

Console.SetCursorPosition(0, bounds.Item2.Y+5);
Console.ReadKey();




