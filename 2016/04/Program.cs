//Solution for https://adventofcode.com/2016/day/4 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** My input: a whole list of weird strings for which we need to filter out strings with a failed checksum:
// Sample format: not-a-real-room-404[oarel] (base-content-roomid[checksum])

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// Your task: sum up the IDs of the strings with a correct checksum
// What is a correct checksum? Good question, I had to reread the description 10 times before I got it too ;).
// But anyway, the basic process is this:
// - take all characters in the base-content, ignoring the dashes, e.g:
//      not-a-real-room-404[oarel] => notarealroom
// - create a map from char to charcount, e.g:
//      n - 1
//      o - 3
//      t - 1
//      a - 2
//      r - 2
//      e - 1
//      l - 1
//      m - 1
// - sort all pairs by count and then by char
//      o - 3
//      a - 2
//      r - 2
//      e - 1
//      l - 1
//      m - 1
//      n - 1
//      t - 1
// - take the chars of the first 5 pairs:
//      oarel -> checksum matches!

// Now in code:

// First get all the regex matches based on the pattern provided:

// ([a-z\-]+)       => capture a to z and - multiple
// (\d+)            => capture a sequence of digits
// \[([a-z]+)\]     => capture whatever chars between [ and ] (need to escape those)

Regex stringMatcher = new Regex(@"([a-z\-]+)(\d+)\[([a-z]+)\]");
MatchCollection matches = stringMatcher.Matches(myInput);

Console.WriteLine("Part 1 - Sum of the sector IDs of the real rooms:" + GetRealRoomsSectorIdsSum(matches));

long GetRealRoomsSectorIdsSum(MatchCollection pMatches)
{
    long realRoomIdSum = 0;

    foreach (Match match in matches)
    {
        //Evaluate room returns the room id if the string is valid, 0 otherwise
        realRoomIdSum += EvaluateRoom(match);
    }

    return realRoomIdSum;
}

//return the room id if the room string is valid, 0 otherwise
long EvaluateRoom(Match match)
{
    //The match we are getting:
    //groups[0] -> everything
    //groups[1] -> the first part of characters
    //groups[2] -> the room id as a string
    //groups[3] -> the checksum

    string chars = match.Groups[1].Value.Replace("-", "");              ////Remove all dashes

    //Create char count map (thought about doing this with LINQ, but probably gets pretty unreadable)
    Dictionary<char, int> charCountMap = new Dictionary<char, int>();
    foreach (char c in chars)
    {
        charCountMap.TryGetValue(c, out int count);
        charCountMap[c] = count+1;
    }

    //Convert dictionary to a list of pairs we can sort with a custom sorter
    List<(char, int)> sortedCharCountMap = charCountMap.Select(x => (x.Key, x.Value)).ToList();
    sortedCharCountMap.Sort(CharCountSorter);

    //Take the first 5 elements from the sorted map, but only the char, not the int
    string checksum = String.Join("", sortedCharCountMap.Take(5).Select (x => x.Item1));

    //if the checksum is correct, return the room id, 0 otherwise
    return (checksum == match.Groups[3].Value) ? int.Parse(match.Groups[2].Value) : 0;
}

int CharCountSorter((char, int) a, (char, int) b)
{
    //if the count is the same, compare a to b (ascending)
    if (a.Item2 == b.Item2) return a.Item1.CompareTo(b.Item1);
    //otherwise compare based on counts (descending)
    else return b.Item2.CompareTo(a.Item2);
}

Console.WriteLine("Part 2 - Finding the ID of the right room....\n");
Console.WriteLine("Printing all of these since they are so hilarious, just scroll through them for the answer...\n");
long northpoleRoomId = FindTheNorthPoleRoom(matches);
Console.WriteLine("\nThe northpole room id is:" + northpoleRoomId);

long FindTheNorthPoleRoom(MatchCollection matches)
{
    long northpoleRoomID = -1;

    foreach (Match match in matches)
    {
        long roomID = EvaluateRoom(match);

        if (roomID > 0) 
        {
            string decodedString = DecodeRoom(match);

            //Just to make it clearer ;)
            if (decodedString.Contains("pole"))
            {
                Console.WriteLine("\n*** " + decodedString.ToUpper() + " ==> ID:" + roomID + " ***\n");
                northpoleRoomID = roomID;
            }
            else
            {
                Console.WriteLine(decodedString); 
            }
        }
    }

    return northpoleRoomID;
}

string DecodeRoom(Match match)
{
    //The match we are getting:
    //groups[0] -> everything
    //groups[1] -> the first part of characters
    //groups[2] -> the room id as a string
    //groups[3] -> the checksum

    //Dashes to spaces
    string chars = match.Groups[1].Value.Replace("-", " ");
    int cycle = int.Parse(match.Groups[2].Value);

    string newString = "";

    for (int i = 0; i < chars.Length; i++)
    {
        if (chars[i] == ' ')
        {
            newString += chars[i];
        }
        else
        {
            newString += (char)('a'+ ((chars[i] - 'a') + cycle) % (26));
        }
    }

    return newString;    
}