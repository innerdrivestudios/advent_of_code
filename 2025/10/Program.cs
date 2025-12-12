// Solution for https://adventofcode.com/2025/day/10 (Ctrl+Click in VS to follow link)

using Config = (int correctState, int[] buttonFlags, int[][] buttons, int[] joltageRequirements);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: ....

string[] myInput = File.ReadAllLines(args[0]);

// ** Define some helper methods

string GetString (string pInput) => pInput.Substring(1, pInput.Length-2);

int ConvertHashTagAndDotsToBits (string pInput)
{
    return Convert.ToInt32(String.Concat(pInput.Replace('.', '0').Replace('#', '1').Reverse()), 2);
}

int ConvertNumberListToBits(int[] pButtonList)
{
    int result = 0;
    foreach (int buttonId in pButtonList) result |= 1 << buttonId;

    return result;
}

int[] ConvertNumberStringToNumberList(string pInput) =>
    GetString(pInput).Split(",").Select(int.Parse).ToArray();

Config GetConfig (string pConfigurationInfo)
{
    Config config = new Config();

    string[] configInfo = pConfigurationInfo.Split(' ');

    config.correctState = ConvertHashTagAndDotsToBits(GetString(configInfo[0]));

    config.buttonFlags = new int[configInfo.Length - 2];
    config.buttons = new int[configInfo.Length - 2][];
    
    for (int i = 0; i < config.buttons.Length;i++)
    {
        config.buttons[i] = ConvertNumberStringToNumberList(configInfo[i + 1]);
        config.buttonFlags[i] = ConvertNumberListToBits(config.buttons[i]);
    }

    config.joltageRequirements = ConvertNumberStringToNumberList(configInfo.Last());

    return config;
}

// Search for part 1

int CalculateRequiredAmountOfButtons (Config pConfig)
{
    PriorityQueue<int, int> todo = new();
    Dictionary<int, int> costs = new();

    //we start off
    todo.Enqueue(0, 0);
    costs[0] = 0;

    while (todo.Count > 0)
    {
        int current = todo.Dequeue();
        int currentCost = costs[current];

        if (current == pConfig.correctState) return currentCost;

        foreach (int button in pConfig.buttonFlags)
        {
            int newState = current ^ button;
            int newCost = currentCost + 1;

            if (!costs.ContainsKey(newState) || costs[newState] > newCost)
            {
                costs[newState] = newCost;
                todo.Enqueue(newState, newCost);
            }
        }
    }

    return -1;
}

int total = 0;

foreach (string inp in myInput)
{
    Config c = GetConfig(inp);
    int presses = CalculateRequiredAmountOfButtons(c);

    total += presses;
}

Console.WriteLine("Part 1: " + total);


// ** Part 2:

int CheckJoltageRequirements(int[] pCurrent, int[] pRequired)
{
    int distance = 0;
    for (int i = 0; i < pCurrent.Length; i++)
    {
        if (pCurrent[i] > pRequired[i]) return 1;    
        if (pCurrent[i] != pRequired[i]) distance += pRequired[i] - pCurrent[i];    
    }

    return -distance;
}

int ConvertToId(int[] pJoltageReq)
{
    int total = 0;
    for (int i = 0; i < pJoltageReq.Length; i++)
    {
        total = total * 100 + pJoltageReq[i];
    }
    return total;
}

int CalculateRequiredAmountOfButtonsPart2(Config pConfig)
{
    PriorityQueue<int[], int> todo = new();
	Dictionary<int, int> costs = new();

    //we start off
    int[] start = new int[pConfig.joltageRequirements.Length];
	todo.Enqueue(start, 0);
	costs[ConvertToId(start)] = 0;

	while (todo.Count > 0)
	{
		int[] current = todo.Dequeue();
		int currentCost = costs[ConvertToId(current)];

		if (CheckJoltageRequirements(current, pConfig.joltageRequirements) == 0) return currentCost;

		foreach (int[] buttons in pConfig.buttons)
		{
            int[] newState = new int[pConfig.joltageRequirements.Length];

            for (int i = 0; i < newState.Length; i++)
            {
                newState[i] = current[i];
            }

            for (int i = 0; i < buttons.Length; i++)
            {
                newState[buttons[i]]++;
            }

			//Console.WriteLine(string.Join(",", newState));
            //Console.ReadKey();

            int joltageReq = CheckJoltageRequirements(newState, pConfig.joltageRequirements);

			if (joltageReq > 0) continue;
			//Console.WriteLine(joltageReq);
			int newCost = currentCost + 1;

            int id = ConvertToId(newState);

			if (!costs.ContainsKey(id) || costs[id] > newCost)
			{
				costs[id] = newCost;
				todo.Enqueue(newState, newCost - joltageReq);
			}
		}
	}

	return -1;
}


total = 0;
foreach (string inp in myInput)
{
	Console.WriteLine(inp);
	Config c = GetConfig(inp);
	int presses = CalculateRequiredAmountOfButtonsPart2(c);
	Console.WriteLine("P:" + presses);

	total += presses;
}

Console.WriteLine(total);
