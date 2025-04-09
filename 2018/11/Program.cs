//Solution for https://adventofcode.com/2018/day/11 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a grid serial number

int gridSerialNumber = int.Parse(args[0]);

// Made two implementations,  BruteForce and Optimized

BruteForce bruteForce = new BruteForce(gridSerialNumber);
//bruteForce.Run();

Optimized optimized = new Optimized(gridSerialNumber);
optimized.Run();