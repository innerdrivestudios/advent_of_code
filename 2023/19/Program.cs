// Solution for https://adventofcode.com/2023/day/19 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of rules

// rfg{s<537:gd,x>2440:R,A}
// We map a key eg rfg to a list of rules existing of conditions and output channels... a bit unreadable but hey...
// i.e. inchannel -> (condition, outchannel) where condition might be "" (which always evaluates to true)
Dictionary<string, (string, string)[]> rules = new();

// Get all the input split into paragraphs
string[] myInput = File.ReadAllText(args[0]).ReplaceLineEndings().SplatParagraphs();

// And separate those paragraphs into lines...
string[] ruleDefinitions = myInput[0].SplatLines();
string[] xmasValues = myInput[1].SplatLines();

// Awkwardly parse the input inchannel -> (condition, outchannel) into a dictionary...
foreach (string ruleDefinition in ruleDefinitions)
{
    // Split each rule into all small separate parts...
    // e.g. rfg{s<537:gd,x>2440:R,A} becomes [ rfg , s<537:gd , x>2440:R , A ]
    string[] ruleParts = ruleDefinition.Splat(['{', '}', ',']);

    // Get the rule name from the parts
    string ruleName = ruleParts[0];

    // Get the conditions from the parts
    (string, string)[] mappings = new (string, string)[ruleParts.Length - 1];

    for (int i = 1; i < ruleParts.Length; i++)
    {
        //Split s<537:gd into [ s<537 , gd ] and A into [ A ]
        string[] mappingParts = ruleParts[i].Splat(":");
        
        //Single item case, use "" as condition
        if (mappingParts.Length == 1)
        {
            mappings[i - 1] = ("", mappingParts[0]);
        }
        //Dual item case
        else if (mappingParts.Length == 2)
        {
            mappings[i - 1] = (mappingParts[0], mappingParts[1]);
        }
        else throw new Exception(ruleDefinition);
    }

    rules[ruleName] = mappings;
}

// Now parse the list of xmas values into dictionaries of char to values... {x=787,m=2655,a=1222,s=2876}
List<Dictionary<char, int>> xmasValuesList = new();

foreach (string xmasValue in xmasValues)
{
    Dictionary<char, int> xmasValueMap = new();
    string[] xmasParts = xmasValue.Splat(['{', '=',',','}']);
    
    for (int i = 0; i < xmasParts.Length/2; i++)
    {
        //xmasParts looks like [x, 200,m,400,etc...
        //i*2 indexes strings, 1+i*2 indexes values, the [0] is to get the first char of the string
        xmasValueMap[xmasParts[i * 2][0]] = int.Parse(xmasParts[1 + (i * 2)]);
    }

    xmasValuesList.Add(xmasValueMap);
}

// Define some helper methods:

bool Evaluate (string pCondition, Dictionary<char, int> pXmasValue)
{
    if (pCondition.Length == 0) return true;

    char xmas = pCondition[0];
    char op = pCondition[1]; 
    int value = int.Parse(pCondition.Substring(2));

    if (op == '<') return pXmasValue[xmas] < value;
    else return pXmasValue[xmas] > value;
}

// Now run the actual tests using all the rules...

bool IsAccepted (Dictionary<string, (string, string)[]> pRules, Dictionary<char, int> pXmasValue, string pStart)
{
    (string, string)[] rules = pRules[pStart];

    foreach ((string condition, string channel) in rules)
    {
        if (Evaluate (condition, pXmasValue))
        {
            if (channel == "A") return true;
            if (channel == "R") return false;
            return IsAccepted(pRules, pXmasValue, channel);
        }
    }

    throw new Exception("Should never get here");
}

Console.WriteLine("Part 1: " + xmasValuesList.Where (x => IsAccepted(rules, x, "in")).Sum (x => x.Values.Sum()));

// Looking at the sample and assuming the following ranges:
//
// in{s<1351:px,qqz}
// px{a<2006:qkq, m>2090:A, rfg}
// qqz{s>2770:qs, m<1801:hdj, R}
// rfg{s<537:gd, x>2440:R, A}
// pv{a>1716:R, A}
// lnx{m>1548:A, A}
// qs{s>3448:A, lnx}
// qkq{x<1416:A, crn}
// crn{x>2662:A, R}
// gd{a>3333:R, R}
// hdj{m>838:A, pv}
//
// Can we filter through all the rules and decide WHEN/IF we would reach A?

