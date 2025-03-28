// Solution for https://adventofcode.com/2019/day/9 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of program lines that represent opcode and parameters

using System.Diagnostics;

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings("");

// Due to the big number requirement we interpret everything as longs
long[] program = myInput.Split(",").Select(long.Parse).ToArray();

// ** This puzzle reuses the existing IntCode computer from day 5,
// which reuses the existing IntCode computer from day 2 :)
// In other words, good opportunity to take those existing
// IntCode computer setups and document them some more.
//
// Previous IntCode computers:
// https://adventofcode.com/2019/day/2
// https://adventofcode.com/2019/day/5

// Additional required (global, sorry ;)) variable
long relativeBase = 0;

// For this puzzle, we don't use the actual int program array, since the puzzle mentions:
// "The computer's available memory should be much larger than the initial program.
// Memory beyond the initial program starts with the value 0 and can be read or written
// like any other memory. (It is invalid to try to access memory at a negative address, though.)"
//
// The easiest way to simulate this is to use a Dictionary of long to longs
// (from instruction pointer, to memory contents)

Dictionary<long, long> memory = new Dictionary<long, long>();

// Copy the original program into the memory
for (uint i = 0; i < program.Length; i++)
{
	memory[i] = program[i];
}

// Full disclosure, this one had me stumped for hours.
// In the end I had to look up several hints on Reddit, in hindsight I'm just @$#@(^$ stupid.
//
// So let me start by documenting my main mistakes, maybe it will save you some time:

// Requirement 1:
// "Parameters in mode 2, relative mode, behave very similarly to parameters in position mode:
// the parameter is interpreted as a position. Like position mode, parameters in relative mode
// can be read from or written to."
//
// My mistakes:
// - I only implemented this for reading, not for writing, e.g I was still using
//		pProgram[i+3] instead of pProgram[GetIndex(i+3, param3Mode)]
// - I was using the same GetValue method instead of GetIndex, the problem being that:
//		- GetValue already resolves the calculated index in memory
//		- GetIndex should not do that, since we already do pProgram[GetIndex()]
//		In other words, I was dereferencing the resulting value once too many times while writing

// Requirement 2:
// " The relative base is modified with the relative base offset instruction:
// Opcode 9 adjusts the relative base by the value of its only parameter.
// The relative base increases (or decreases, if the value is negative) by the value of the parameter."
//
// My mistake:
// - I forgot to include a parameter mode for the relative Base operation:
//		e.g. do
//			relativeBase += GetValue(pProgram, i + 1, param1Mode);
//		instead of	
//			relativeBase += pProgram[i + 1];

// Other than that everything was ok, but the diagnostic program output was completely useless unfortunately
// (At least for me :)).

