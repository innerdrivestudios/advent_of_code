//Solution for https://adventofcode.com/2016/day/10 (Ctrl+Click in VS to follow link)

//Explanation below
using System.Text.RegularExpressions;
using Bot = (int botID, int lowHandlerID, int highHandlerID, System.Collections.Generic.List<int> inbox);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

//Your input: a list of which bots are given which chips (each chip being represented by a number)
//Chips can be put ON the assembly line         => value 2 goes to bot 2, 
//Chips can be passed from bot to bot           => bot 142 gives low to bot 110 and high to bot 163
//Chips can be passed from bot to output line   => bot 109 gives low to output 0 and high to bot 43

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Part 1:
// Based on your instructions, what is the number of the bot that is responsible
// for comparing value-61 microchips with value-17 microchips?

///////////////////////////// INPUT PRE PROCESSING //////////////////////////

// What should be the setup? 

// There is basically only one thing we need to store/process:
//  - We have bots that gather 2 chips with different id's and pass them on to two bins:
//       - a bin for the chip with the low id (another bot's input bin or an output bin)
//       - a bin for the chip with the high id (another bot's input bin or an output bin)
//  - To distinguish output and bots we'll store outputs as their id + a 1000 value
//
// To store the input data easily, we link each bot id to some data for it:
// int botID => (int botID, int lowHandlerID, int highHandlerID, System.Collections.Generic.List<int> inbox);
// where the inbox is an inbox of chip id's and the rest is probably self explanatory.
// After that we'll use the "value x goes to bot y" lines to pre fill some inboxes:

//Map a bot to its data
Dictionary<int, Bot> botData = new();

//If we detect a bot has two input values, it is ready to be processed
List<int> botsToProcess = new ();

//Output map from output bin ID to chip ID (required for part 2)
Dictionary<int, int> outputMap = new();

SetupBots (myInput, botData, botsToProcess);

void SetupBots(string pInput, Dictionary<int, Bot> pBotData, List<int> pBotsToProcess)
{
    string[] instructions = pInput.Split (Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

    //First set up all bot's (with an empty inbox)

    Regex botInstruction = new Regex(@"bot (\d+) gives low to (bot|output) (\d+) and high to (bot|output) (\d+)"); //bot 123 gives low to bot 191 and high to bot 162

    foreach (string instruction in instructions)
    {
        Match match = botInstruction.Match(instruction);

        if (match.Success)
        {
            int botID = int.Parse(match.Groups[1].Value);
            int lowHandlerID = int.Parse(match.Groups[3].Value) + ((match.Groups[2].Value == "output")?1000:0);
            int highHandlerID = int.Parse(match.Groups[5].Value) + ((match.Groups[4].Value == "output")?1000:0);

            Bot bot = (botID, lowHandlerID, highHandlerID, new List<int>());
            pBotData[bot.botID] = bot;
        }
    }

    //Then fill their initial inbox config...

    Regex valueInstruction = new Regex(@"value (\d+) goes to bot (\d+)"); 

    foreach (string instruction in instructions)
    {
        Match match = valueInstruction.Match(instruction);

        if (match.Success)
        {
            int chipID = int.Parse(match.Groups[1].Value);
            int handlerID = int.Parse(match.Groups[2].Value);

            Bot bot = pBotData[handlerID];
            bot.inbox.Add(chipID);

            //Bot's only 'trigger' if they've received two chips
            if (bot.inbox.Count == 2) botsToProcess.Add(handlerID);
        }
    }
}

Console.WriteLine ("Part 1 - Bot responsible for 17 & 61: " + FindResponsible(botData, botsToProcess, [17, 61], outputMap));

int FindResponsible(Dictionary<int, Bot> pBotData, List<int> pBotsToProcess, List<int> pChipIds, Dictionary<int, int> pOutputmap)
{
    //make sure the values we are looking for are in ascending order to simplify the rest of the code
    pChipIds.Sort();

    int responsibleBotId = -1;

    while (pBotsToProcess.Count > 0)
    {
        //Get the bot to process from the end (quicker to remove)
        int indexToProcess = pBotsToProcess.Count - 1;
        int botIDToProcess = pBotsToProcess[indexToProcess];
        botsToProcess.RemoveAt(indexToProcess);

        //Get the bot and sort its inbox to match the passed in pChipIds
        Bot bot = pBotData[botIDToProcess];
        bot.inbox.Sort();                   
        
        //Did we find the bot responsible?
        if (bot.inbox[0] == pChipIds[0] && bot.inbox[1] == pChipIds[1])
        {
            responsibleBotId = botIDToProcess;
            //Don't stop/return here, we need the full process to complete for part two...
        }

        //In case this was not the bot you are looking for :]...

        //Output is for another bot? (either low or high)
        if (bot.lowHandlerID < 1000) { 
            Bot lowHandler = pBotData[bot.lowHandlerID];
            lowHandler.inbox.Add(bot.inbox[0]);
            if (lowHandler.inbox.Count == 2) pBotsToProcess.Add(lowHandler.botID);
        }
        else //Output is an actual output bin?
        {
            pOutputmap[bot.lowHandlerID-1000] = bot.inbox[0];
        }

        //Output is another bot? (either low or high)
        if (bot.highHandlerID < 1000)
        {
            Bot highHandler = pBotData[bot.highHandlerID];
            highHandler.inbox.Add(bot.inbox[1]);
            if (highHandler.inbox.Count == 2) pBotsToProcess.Add(highHandler.botID);
        }
        else //Output is an actual output bin?
        {
            pOutputmap[bot.highHandlerID-1000] = bot.inbox[1];
        }
    }

    return responsibleBotId;
}

// ** Part 2: Get the output chips in bin 0, 1 and 2 and multiply their values...

Console.WriteLine("Part 2 - Multiplied value of output: " + GetValueOf([0,1,2], outputMap));

int GetValueOf(List<int> pOutputs, Dictionary<int, int> pOutputMap)
{
    int result = 1;

    foreach (int outputBin in pOutputs)
    {
        result *= pOutputMap[outputBin];
    }

    return result;
}