// in:
// (x=1..4000, m=1..4000, a=1..4000, s=1..4000) -> in{s<1351:px,qqz}
//  -> (x=1..4000, m=1..4000, a=1..4000, s=1..1350) -> px
//  -> (x=1..4000, m=1..4000, a=1..4000, s=1351..4000) -> px
//  -> (x=1..4000, m=1..4000, a=1..4000, s=1..4000) -> qqz
//
// px:
//  (x=1..4000, m=1..4000, a=1..4000, s=1..1350)        -> px{a<2006:qkq, m>2090:A, rfg}
//  -> (x=1..4000, m=1..4000, a=1..2005, s=1..1350)     -> qkq
//  -> (x=1..4000, m=1..4000, a=2006..4000, s=1..1350)  -> qkq
//  -> (x=1..4000, m=2091..4000, a=1..4000, s=1..1350)  -> A (DONE) -> Calculate amount of combinations !
//  -> (x=1..4000, m=1..4000, a=1..4000, s=1..1350)     -> rfg
//  (x=1..4000, m=1..4000, a=1..4000, s=1351..4000)     -> px{a<2006:qkq, m>2090:A, rfg}
//
// Etc...

// In other words, we define and adjust ranges, sent these to our channels to figure out whether we
// eventually accept them or not ... we only care about ranges that are eventually Accepted,
// and when they ARE accepted we multiply the delta of all ranges (xRange*mRange*etc) and add them to the total...

Dictionary<char, (int, int)> xmasValueRanges = new()
{
    {'x', (1,4000)},
    {'m', (1,4000)},
    {'a', (1,4000)},
    {'s', (1,4000)}
};

long CountRanges (Dictionary<char, (int, int)> pRanges)
{
    long result = 1;
    foreach (var kv in pRanges)
    {
        result *= (kv.Value.Item2 - kv.Value.Item1 + 1);
    }

    return result;
}

long CountAccepted (Dictionary<string, (string, string)[]> pRules, Dictionary<char, (int, int)> pXmasValueRanges, string pStart)
{
    // Base cases
    if (pStart == "A") return CountRanges(pXmasValueRanges);
    if (pStart == "R") return 0;

    // Get the rules for this call
    (string, string)[] rules = pRules[pStart];

    long totalAccepted = 0;

    // Now we go over each rule, and count the accepted values per range...
    Dictionary<char, (int, int)> currentRanges = pXmasValueRanges;

    foreach (var rule in rules)
    {
        // No condition? 
        if (rule.Item1 == "")
        {
            // Send the whole range to the next step
            totalAccepted += CountAccepted(pRules, currentRanges, rule.Item2);
            // Rules without conditions are always the last...
            break;
        }
        else // we have a condition...
        {
            //First get the char (aka channel) for which we need to adjust the range...
            char channel = rule.Item1[0];
            char op = rule.Item1[1];
            var rangeToAdjust = currentRanges[channel];

            //And the comparison value...
            int comparisonValue = int.Parse(rule.Item1.Substring(2));

            //If the comparison value is outside the range we need to adjust, we just use the full range
            if (comparisonValue <= rangeToAdjust.Item1 || comparisonValue >= rangeToAdjust.Item2) return CountAccepted(pRules, currentRanges, rule.Item2);

            // Else we need to split the range where we send 1 range into the next channel and continue with the remainder
            (int, int) range1 = rangeToAdjust;
            (int, int) range2 = rangeToAdjust;

            // The basic idea is reduce the end of the first range and increase the start of the second range,
            // in pseudo like this:
            //range1.Item2 = comparisonValue;
            //range2.Item1 = comparisonValue;

            // But the actual value also depends on whether we are using < or >
            // E.g. if x<1500 we need to split into ..1499 and 1500..
            // but if x>1500 we need to split into ..1500 and 1501..

            int baseValue = comparisonValue - (op == '<' ? 1 : 0);
            range1.Item2 = baseValue;
            range2.Item1 = baseValue+1;

            // And then the question remains... what do we do with these ranges?

            if (op == '<')
            {
                //If our condition applied to everything below a certain value
                //count the accepted values in that new range
                Dictionary<char, (int, int)> newRanges1 = new(currentRanges);
                newRanges1[channel] = range1;
                totalAccepted += CountAccepted(pRules, newRanges1, rule.Item2);

                //And continue with the rest...
                currentRanges[channel] = range2;
            }
            else // we do it the other way around
            {
                Dictionary<char, (int, int)> newRanges2 = new(currentRanges);
                newRanges2[channel] = range2;
                totalAccepted += CountAccepted(pRules, newRanges2, rule.Item2);
                currentRanges[channel] = range1;
            }
        }
    }

    return totalAccepted;
}

Console.WriteLine("Part 2: " + CountAccepted(rules, xmasValueRanges, "in"));










