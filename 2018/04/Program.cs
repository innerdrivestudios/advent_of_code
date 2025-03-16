//Solution for https://adventofcode.com/2018/day/4 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: actions of different guards during the minute hours (begin shift, asleep, wake)

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Part 1 -  Find out which guard sleeps the most and during which minute.
//              Then multiply the guard's ID with that minute.

// Let's start by sorting the log, so that we can actually parse it.

string[] orderedLogs = myInput
    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
    .Order()
    .ToArray();

/**
Console.WriteLine("ORDERED LOG:");
Console.WriteLine("=============================================");
foreach (string line in orderedLogs) Console.WriteLine(line);
Console.WriteLine("=============================================");
/**/

// Approach and things to note for part 1:
// - we only care about the sleeping part
// - log blocks are always at least 3 rows, but can also be 5, 7, etc
// - duty always ends with waking up
// 
// The easiest way to store the required data and process it, is a array of ints
// per guard, mapping minute to minutes asleep
//
// For every set of log entries, we'll check the start and end index of a nap
// and increase the counter for those minutes in the log.

int logEntryCount = orderedLogs.Length;

// We'll store guards in a map from guard id to sleep log
Dictionary<int, int[]> guardInfo = new Dictionary<int, int[]>();

int index = 0;

while (index < logEntryCount)
{
    // We expect a guard line first
    // e.g. [1518-04-21 00:04] Guard #3331 begins shift

    string guardLine = orderedLogs[index];
    int hashIndex = guardLine.IndexOf("#");
    if (hashIndex < 0) throw new InvalidDataException("No hash found");
    int endOfId = guardLine.IndexOf(" ", hashIndex);

    int guardID = int.Parse(guardLine.Substring(hashIndex+1, endOfId - hashIndex));

    // Now that we have the guard id we get the sleep log for that guard

    if (!guardInfo.TryGetValue(guardID, out int[] sleepLog)) { 
        guardInfo [guardID] = sleepLog = new int [60];
    }

    // Now we keep processing sleep/wake entries until we hit a new guard line

    do
    {
        int startSleep = GetTime(orderedLogs[index + 1]);
        int stopSleep = GetTime(orderedLogs[index + 2]);

        for (int i = startSleep; i < stopSleep; i++) sleepLog[i]++;

        index += 2;
    }
    //keep doing this while there still IS a next line and that next line is not a guard log line
    while (index + 1 < logEntryCount && (!orderedLogs[index+1].Contains("#")));

    index++;
}

int GetTime (string pLogLine)
{
    return int.Parse(pLogLine.Substring(pLogLine.IndexOf(":")+1, 2));
}

// Now that we have the sleep log for each guard, we'll find the guard with the most minutes of sleep

int mostMinutesAsleep = guardInfo.Max(x => x.Value.Sum());
var sleepiestGuard = guardInfo.Where (x => x.Value.Sum() == mostMinutesAsleep).First();

// Now check what the maximum time is that this guard is asleep during a certain minute
int[] guardLog = sleepiestGuard.Value;
int sleepiestMinuteCount = guardLog.Max();

int sleepiestMinute = -1;
for (int i = 0; i < guardLog.Length; i++)
{
    if (guardLog[i] == sleepiestMinuteCount)
    {
        sleepiestMinute = i; break;
    }
}

Console.WriteLine("Part 1:");
Console.WriteLine("Guard id:"           + sleepiestGuard.Key);
Console.WriteLine("Sleepiest minute: "  + sleepiestMinute);
Console.WriteLine("Solution: "          + (sleepiestGuard.Key * sleepiestMinute));
Console.WriteLine("");

// ** Part 2 - Slightly different selection approach, find the guard with the highest max

int guardId = -1;
int minute = -1;
int minuteCountMax = int.MinValue;

foreach (var guard in guardInfo)
{
    for (int min = 0; min < 60; min++)
    {
        if (guard.Value[min] > minuteCountMax)
        {
            minuteCountMax = guard.Value[min];
            minute = min;
            guardId = guard.Key;
        }
    }
}

Console.WriteLine("Part 2:");
Console.WriteLine("Solution: " + (guardId * minute));