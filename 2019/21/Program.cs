// Solution for https://adventofcode.com/2019/day/21 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of program lines that represent opcode and parameters

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings("");

// This puzzle reuses the existing IntCode computer from day 19,
// which reuses the IntCode computer from day 17,
// which reuses the IntCode computer from day 15,
// which reuses the IntCode computer from day 13,
// which reuses the IntCode computer from day 11,
// which reuses the IntCode computer from day 9,
// which reuses the IntCode computer from day 5,
// which reuses the existing IntCode computer from day 2 :)

// Previous IntCode computers:
// https://adventofcode.com/2019/day/2
// https://adventofcode.com/2019/day/5
// https://adventofcode.com/2019/day/9
// https://adventofcode.com/2019/day/11
// https://adventofcode.com/2019/day/13
// https://adventofcode.com/2019/day/15
// https://adventofcode.com/2019/day/17

// ** Part 1:

// Given a floor e.g. XXXX_X_XXXXX etc...
// we basically want to jump if any of the 3 tiles in front of us is a hole,
// but we can safely land on the 4th..., because when we jump we always skip 3 steps...
// so the landing spot needs to be ground and we want to time it in such a way that
// we can hop from tile to tile...

string part1Instructions =
    "NOT A T"   + Environment.NewLine +    //Store A.isHole in T
    "NOT B J"   + Environment.NewLine +    //Store B.isHole in J
    "OR J T"    + Environment.NewLine +    //Store A.isHole OR B.isHole in T so J is free again
    "NOT C J"   + Environment.NewLine +    //Store C.isHole in J
    "OR T J"    + Environment.NewLine +    //Combine what we had in T with what we have in J -> (A.isHole | B.isHole | C.isHole)
    "AND D J"   + Environment.NewLine +    //Combine with AND D.isGround
    "WALK"      + Environment.NewLine;     //Start walking...

SpringDroid io = new SpringDroid();

io.SetProgram(part1Instructions);

IntCodeComputer robotController = new IntCodeComputer(myInput, io);
robotController.Run();

Console.WriteLine("Part 1:" + io.GetOutput());

// ** Part 2:

// Ok we want to jump under the same condition as in part 1 but that might also put us in this situation...
// If we jump ON 1, we'll land ON 2 and we are immediately forced to jump again, and we'll land on 3 (and die)
//
// 1   2   3
// XXX_X_XX_XX_XXX
//
// But if we postpone our jump we have a chance of surviving...
//
//   1   2   3
// XXX_X_XX_XX_XXX
//    123456789
//
// In other words, we should stick to the rules we already had, 
// BUT not if this gets us into a situation where the next spot (5 -> register E)
// would force us to jump in an empty spot (8 -> register H)
// if the next space is empty which forces us to jump in an empty space...
//
// In other words JUMP if we already wanted to JUMP and NOT (NOT E AND NOT H)
// Simplifying this using de morgan's laws =
// NOT NOT (NOT NOT E OR NOT NOT H) =
// (E OR H)

// In other words as long as either E or H is a ground patch, we're fine...

string part2Instructions =
    "NOT A T"   + Environment.NewLine +     //Store A.isHole in T
    "NOT B J"   + Environment.NewLine +     //Store B.isHole in J
    "OR J T"    + Environment.NewLine +     //Store A.isHole or B.isHole in T so J is free again
    "NOT C J"   + Environment.NewLine +     //Store C.isHole in J
    "OR T J"    + Environment.NewLine +     //Combine what we had in T with what we have in J (A.isHole | B.isHole | C.isHole)
    "AND D J"   + Environment.NewLine +     //Combine with AND D.isGround

    "NOT E T"   + Environment.NewLine +		// Clear out register T by setting it to NOT E ...
    "AND E T"   + Environment.NewLine +		// AND E (which is false)

    "OR E T"    + Environment.NewLine +		// Then OR it with E
    "OR H T"    + Environment.NewLine +		// And OR it with H
    "AND T J"   + Environment.NewLine +		// And AND them with what we already had
    "RUN"       + Environment.NewLine;      // RUN!

Console.WriteLine();
io.SetProgram(part2Instructions);
robotController.Run();

Console.WriteLine("Part 2:" + io.GetOutput());
