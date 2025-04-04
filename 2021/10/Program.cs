// Solution for https://adventofcode.com/2021/day/10 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: nested character strings using (){}[]<>

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

string[] chunks = myInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

// Step 1: Process the input...

// Let's write a helper function first that processes a given chunk up until the string is done.
// This kind of challenge is typically approached using a stack...
// ({[< pushes items onto the stack, >]}) pops items from the stack assuming the elements
// match symmetrically, e.g. ) pops (, ] pops [, etc.
//
// Processing the string this way we have got three options:
// - we parsed the whole string and the stack is empty -> string is valid and complete
// - we parsed the whole string and the stack is not empty -> string is valid but not complete
// - we encounter an invalid match, stack is not empty, last element indicates what we expect,
//   given element determines the score

ParseResult ProcessString (string pInput)
{
	Stack<char> stack = new Stack<char>();

	for (int i = 0; i < pInput.Length; i++)
	{
		char c = pInput[i];

		//Opening chars are pushed on the stack...
		if (ParseUtil.IsOpeningChar(c))
		{
			stack.Push(c);
		}
		//Closing chars are either...
		else if (ParseUtil.IsClosingChar(c))
		{
			//Popped of the stack if the next char is a closing brace of the current char...
			if (ParseUtil.Match(stack.Peek(), c))
			{
				stack.Pop();
			}
			//Or an invalid parse result...
			else
			{
				return new ParseResult(ParseResult.ParseStatus.Invalid, pInput, stack, i);
			}
		}
	}

	//If stack is empty, we have two options ...
	return stack.Count == 0 ?
		new ParseResult(ParseResult.ParseStatus.Complete, pInput) :
		new ParseResult(ParseResult.ParseStatus.Incomplete, pInput, stack);
}

List<ParseResult> invalidResults = new List<ParseResult>();
List<ParseResult> completeResults = new List<ParseResult>();
List<ParseResult> incompleteResults = new List<ParseResult>();

foreach (string chunk in chunks) 
{
	ParseResult parseResult = ProcessString(chunk);

	switch (parseResult.status)
	{
		case ParseResult.ParseStatus.Incomplete:	incompleteResults.Add(parseResult);		break;
		case ParseResult.ParseStatus.Complete:      completeResults.Add(parseResult);       break;
		case ParseResult.ParseStatus.Invalid:		invalidResults.Add(parseResult);		break;
	}
}

// ** Part 1: Calculate the total syntax error score for all errors
Console.WriteLine("Part 1: " + invalidResults.Sum (x => x.invalidCharScore));

// ** Part 2: None of the remaining strings is complete or invalid, so only incomplete strings left.
// Score the required auto completion and choose the middle score

long[] autocompletionScores = incompleteResults.Select(x => x.GetAutoCompleteScore()).OrderBy(x=>x).ToArray();
//0,1,2,3,4 -> 5. 5/2 = 2.5 in ints 2, so...
int middleElement = autocompletionScores.Length / 2;
Console.WriteLine("Part 2: " + autocompletionScores[middleElement]);
