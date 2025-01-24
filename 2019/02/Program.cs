// Solution for https://adventofcode.com/2019/day/2 (Ctrl+Click in VS to follow link)

// Your input: a bunch of program that represent opcode and parameters

using System.Security.Principal;

string myInput = "1,0,0,3,1,1,2,3,1,3,4,3,1,5,0,3,2,6,1,19,1,5,19,23,2,6,23,27,1,27,5,31,2,9,31,35,1,5,35,39,2,6,39,43,2,6,43,47,1,5,47,51,2,9,51,55,1,5,55,59,1,10,59,63,1,63,6,67,1,9,67,71,1,71,6,75,1,75,13,79,2,79,13,83,2,9,83,87,1,87,5,91,1,9,91,95,2,10,95,99,1,5,99,103,1,103,9,107,1,13,107,111,2,111,10,115,1,115,5,119,2,13,119,123,1,9,123,127,1,5,127,131,2,131,6,135,1,135,5,139,1,139,6,143,1,143,6,147,1,2,147,151,1,151,5,0,99,2,14,0,0\r\n";
myInput = myInput.ReplaceLineEndings();

int[] program = myInput.Split(",").Select(int.Parse).ToArray();

// Part 1 - "Run" the program and return the value at position 0
//
// How do you run the progam?
// if program[i] == 1 => program[program[i+3]] = program[program[i+1]] + program[program[i+2]]
// if program[i] == 2 => program[program[i+3]] = program[program[i+1]] * program[program[i+2]]
// if program[i] == 99 => halt the program
// after i => i+4

// Noun and Verb terminology used in part 2
long RunProgram(int[] pProgram, int pNoun, int pVerb)
{
	pProgram = (int[])pProgram.Clone();

	int i = 0;
	pProgram[1] = pNoun;
	pProgram[2] = pVerb;

	while (pProgram[i] != 99)
	{
		if (pProgram[i] == 1)
		{
			pProgram[pProgram[i + 3]] = pProgram[pProgram[i + 1]] + pProgram[pProgram[i + 2]];
		}
		else if (pProgram[i] == 2)
		{
			pProgram[pProgram[i + 3]] = pProgram[pProgram[i + 1]] * pProgram[pProgram[i + 2]];
		}

		i += 4;
	}
	return pProgram[0];
}

Console.WriteLine("Part 1 - Result of program (12,2): " + RunProgram (program, 12, 2)) ;

// Part 2 - Find the verb and noun that make the "valueToAchieve"
// Alternatively we could just run this in a double loop

Console.WriteLine("Part 2:");
Console.WriteLine("Result of program (0,0): " + RunProgram (program, 0, 0));

long valueToAchieve = 19690720;
Console.WriteLine("Value to achieve: " + valueToAchieve);

long baseValue = valueToAchieve - RunProgram(program, 0, 0);
long diffA = RunProgram(program, 1, 0) - RunProgram(program, 0, 0);
long diffB = RunProgram(program, 0, 1) - RunProgram(program, 0, 0);

Console.WriteLine("Calculate the base value, verb stepsize, noun stepsize");
Console.WriteLine(baseValue + " / " + diffA + " / " + diffB);

int verb = (int) (baseValue / diffA);
int noun = (int) (baseValue - ((baseValue / diffA) * diffA));

Console.WriteLine("Verb:" + verb);
Console.WriteLine("Noun:" + noun);

Console.WriteLine($"Test RunProgram(program, {verb}, {noun}): " + RunProgram(program, verb, noun));
Console.WriteLine("100 * verb + noun: " + (100 * verb + noun));

Console.ReadKey();