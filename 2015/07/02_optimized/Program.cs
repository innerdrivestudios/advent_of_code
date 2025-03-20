// Solution for https://adventofcode.com/2015/day/7 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of expressions to evaluate that leads to all listed
// variables having a specific (ushort) value

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Part 1 -

// In little Bobby's kit's instructions booklet (provided as your puzzle input),
// what signal is ultimately provided to wire a?

// This code implements an optimized version based on recursion to calculate all
// required variables in exactly the correct order, parsing each expression only once

// First, break the input into a table that maps a variable to the expression used to generate its value
Dictionary<string, string> equations = myInput
          .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)//Split all lines by \r\n
          .Select(line => line.Split(" -> ", StringSplitOptions.None))      //Then select a string[] from those lines by splitting on " -> "
          .ToDictionary(parts => parts[1], parts => parts[0]);              //And create a dictionary from variable to the expression that calculates or IS its value

//Value table of 16-bit end values
Dictionary<string, ushort> valueTable = new();
Console.WriteLine("Part 1:" + GetValue("a"));

// ** Part 2:
valueTable.Clear();
valueTable["b"] = 956;
Console.WriteLine("Part 2:" + GetValue("a"));

// ** All methods used to get the value...

ushort GetValue(string pValueOrVariable)
{
    //if we are lucky the variable is just a simple ushort and we can return it immediately
    if (ushort.TryParse(pValueOrVariable, out ushort value))
    {
        return value;
    }

    //If not, we check if a value for the given variable name is already in the value table:
    if (valueTable.ContainsKey(pValueOrVariable)) return valueTable[pValueOrVariable];

    //otherwise calculate it based on the given equation for this variable stored in the equations table
    //STORE the result of that equation (to avoid endless recursion) and return it
    ushort result = ProcessExpression(equations[pValueOrVariable]);
    valueTable[pValueOrVariable] = result;
    return result;
}

ushort ProcessExpression(string pEquations)
{
    string[] equationParts = pEquations.Split(" ", StringSplitOptions.RemoveEmptyEntries);
    
    if      (equationParts.Length == 1) return GetValue(equationParts[0]);
    else if (equationParts.Length == 2) return ProcessUnaryOpExpression(equationParts);
    else if (equationParts.Length == 3) return ProcessBinaryOpExpression(equationParts);
    else throw new Exception("Format not supported");
}

ushort ProcessUnaryOpExpression(string[] pEquationParts)
{
    if (pEquationParts[0] == "NOT") return (ushort)(~GetValue(pEquationParts[1]));
    else throw new Exception("Unary operator not supported");
}

ushort ProcessBinaryOpExpression(string[] pEquationParts)
{
    if      (pEquationParts[1] == "AND")      return (ushort)(GetValue(pEquationParts[0]) & GetValue(pEquationParts[2]));
    else if (pEquationParts[1] == "OR")       return (ushort)(GetValue(pEquationParts[0]) | GetValue(pEquationParts[2]));
    else if (pEquationParts[1] == "RSHIFT")   return (ushort)(GetValue(pEquationParts[0]) >> GetValue(pEquationParts[2]));
    else if (pEquationParts[1] == "LSHIFT")   return (ushort)(GetValue(pEquationParts[0]) << GetValue(pEquationParts[2]));
    else throw new Exception("Binary operator not supported");
}


