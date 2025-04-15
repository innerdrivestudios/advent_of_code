//Solution for https://adventofcode.com/2015/day/12 (Ctrl+Click in VS to follow link)

using System.Text.Json.Nodes;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a big-ol' JSON file
string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

//Your task: count nodes that match certain requirements

Console.WriteLine(
    "Part 1: Sum of all numeric nodes -> " 
    + SumAllNumericNodes(myInput)
);

Console.WriteLine(
    "Part 2: Sum of all numeric nodes excluding red -> "  
    + SumAllNumericNodes(myInput, ["red"])
);

int SumAllNumericNodes(string pInput, HashSet<string> pExclusions = null)
{
    int total = 0;

	//Get the whole parsed json tree
	JsonNode root = JsonObject.Parse(pInput);
	//Find all the numeric nodes and sum them
	Traverse(root, ref total, pExclusions);     

    return total;
}

void Traverse(JsonNode pNode, ref int pTotal, HashSet<string> pExclusions)
{
    if (pNode is JsonObject jsonObject) TraverseJsonObject(jsonObject, ref pTotal, pExclusions);
    else if (pNode is JsonArray jsonArray) TraverseJsonArray (jsonArray, ref pTotal, pExclusions);
    else if (pNode is JsonValue jsonValue) TraverseJsonValue (jsonValue, ref pTotal);
    else
    {
        throw new Exception ("Unhandled node type found:" + pNode.GetType());
    }
}

//Unfortunately no overloading in top level statement ;)
void TraverseJsonObject (JsonObject pNode, ref int pTotal, HashSet<string> pExclusions)
{
    //If any of this nodes children has an excluded property ignore it completely
    if (pExclusions != null)
    {
        foreach (var node in pNode)
        {
            if (pExclusions.Contains(node.Value.ToString())) return;
        }
    }

    foreach (var node in pNode)
    {
        Traverse(node.Value, ref pTotal, pExclusions);
    }
}    

void TraverseJsonArray (JsonArray pNode, ref int pTotal, HashSet<string> pExclusions)
{
    foreach (var node in pNode)
    {
        Traverse(node, ref pTotal, pExclusions);
    }
}

void TraverseJsonValue (JsonValue pNode, ref int pTotal)
{
    if (pNode.GetValueKind() == System.Text.Json.JsonValueKind.Number)
    {
        pNode.TryGetValue(out int value);
        pTotal += value;
    }
}


