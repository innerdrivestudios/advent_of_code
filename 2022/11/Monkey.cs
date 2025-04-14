using System.Numerics;
using System.Text.RegularExpressions;

/** Parses and operates a monkey based on data like this:

Monkey 0:
  Starting items: 98, 97, 98, 55, 56, 72
  Operation: new = old * 13
  Test: divisible by 11
    If true: throw to monkey 4
    If false: throw to monkey 7

*/
class Monkey
{
    public readonly int id;                                      //monkey id
    public readonly List<long> startingItems;                   //list of starting items as given
    (char op, long operand) operation;                           // * or + and a number times OLD
    (long divisibleBy, int trueMonkey, int falseMonkey) test;
    
    private static Dictionary<int, Monkey> monkeyMap = new Dictionary<int, Monkey>();

    private long _itemsInspected = 0;
    public long ItemsInspected => _itemsInspected;

    private long worryLevelDivider = 1;

    //We use this value to modulo any results in order to avoid overflow
    public static long globalModulo = 1;

    public Monkey (string pMonkeyData, long pWorryLevelDivider)
    {
        Regex monkeyParser = new Regex(
            @"Monkey (\d+):"+Environment.NewLine+                                   //Group 1
            @"\s+Starting items: (\d+(?:, ?\d+)*)" + Environment.NewLine +          //Group 2
            @"\s+Operation: new = old ([*+]) (\d+|old)" + Environment.NewLine +     //Group 3 and 4
            @"\s+Test: divisible by (\d+)" + Environment.NewLine +                  //Group 5
            @"\s+If true: throw to monkey (\d+)" + Environment.NewLine +            //Group 6
            @"\s+If false: throw to monkey (\d+)"                                   //Group 7
        );

        Match match = monkeyParser.Match( pMonkeyData );
        if (!match.Success) throw new Exception("Invalid monkey data");
        
        id = int.Parse(match.Groups[1].Value);
        startingItems = match.Groups[2].Value.Split(",", StringSplitOptions.TrimEntries).Select(long.Parse).ToList();

        if (match.Groups[4].Value == "old")
        {
            operation.op = 'X'; //easiest way to encode this
        }
        else
        {
            operation.op = match.Groups[3].Value[0];
            operation.operand = long.Parse (match.Groups[4].Value);
        }

        test.divisibleBy = long.Parse(match.Groups[5].Value);
        test.trueMonkey = int.Parse(match.Groups[6].Value);
        test.falseMonkey = int.Parse(match.Groups[7].Value);

        worryLevelDivider = pWorryLevelDivider;

        //We need to maintain the property that whatever the modulo result,
        //it still needs to be divisible by our test value
        globalModulo *= test.divisibleBy;

        //Now we got all monkey data successfully parsed
        monkeyMap[id] = this;
    }

    public override string ToString()
    {
        string items = string.Join(", ", startingItems);
        string opStr = $"new = old {operation.op} {operation.operand}";
        string testStr =
            $"  Test: divisible by {test.divisibleBy}\n" +
            $"    If true: throw to monkey {test.trueMonkey}\n" +
            $"    If false: throw to monkey {test.falseMonkey}";

        return $"Monkey {id}:\n" +
               $"  Starting items: {items}\n" +
               $"  Operation: {opStr}\n" +
               $"{testStr}";
    }

    public void Inspect()
    {
        //Implementing the monkey test attributes according to specs
        for (int i = startingItems.Count-1; i >= 0; i--)
        {
            startingItems[i] = (InspectItem(startingItems[i]) / worryLevelDivider) % (globalModulo);
            bool testResult = (startingItems[i] % (long)test.divisibleBy) == 0;

            monkeyMap[testResult ? test.trueMonkey : test.falseMonkey].startingItems.Add(startingItems[i]);
            startingItems.RemoveAt(i);

            _itemsInspected++;
        }
    }

    private long InspectItem(long pItem)
    {
        if (operation.op == 'X') return pItem * pItem;
        if (operation.op == '*') return pItem * (long)operation.operand;
        if (operation.op == '+') return pItem + (long)operation.operand;

        throw new NotImplementedException();
    }
    
}

