// Solution for https://adventofcode.com/2020/day/16 (Ctrl+Click in VS to follow link)

using Field = (string name, (int start, int end) r1, (int start, int end) r2);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: ticketing info in 3 parts:
// - ticket fields + ranges
// - your ticket info
// - nearby tickets

using System.Text.RegularExpressions;

string[] myInput = File.ReadAllText(args[0])
    .ReplaceLineEndings()
    .Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

// Remove headers...
myInput[1] = myInput[1].Replace("your ticket:", "").Trim();
myInput[2] = myInput[2].Replace("nearby tickets:", "").Trim();

// ** Part 1: Figure out all ticket values that falls outside of any range and sum them

// Get all the separate integers in the nearby ticket values, note we can't use a hashset since we need to keep duplicates!
List<int> nearbyTicketValues = myInput[2]
    .Split([Environment.NewLine, ","], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
    .Select(int.Parse).ToList();

// Now simply loop over all ranges, delete all numbers within the range from the nearby ticket values
// Whatever is left is invalid...

Regex rangeParser = new Regex(@"(\d+)-(\d+)", RegexOptions.Compiled);

MatchCollection matches = rangeParser.Matches(myInput[0]);
foreach (Match match in matches)
{
    int start = int.Parse(match.Groups[1].Value);
    int end = int.Parse(match.Groups[2].Value);
    for (int i = start; i <= end; i++) while (nearbyTicketValues.Remove(i));
}

Console.WriteLine("Part 1: " + nearbyTicketValues.Sum());

// ** Part 2: Work out which field is which on your ticket,
// look for the six fields on your ticket that start with the word departure.
// What do you get if you multiply those six values together?

// Ok, so part 1 was approached too simply.
// The answer was correct, but we are lacking the data structures to answer the second part,
// so we'll parse everything again and now in a format that is more suitable to solving the given problem.

var validTicketData =
     myInput[2]                                                            //all lines with , separated ticket data, separated by newlines
    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)     //all separate lines... 
    .Select(x => x.Split(",").Select(int.Parse).ToList())                  //for each line, split it on , and select all integers from it
    .Where (x => !x.Intersect(nearbyTicketValues).Any())                   //but keep only those lines that don't have any matches with invalid ticket data
    .ToList();                                                             //so now we have a list of lists of ticket data

// Now we have all valid ticket data ... we need to deduce which field is which...
// For this we first need to know the names and valid ranges for each field...

Regex fieldParser = new Regex(@"(\D+): (\d+)-(\d+) or (\d+)-(\d+)");
MatchCollection fieldMatch = fieldParser.Matches(myInput[0].ReplaceLineEndings(""));

List<Field> fields = new();

foreach (Match match in fieldMatch)
{
    fields.Add(
        (
            match.Groups[1].Value,
            ( 
                int.Parse(match.Groups[2].Value),
                int.Parse(match.Groups[3].Value)
            ),
            (
                int.Parse(match.Groups[4].Value),
                int.Parse(match.Groups[5].Value)
            )
        )
    );
}

// Now we need to check each column of ticket data against each field to see if all values are valid...
// AND we also need to check if that column is ONLY valid for that field or other fields as well...

bool ColumnMatchesFieldAndRange (List<List<int>> pAllColumns, int pColumnIndex, Field pField)
{
    int rows = pAllColumns.Count;

    for (int i = 0; i < rows; i++)
    {
        int rowValue = pAllColumns[i][pColumnIndex];
        if (
            !(
                (pField.r1.start <= rowValue && rowValue <= pField.r1.end) ||
                (pField.r2.start <= rowValue && rowValue <= pField.r2.end)
            )
            ) return false;
    }
    return true;
}

// We could do this in 2 ways:
// - find all columns that could match a field
// - find all fields that match a column

// We'll do it the first way:
Dictionary<int, List<int>> columnToFieldmap = new();

for (int column = 0; column < validTicketData[0].Count; column++)
{
    for (int field = 0; field < fields.Count; field++)
    {
        if (ColumnMatchesFieldAndRange(validTicketData, column, fields[field]))
        {
            Console.WriteLine("Column " + column + " matches field:" + fields[field].name);
            columnToFieldmap[column] = columnToFieldmap.GetValueOrDefault(column, new List<int>());
            columnToFieldmap[column].Add(field);
        }
    }
}

// Now we need to filter/reduce these matches/options, which we'll do like this:
//  - We look for any column that only maps to 1 field
//  - We store the mapping from that FIELD to that column index:
Dictionary<int, int> fieldToColumnIndex = new();

while (columnToFieldmap.Count > 0)
{
    Console.WriteLine(columnToFieldmap.Count + " items to reduce...");

    // Find all columns mapped to a single field
    var columnsWithSingleField = columnToFieldmap.Where(x => x.Value.Count == 1);

    // Just pick the first one and remove that column from the list
    var columnWithSingleField = columnsWithSingleField.First();
    columnToFieldmap.Remove(columnWithSingleField.Key);

    //And remove that column's field index from the field list of all other columns...
    foreach (var column in columnToFieldmap)
    {
        column.Value.Remove(columnWithSingleField.Value[0]);
    }

    Console.WriteLine("Mapping column " + columnWithSingleField.Key + " to field " + fields[columnWithSingleField.Value[0]]);
    fieldToColumnIndex[columnWithSingleField.Value[0]] = columnWithSingleField.Key;
}

Console.WriteLine("All columns mapped?" + (fieldToColumnIndex.Count == fields.Count));

// Phew! Now that we've got all that, just do a simple run over and collect all values for departure fields

int[] myTicketData = myInput[1]
    .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Select(int.Parse)
    .ToArray();

// Note the 'long' :) whoopsie...

List<long> departureValues = new();
for(int i = 0; i < fields.Count; i++)
{
    if (fields[i].name.StartsWith("departure")) departureValues.Add(myTicketData [fieldToColumnIndex[i]]);
}

Console.WriteLine("Departure values count:" + departureValues.Count);

Console.WriteLine("Part 2:" + departureValues.Aggregate((x,y) => x * y));




