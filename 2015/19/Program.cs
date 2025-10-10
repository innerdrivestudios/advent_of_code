// Solution for https://adventofcode.com/2015/day/19 (Ctrl+Click in VS to follow link)

using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of grammar rules and a sentence to create with this grammar (all the way at the end)
string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

// ** Part 1: Your task: derive some subrules and check if the sentence can be created from the grammar

// Step 1.Process the input into the sentence and the grammar rules:

// - Split the rules from the sentence
string[] myInputParts = myInput.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

// - Extract all the rules, since we have a bunch of duplicate keys mapping to values,
//   not using a dictionary here since it would only complicate things.
//   So this is a list of shorter keys to longer values, just like the provided input list.
List<(string key, string value)> rules =
	myInputParts[0].
	Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).
	Select(x => x.Split(" => ", StringSplitOptions.RemoveEmptyEntries)).
	Select(x => (x[0], x[1])).
	ToList(); 

//- And extract the sentence
string sentence = myInputParts[1].Trim();

/////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////    PART 1    //////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////

Console.WriteLine(
	"Part 1 - Distinct molecule count after 1 replacement:"+
	DistinctMoleculeCountAfterOneReplacement (sentence, rules)
);


// Applies all the given rules to see how many different variants (next steps) of the input string can be constructed
int DistinctMoleculeCountAfterOneReplacement(string pInput, List<(string, string)> pRules)
{
	HashSet<string> uniqueResults = new HashSet<string>();

	foreach (var rule in pRules)
	{
		ApplyRule(rule, pInput, uniqueResults);
	}

	return uniqueResults.Count;
}

// Apply a single rule to the input (which can match countless times and result in many different strings)
void ApplyRule((string key, string value) pRule, string pInput, HashSet<string> pResults)
{
	Regex replacer = new Regex(pRule.key);
	MatchCollection matches = replacer.Matches(pInput);

	foreach (Match match in matches)
	{
		//apply rule to all matches one by one and not in one go otherwise we'll get the wrong results :)
		string resultString = replacer.Replace(pInput, pRule.value, 1, match.Index);
		pResults.Add(resultString);
	}
}

/////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////    PART 2    //////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////

Console.WriteLine("Part 2: Reducing Rudolph's medicine :) \n");

//This part took a huge amount of time (as in DAYS) and tinkering until I thought "maybe he's giving a hint in part 1" :)

//We are going to make 2 sets of rules, based on whether the rules expands to RnAr (which are both terminal nodes)
//BUT we also include the expanded version of those same rules (basically what we did in part 1 for the input string)
List<(string key, string value, int count)> RnArRules = new();		//the rules that expand to RnArRules
List<(string key, string value, int count)> NonRnArRules = new();	//all the rules that expand to something else then RnArRules

ProcessRules(rules, RnArRules, NonRnArRules);

void ProcessRules(
		List<(string key, string value)> pInputRules,	
		List<(string key, string value, int count)> pRnArRules, 
		List<(string key, string value, int count)> pNonRnArRules 
	)
{
	foreach (var ruleA in pInputRules)
	{
		if (ruleA.value.Contains("Rn"))
		{
			//base rule maps a key directly to a value so the expansion count is 1
			pRnArRules.Add((ruleA.key, ruleA.value, 1));
		}
		else
		{
            //base rule maps a key directly to a value so the expansion count is 1
            pNonRnArRules.Add((ruleA.key, ruleA.value, 1));
		}

		//Now calculate which values we can generate if we expand rule.value ONCE 
		HashSet<string> uniqueResults = new HashSet<string>();
		
		//Each each rule we have, apply the rule to value of the RnAr rule, to get like a second level expansion
		foreach (var ruleB in pInputRules)
		{
			ApplyRule(ruleB, ruleA.value, uniqueResults);
		}

		//Now with all the values we found for that ruleB
		foreach (var result in uniqueResults)
		{
			//And store a 2 since it is two expansions from the original key to this newly calculated value
			if (result.Contains("Rn")) pRnArRules.Add((ruleA.key, result,2));
			else pNonRnArRules.Add((ruleA.key, result,2));
		}
	}
}

//Now that we have those rules, we're gonna try and reduce the input string counting the replacements made...

int replacementsMade = 0;

//This part was mostly trial and error, apply the RnArRules until you can't no more, swap to NonRnArRules, and repeat
Stopwatch stopwatch = Stopwatch.StartNew();

Console.WriteLine(sentence + "\n");

while (sentence != "e")
{
	do
	{
		string newSentence = ReduceString(sentence, RnArRules, ref replacementsMade);
		if (newSentence == sentence) break;
		else
		{
			sentence = newSentence;
			Console.WriteLine(sentence + "\n");
		}
	} while (true);

    do
    {
        string newSentence = ReduceString(sentence, NonRnArRules, ref replacementsMade);
		if (newSentence == sentence) break;
		else
		{
			sentence = newSentence;
			Console.WriteLine(sentence + "\n");
		}
    } while (true);
}

Console.WriteLine(replacementsMade + " replacements made.");
Console.WriteLine("Calculated in "+stopwatch.ElapsedMilliseconds + " milliseconds");

string ReduceString (string pInput, List<(string key, string value, int count)> pRules, ref int pReplacedCount)
{
    foreach (var rule in pRules)
    {
        Regex replacer = new Regex(rule.value);
        //replace one string at a time and count the replacements...
		while (true)
		{
			string resultString = replacer.Replace(pInput, rule.key, 1);
			if (resultString.Length != pInput.Length)
			{
				pReplacedCount += rule.count;
				pInput = resultString;
			}
			else break;
		}

    }
    return pInput;
}

