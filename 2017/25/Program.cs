//Solution for https://adventofcode.com/2017/day/25 (Ctrl+Click in VS to follow link)

// State are made of matches, 0 and 1, and these matches tell us the next value and the next state
using State = (char state, System.Collections.Generic.List<(int value, int direction, char nextState)> matches);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

string[] myInput = File.ReadAllLines(args[0]);

// Now process all lines..., little bit convoluted parsing mechanism but ala...

char currentStateIdentifier = 'A';
Dictionary<char, State> states = new();
long stepsToPerform = 0;

State? currentState = null;

const string beginInPattern                 = "Begin in state ";
const string checksumPattern                = "Perform a diagnostic checksum after ";
const string inStatePattern                 = "In state ";
const string ifCurrentValuePattern          = "If the current value is ";
const string writeValuePattern              = "- Write the value ";
const string movePattern                    = "- Move one slot to the ";
const string continueWithPattern            = "- Continue with state ";

foreach (string line in myInput)
{
    string trimmedLine = line.Trim();

    if (trimmedLine.Length == 0)
    {
        if (currentState != null) {
            states[currentState.Value.state] = currentState.Value;
        }

        continue;
    }

    if (trimmedLine.StartsWith(beginInPattern))
    {
        currentStateIdentifier = trimmedLine[beginInPattern.Length];
    }
    else if (trimmedLine.StartsWith(checksumPattern))
    {
        trimmedLine = trimmedLine.Replace(checksumPattern, "");
        trimmedLine = trimmedLine.Replace(" steps.","");
        stepsToPerform = long.Parse (trimmedLine);
    }
    else if (trimmedLine.StartsWith(inStatePattern)) {
        currentState = new State(trimmedLine[inStatePattern.Length], new());
    }
    else if (trimmedLine.StartsWith(ifCurrentValuePattern))
    {
        // Create a new slot, these lines always progress in the order 0, 1 so we don't have to do anything else
        currentState.Value.matches.Add(new());
    }
    else if (trimmedLine.StartsWith (writeValuePattern))
    {
        var currentSlot = currentState.Value.matches.Last();
        currentSlot.value = trimmedLine[writeValuePattern.Length] - '0';
        currentState.Value.matches[currentState.Value.matches.Count-1] = currentSlot;
    }
    else if (trimmedLine.StartsWith(movePattern))
    {
        var currentSlot = currentState.Value.matches.Last();
        currentSlot.direction = trimmedLine[movePattern.Length] == 'r' ? 1 : -1;
        currentState.Value.matches[currentState.Value.matches.Count - 1] = currentSlot;
    }
    else if (trimmedLine.StartsWith(continueWithPattern))
    {
        var currentSlot = currentState.Value.matches.Last();
        currentSlot.nextState = trimmedLine[continueWithPattern.Length];
        currentState.Value.matches[currentState.Value.matches.Count - 1] = currentSlot;
    }
    else
    {
        throw new Exception("Not supposed to happen:" + trimmedLine);
    }
}

// Make sure we store the last state as well:
states[currentState.Value.state] = currentState.Value;

// ** Part 1: Execute all the steps and check how many slots are filled.

// If value is 1, the slot index will be in this hashset, otherwise the value is 0
HashSet<long> slots = new HashSet<long>();
long cursor = 0;

for (long i = 0; i < stepsToPerform; i++)
{
    State state = states[currentStateIdentifier];

    int slotToUse = slots.Contains(cursor) ? 1 : 0;

    var processor = state.matches[slotToUse];
    if (processor.value == 1) slots.Add(cursor);
    else slots.Remove(cursor);

    cursor += processor.direction;
    currentStateIdentifier = processor.nextState;
}

Console.WriteLine("Part 1:" + slots.Count);