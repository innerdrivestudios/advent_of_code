// Solution for https://adventofcode.com/2020/day/18 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of simple mathematical expressions

string[] myInput = File.ReadAllLines(args[0]);

// ** Part 1: Evaluate all these expressions with a different set of mathematical rules than
// the usual ones. In this case: evaluate all expressions from left to right
// (+ and * have the precedence and left to right associativity)

// In order to calculate this, we'll write a very simple expression parser,
// under the assumption that every provided expression is correct.

// pInput		-> the input string
// pEvaluator	-> the parser for the main expression string
long Evaluate (string pInput, Func<StringReader, long> pEvaluator)
{
	//Remove all whitespace
	pInput = Regex.Replace (pInput, @"\s+", "");

	//Create a single stringreader and use it to evaluate the actual expression
	StringReader stringReader = new StringReader(pInput);
	return pEvaluator (stringReader);
}

long LeftToRightParser (StringReader pStringReader)
{
	//The basic grammar is operand operator operand operator operand
	//And each operand can be another expression

	//Minimum requirement is ONE operand
	long result = ParseOperand (pStringReader, LeftToRightParser);

	//And optionally an "infinite" amount of OPERATOR OPERAND follow ups
	while (true)
	{
		char op = (char)pStringReader.Peek();

		if (op == '+' || op == '*')
		{
			op = (char)pStringReader.Read();
			long value = ParseOperand(pStringReader, LeftToRightParser);

			// For the left to right parser,
			// we can simply evaluate the result while parsing

			if (op == '+')
			{
				result += value;
			}
			else if (op == '*')
			{
				result *= value;
			}
			else throw new Exception("Invalid operator");
		}
		else // no follow up...
		{
			break;
		}
	}

	return result;
}

//Operand is either a long or another expression
long ParseOperand (StringReader pStringReader, Func<StringReader, long> pEvaluator)
{
	char c = (char)pStringReader.Peek();

	long number = 0;

	if (char.IsDigit(c))
	{
		number = ParseNumber(pStringReader);
	}
	else
	{
		number = ParseExpressionInParentheses(pStringReader, pEvaluator);
	}

	return number;
}

long ParseNumber (StringReader pStringReader)
{
	string number = "";
	while (char.IsDigit((char)pStringReader.Peek())) number += (char)pStringReader.Read();
	return long.Parse(number);
}

long ParseExpressionInParentheses (StringReader pStringReader, Func<StringReader, long> pEvaluator)
{
	pStringReader.Read ();  //(
	long value = pEvaluator (pStringReader);
	pStringReader.Read();	//)
	return value;
}

Console.WriteLine("Test input:");
Console.WriteLine(Evaluate("1 + 2 * 3 + 4 * 5 + 6", LeftToRightParser));
Console.WriteLine(Evaluate("1 + (2 * 3) + (4 * (5 + 6))", LeftToRightParser));
Console.WriteLine(Evaluate("2 * 3 + (4 * 5)", LeftToRightParser));
Console.WriteLine(Evaluate("5 + (8 * 3 + 9 + 3 * 4 * 3)", LeftToRightParser));
Console.WriteLine(Evaluate("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", LeftToRightParser));
Console.WriteLine(Evaluate("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", LeftToRightParser));
Console.WriteLine();

Console.WriteLine("Part 1:" + myInput.Select (x => Evaluate(x, LeftToRightParser)).Sum());

/******************************/

// Almost the same idea as part 1, but now we execute the + operand immediately 
// and the * when everything else is done...

long FlippedPrecedenceParser(StringReader pStringReader)
{
	Stack<long> longs = new ();
	longs.Push (ParseOperand(pStringReader, FlippedPrecedenceParser));

	while (true)
	{
		char op = (char)pStringReader.Peek();

		if (op == '+' || op == '*')
		{
			pStringReader.Read();
			longs.Push (ParseOperand(pStringReader, FlippedPrecedenceParser));

			if (op == '+')
			{
				longs.Push(longs.Pop() + longs.Pop());
			}

		}
		else
		{
			break;
		}
	}

	while (longs.Count > 1)
	{
		longs.Push(longs.Pop() * longs.Pop());
	}

	return longs.Pop();
}

Console.WriteLine();
Console.WriteLine("Test input:");
Console.WriteLine(Evaluate("1 + 2 * 3 + 4 * 5 + 6", FlippedPrecedenceParser));
Console.WriteLine(Evaluate("1 + (2 * 3) + (4 * (5 + 6))", FlippedPrecedenceParser));
Console.WriteLine(Evaluate("2 * 3 + (4 * 5)", FlippedPrecedenceParser));
Console.WriteLine(Evaluate("5 + (8 * 3 + 9 + 3 * 4 * 3)", FlippedPrecedenceParser));
Console.WriteLine(Evaluate("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", FlippedPrecedenceParser));
Console.WriteLine(Evaluate("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", FlippedPrecedenceParser));

Console.WriteLine();
Console.WriteLine("Part 2:" + myInput.Select(x => Evaluate(x, FlippedPrecedenceParser)).Sum());