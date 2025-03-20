// Solution for https://adventofcode.com/2015/day/7 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of expressions to evaluate that leads to all
// listed variables having a specific (ushort) value

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Part 1 -

// In little Bobby's kit's instructions booklet (provided as your puzzle input),
// what signal is ultimately provided to wire a?

// This code implements the naive version based on brute forcing through all expressions
// to weed out the ones that can be evaluated, repeating this over and over
// (including the parsing of each expression !) until there are no more expressions left

// First, break the input into separate lines to process
List<string> expressions = myInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();

// Reset a table of values to 16-bit values
Dictionary<string, ushort> valueTable = new();
Console.Write("Part 1:" + GetVariableValue("a"));
Console.WriteLine();

// ** Part 2 - 

// Same but now with a different initial value in the value table for variable b

valueTable.Clear();
valueTable["b"] = 956;
Console.Write("Part 2:" + GetVariableValue("a"));

// ** All methods used to derive the variable value

ushort GetVariableValue(string pVariable)
{
    //Basic idea: brute force run through of all expressions that can be evaluated
    //If it CAN be evaluated, we evaluate it and remove it from the list (storing the result in the value table)
    //If it CAN'T be evaluated, we skip it and run it again on the next go through
    //Very slow, but simple.

	List<string> expressionsClone = new List<string>(expressions);

    //while there are still expressions left in the list
    while (expressionsClone.Count > 0)
    {
        //run through each expression and process it
        for (int i = expressionsClone.Count - 1; i >= 0; i--)
        {
            try //to process the expression, if we CAN, the expression will be removed, otherwise we'll get the error and move on
            {
                ProcessExpression(expressionsClone[i]);
                expressionsClone.RemoveAt(i);
            }
            catch {
            }
        }
        Console.WriteLine(expressionsClone.Count + " expressions left to evaluate...");
    }

    return valueTable[pVariable];
}

//af AND ah -> ai
//NOT lk -> ll
//1 -> br
void ProcessExpression(string pExpression)
{
    string[] leftVSRight = pExpression.Split(new string[] { "->" }, StringSplitOptions.None);

    string equation = leftVSRight[0].Trim();
    string variable = leftVSRight[1].Trim();

    string[] equationParts = equation.Split(" ", StringSplitOptions.RemoveEmptyEntries);
    
    if      (equationParts.Length == 1) ProcessSimpleValueExpression (equationParts, variable);
    else if (equationParts.Length == 2) ProcessUnaryOpExpression(equationParts, variable);
    else if (equationParts.Length == 3) ProcessBinaryOpExpression(equationParts, variable);
    else throw new Exception("Format not supported");
}

void ProcessSimpleValueExpression(string[] pEquationParts, string pVariable)
{
    if (!valueTable.ContainsKey(pVariable))
    {
        valueTable[pVariable] = GetValue(pEquationParts[0]);
    }
}

void ProcessUnaryOpExpression(string[] pEquationParts, string pVariable)
{
    if (pEquationParts[0] == "NOT") valueTable[pVariable] = (ushort)(~GetValue(pEquationParts[1]));
}

void ProcessBinaryOpExpression(string[] pEquationParts, string pVariable)
{
    if      (pEquationParts[1] == "AND")      valueTable[pVariable] = (ushort)(GetValue(pEquationParts[0]) & GetValue(pEquationParts[2]));
    else if (pEquationParts[1] == "OR")       valueTable[pVariable] = (ushort)(GetValue(pEquationParts[0]) | GetValue(pEquationParts[2]));
    else if (pEquationParts[1] == "RSHIFT")   valueTable[pVariable] = (ushort)(GetValue(pEquationParts[0]) >> GetValue(pEquationParts[2]));
    else if (pEquationParts[1] == "LSHIFT")   valueTable[pVariable] = (ushort)(GetValue(pEquationParts[0]) << GetValue(pEquationParts[2]));
    else throw new Exception("Format not supported");
}

//Expression can be a variable name or a value. If it is a variable name, it should be in the value table, otherwise we simply parse it
ushort GetValue(string pExpression)
{
    if (valueTable.ContainsKey(pExpression)) return valueTable[pExpression];
    return ushort.Parse(pExpression);
}

