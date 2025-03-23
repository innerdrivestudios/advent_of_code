
using System.Data;

class Solver
{
    private Dictionary<string, int> stringsToValues = new Dictionary<string, int>();
    private string[] valuesToStrings = new string[10];

    private int representedValue = 0;

    public Solver ((string[], string[]) pInputData)
    {
        //     *  **
        //    8687497
        //  0-abc efg   => 6
        //  1-  c  f    => 2*
        //  2-a cde g   => 5
        //  3-a cd fg   => 5
        //  4- bcd f    => 4*
        //  5-ab d fg   => 5
        //  6-ab defg   => 6
        //  7-a c  f    => 3*
        //  8-abcdefg   => 7*
        //  9-abcd fg   => 6

        // But how do we solve it?

        // Well, some numbers (1,4,7,8) have a unique string length... (2,4,3,7)
        Map(1, FindStringOfLength(pInputData.Item1, 2));
        Map(4, FindStringOfLength(pInputData.Item1, 4));
        Map(7, FindStringOfLength(pInputData.Item1, 3));
        Map(8, FindStringOfLength(pInputData.Item1, 7));

        // For the next part we can try to figure out what some of the scrambled characters are...
        // For example:
        // a is mapped to the difference between the characters used for number 1 and 7

        char a = valuesToStrings[7].Except(valuesToStrings[1]).First();

        // b is mapped to whichever char occurs 6 times (looking in the table header)
        // e is mapped to whichever char occurs 4 times (looking in the table header)
        // f is mapped to whichever char occurs 9 times (looking in the table header)
        // the rest is inconclusive

        string allChars = String.Concat(pInputData.Item1);
        string allDistinctChars = "abcdefg";

        Dictionary<int, char> countToCharMap = new();
        foreach (char distinctChar in allDistinctChars)
        {
            countToCharMap[allChars.Count(x => x == distinctChar)] = distinctChar;
        }

        char b = countToCharMap[6];
        char e = countToCharMap[4];
        char f = countToCharMap[9];

        // Now that we have this info, we can also figure out which char c is mapped to and which char f is mapped to
        // Since number 1 is mapped to cf, we can subtract f and we are left with c

        char c = valuesToStrings[1].Except("" + f).First();

        //Console.WriteLine("Char a is mapped to " + a);
        //Console.WriteLine("Char b is mapped to " + b);
        //Console.WriteLine("Char e is mapped to " + e);
        //Console.WriteLine("Char f is mapped to " + f);
        //Console.WriteLine("Char c is mapped to " + c);

        // Armed with this info we can solve the other numbers... 
        // e.g. 9 is mapped to the only string of length 6 that doesn't have e in it
        Map(9, pInputData.Item1.Where(x => x.Length == 6 && !x.Contains(e)).First());

        // 6 is mapped to the only string of length 6 that doesn't have c in it
        Map(6, pInputData.Item1.Where(x => x.Length == 6 && !x.Contains(c)).First());

        // 0 is mapped to the only string of length 6 which is not equal to the strings mapped to 6 & 9
        Map(0, pInputData.Item1.Where(x => x.Length == 6 && x != valuesToStrings[6] && x != valuesToStrings[9]).First());

        // 2 is mapped to the only string of length 5 that DOES contain e
        Map(2, pInputData.Item1.Where(x => x.Length == 5 && x.Contains(e)).First());

        // 5 is mapped to the only string of length 5 that DOES not contain c
        Map(5, pInputData.Item1.Where(x => x.Length == 5 && !x.Contains(c)).First());

        // 3 is mapped to the only string of length 5 which is not equal to 2 or 5
        Map(3, pInputData.Item1.Where(x => x.Length == 5 && x != valuesToStrings[2] && x != valuesToStrings[5]).First());

        // Now that all of the strings have been mapped we can solve the digits...

        foreach (string digit in pInputData.Item2)
        {
            representedValue *= 10;
            representedValue += stringsToValues[digit];
        }

        //Console.WriteLine(representedValue);
    }

    private string FindStringOfLength(string[] pInput, int pLength)
    {
        return pInput.Where(x => x.Length == pLength).First();
    }

    private void Map (int pNumber, string pString)
    {
        valuesToStrings[pNumber] = pString;
        stringsToValues[pString] = pNumber;
    }

    public void PrintSolvedTable()
    {
        for (int i = 0; i < valuesToStrings.Length; i++)
        {
            Console.WriteLine($"[{i}] - {valuesToStrings[i]}");
        }
    }

    public int GetValue()
    {
        return representedValue;
    }


}