long RunIntCodeComputer (Dictionary<long, long> pProgram)
{
	//Clone the program in case we want to run it again
    pProgram = new Dictionary<long, long>(pProgram);
    Stopwatch stopwatch = new Stopwatch();

    long i = 0;
	relativeBase = 0;

    while (true)
    {
        long valueRead = pProgram[i];                //the actual value read from the program
        long opCode = valueRead % 100;               //filtering out opcodes...

		// Day 5 Addition - parameter modes:
		//
		// Each parameter of an instruction is handled based on its parameter mode.
		// - parameter mode 0, (position mode) causes the parameter to be interpreted
		//   as a position e.g. if the parameter is 50, its value is the value stored
		//   at address 50 in memory. 
		// - parameters mode 1, (immediate mode) causes parameters to be interpreted as a value
		//   e.g. if the parameter is 50, its value is simply 50. 

		// Day 9 Addition: 
		// - parameters mode 2 (relative mode, see GetIndex)

		// There are 3 different parameters for which we need to determine their modes:

		int param1Mode = (int)((valueRead / 100) % 10);		//filtering out mode for param 1...
		int param2Mode = (int)((valueRead / 1000) % 10);	//filtering out mode for param 2...
		int param3Mode = (int)((valueRead / 10000) % 10);   //filtering out mode for param 3...

        // Day 2:
        // Opcode 1 adds together numbers read from two positions and stores the
        // result in a third position.The three integers immediately after the
        // opcode tell you these three positions:
        // - the first two indicate the positions from which you should read the input values,
        // and the third indicates the position at which the output should be stored.

        // Once you're done processing opcode 1,
        // move to the next one by stepping forward 4 positions.

        if (opCode == 1)
		{
			//Notice GetIndex vs getValue... grr... 3 hours down the drain.. grr...
			pProgram[GetIndex(pProgram, i + 3, param3Mode)] =
				GetValue(pProgram, i + 1, param1Mode) + 
				GetValue(pProgram, i + 2, param2Mode);

			i += 4;
		}

		// Day 2:
        // Opcode 2 works exactly like opcode 1, except it multiplies the two inputs instead
		// of adding them. Again, the three integers after the opcode indicate where the
		// inputs and outputs are, not their values.

		// Once you're done processing opcode 2,
		// move to the next one by stepping forward 4 positions.

		else if (opCode == 2)
		{
			pProgram[GetIndex(pProgram, i + 3, param3Mode)] =
				GetValue(pProgram, i + 1, param1Mode) * 
				GetValue(pProgram, i + 2, param2Mode);

            i += 4;
		}

        // Day 3:
        // Opcode 3 takes a single integer as input and saves it to the position given by its
        // only parameter. For example, the instruction 3,50 would take an input value and
        // store it at address 50.
		
		else if (opCode == 3)
		{
			long singleIntegerAsInput = long.Parse(Console.ReadLine());
            stopwatch.Start();

            pProgram[GetIndex(pProgram, i + 1, param1Mode)] = singleIntegerAsInput;

			i += 2;
		}

		// Day 4:
		// Opcode 4 outputs the value of its only parameter.
		// For example, the instruction 4,50 would output the value at address 50.

		else if (opCode == 4)
		{
			Console.WriteLine(GetValue(pProgram, i + 1, param1Mode));
			i += 2;
		}

		// Opcode 5 is jump-if-true:
        // if the first parameter is non - zero,
        // it sets the instruction pointer to the value from the second parameter.
        // Otherwise, it does nothing.

		else if (opCode == 5)
		{
			long firstParam = GetValue(pProgram, i + 1, param1Mode);

            if (firstParam != 0)
            {
                i = GetValue(pProgram, i + 2, param2Mode);
            }
            else
            {
                i += 3;
            }
        }

		// Opcode 6 is jump-if-false:
		// if the first parameter is zero,
		// it sets the instruction pointer to the value from the second parameter.
		// Otherwise, it does nothing.

		else if (opCode == 6)
        {
            long firstParam = GetValue(pProgram, i + 1, param1Mode);

            if (firstParam == 0)
            {
                i = GetValue(pProgram, i + 2, param2Mode);
            }
            else
            {
                i += 3;
            }
        }

		// Opcode 7 is less than:
		// if the first parameter is less than the second parameter,
		// it stores 1 in the position given by the third parameter.
		// Otherwise, it stores 0.

		else if (opCode == 7)
        {
            long firstParam = GetValue(pProgram, i + 1, param1Mode);
            long secondParam = GetValue(pProgram, i + 2, param2Mode);

            pProgram[GetIndex(pProgram, i + 3, param3Mode)] = (firstParam < secondParam) ? 1 : 0;

            i += 4;
        }

		// Opcode 8 is equals:
		// if the first parameter is equal to the second parameter,
		// it stores 1 in the position given by the third parameter.
		// Otherwise, it stores 0.

		else if (opCode == 8)
        {
            long firstParam = GetValue(pProgram, i + 1, param1Mode);
            long secondParam = GetValue(pProgram, i + 2, param2Mode);

            pProgram[GetIndex(pProgram, i + 3, param3Mode)] = (firstParam == secondParam) ? 1 : 0;

			i += 4;
		}

		// Opcode 9 adjusts the relative base by the value of its only parameter.
		// The relative base increases (or decreases, if the value is negative)
		// by the value of the parameter.

		else if (opCode == 9)
		{
			relativeBase += GetValue(pProgram, i + 1, param1Mode);
			//relativeBase += pProgram[i + 1]; -> wrong! Needs to use parameter modes as well!
            i += 2;
		}

		// opCode 99 halts the program

		else if (opCode == 99)
        {
            break;
        }

        else
        {
            Console.WriteLine("Invalid opcode:" + opCode);
        }

    }

    Console.WriteLine("Computed in " + stopwatch.ElapsedMilliseconds + " milliseconds.");

    return pProgram[0];
}

//Day 5 Addition:
// Each parameter of an instruction is handled based on its parameter mode.
// - parameter mode 0, (position mode) causes the parameter to be interpreted
//   as a position e.g. if the parameter is 50, its value is the value stored
//   at address 50 in memory. 
// - parameters mode 1, (immediate mode) causes parameters to be interpreted as a value
//   e.g. if the parameter is 50, its value is simply 50. 

// Day 9 Addition:
// - parameter mode 2, (relative mode) causes parameters to be interpreted as a position
// relative to the relative base.

// GetValue looks up a value in memory using the GetIndex method
long GetValue(Dictionary<long, long> pProgram, long pIndex, int pParameterMode)
{
	return pProgram.GetValueOrDefault(
		GetIndex(pProgram, pIndex, pParameterMode)
	);
}

long GetIndex (Dictionary<long, long> pProgram, long pIndex, int pParameterMode)
{
    // Parameter mode 0 - Position mode, value is interpreted as a memory address
    if (pParameterMode == 0) return pProgram.GetValueOrDefault(pIndex);
    // Parameter mode 1 - Immediate mode, value is interpreted as a value
    else if (pParameterMode == 1) return pIndex;
    // Parameter mode 2 - Relative mode, value is interpreted as memory address relative to the relativeBase
    else if (pParameterMode == 2) return relativeBase + pProgram.GetValueOrDefault(pIndex);

	throw new InvalidDataException("Parameter mode " + pParameterMode + " not supported.");
}

Console.WriteLine ("Part 1 Running program (provide 1 as input please !)");
RunIntCodeComputer(memory);

Console.WriteLine("Part 2 Running program (provide 2 as input please !)");
RunIntCodeComputer(memory);
