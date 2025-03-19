//Solution for https://adventofcode.com/2024/day/5 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of page ordering rules eg X|Y where X must come before Y
//                and a list of ordered pages, for which we need to check if they are correct

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// Step 1: Parse all the input

int newLineIndex = myInput.IndexOf(Environment.NewLine + Environment.NewLine);
string orderingRules = myInput.Substring(0, newLineIndex);
string[] orderingRulesAsStringPairs = orderingRules.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

// Create the ordering lists of which page needs to come before which pages

Dictionary<string, List<string>> orderingRuleTable = new Dictionary<string, List<string>>();

foreach (string pair in orderingRulesAsStringPairs)
{
    string[] actualPair = pair.Split('|');
    string key = actualPair[0]; 
    string value = actualPair[1];

    List<string> values;
    if (!orderingRuleTable.TryGetValue(key, out values))
    {
        values = new List<string>();
        orderingRuleTable[key] = values;
    }

    values.Add(value);
}

// Now parse all the lists of pages, into a list of string arrays

string pageListsString = myInput.Substring(newLineIndex + Environment.NewLine.Length * 2);
List<string[]> pageLists = pageListsString
    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
    .Select (x => x.Split (",", StringSplitOptions.RemoveEmptyEntries))
    .ToList();

// ** Part 1: Get the total sum of the center values of the correctly ordered page listings

List<string[]> correctPageListCount = new List<string[]>();
List<string[]> incorrectUpdateList = new List<string[]>();

foreach (string[] pageList in pageLists)
{
    if (IsCorrect(pageList)) correctPageListCount.Add(pageList);
    else incorrectUpdateList.Add(pageList);
}

bool IsCorrect(string[] pPageList)
{
    for (int i = 1; i < pPageList.Length; i++)
    {
        try
        {
            //Every page needs to be part of the ordering table of the page that comes 
            //before it, since the orderingRuleTable indicates the correct order
            if (!orderingRuleTable[pPageList[i - 1]].Contains(pPageList[i])) return false;
        } catch 
        { 
            return false; 
        }
    }
    return true;
}

int correctCenterSum = 0;
for (int i = 0; i < correctPageListCount.Count; i++)
{
    int centerElement = correctPageListCount[i].Length / 2;
    correctCenterSum += int.Parse(correctPageListCount[i][centerElement]);
}

Console.WriteLine("Part 1: " + correctCenterSum);

// ** Part 2: Sort every incorrect list based on the orderingtable and sum their centers

for (int i = 0; i < incorrectUpdateList.Count; i++)
{
    Array.Sort(
        incorrectUpdateList[i], 
            (a,b) =>
            {
                if (orderingRuleTable.ContainsKey(a) && orderingRuleTable[a].Contains(b)) return -1;
                else return 1;
            }
        );

    //Console.WriteLine(string.Join(",", incorrectUpdateList[i]));
}

int incorrectCenterSum = 0;
for (int i = 0; i < incorrectUpdateList.Count; i++)
{
    int centerElement = incorrectUpdateList[i].Length / 2;
    incorrectCenterSum += int.Parse(incorrectUpdateList[i][centerElement]);
}

Console.WriteLine("Part 2: " + incorrectCenterSum);

