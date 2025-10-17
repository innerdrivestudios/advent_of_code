// Solution for https://adventofcode.com/2015/day/24 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Input: list of package weights
string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

// First get all the numbers and make sure they are sorted to speed up the algorithm
List<int> packages = myInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
packages.Sort((a,b) => b-a);

// ** Part 1 & 2: Create x different groups of packages of equal weights, with the first group having the least amount of packages

Console.WriteLine("Part 1: Quantum entanglement of the ideal config:" + CalculateIdealQuantumEntanglement(packages, 3));
Console.WriteLine("Part 2: Quantum entanglement of the ideal config:" + CalculateIdealQuantumEntanglement(packages, 4));

// Helper methods...

long CalculateIdealQuantumEntanglement(List<int> pPackages, int pNumberOfGroups)
{
    // Getting all permutations and testing those takes too much time, so we use a smarter approach (see the worked_example.txt)
    // Note that this method doesn't actually split the packages into x groups, it just generates all possible configurations
    // of groups with a weight of total / number of groups (we don't need more than that)
    List<List<int>> possibleConfigurations = new List<List<int>>();
    GetPossibleConfigurationsOfWeightX(pPackages, pPackages.Sum() / pNumberOfGroups, possibleConfigurations);

    // Find the minimum package length of these configs
    int minPackageLength = possibleConfigurations.Min (a => a.Count);
    possibleConfigurations = possibleConfigurations.Where (a => a.Count == minPackageLength).ToList();

    // And calculate the optimum quantum entanglement score ;)
    long optimumQuantumScore = possibleConfigurations.Min(a => CalculateQuantumEntanglement(a));

	return optimumQuantumScore;
}


// This method expects a descending sorted list of package weights, so it can go through all the provided
// packages in an order fashion in order to try and establish the requested weight.

void GetPossibleConfigurationsOfWeightX (List<int> pPackages, int pWeight, List<List<int>> pFoundConfigurations, List<int> pConfigurationSoFar = null)
{
    pConfigurationSoFar = pConfigurationSoFar ?? new List<int>();

    //See if we can even make the requested configuration by combining everything so far, otherwise do an early exit
    int maxWeight = pPackages.Sum() + pConfigurationSoFar.Sum();
    if (maxWeight < pWeight) return;

    //Ok, so in theory by combining items from pPackages and pConfigurationSoFar we should be able to
    //get to pWeight, the question is, can we get to it EXACTLY

    for (int i = 0; i < pPackages.Count; i++)
    {
        List<int> configurationSoFarClone = new List<int>(pConfigurationSoFar) { pPackages[i] };

        int total = configurationSoFarClone.Sum();

        if (total == pWeight)
        {
            pFoundConfigurations.Add(configurationSoFarClone);
        }
        else if (total < pWeight)
        {
            List<int> subPackages = pPackages.Slice(i+1, pPackages.Count-(i+1));
            GetPossibleConfigurationsOfWeightX(subPackages, pWeight, pFoundConfigurations, configurationSoFarClone);
        }
        //else total > pWeight, which is a bust so we skip it
    }
}

long CalculateQuantumEntanglement (List<int> pWeights)
{
    long result = 1;
    foreach (int weight in pWeights) result *= weight;
    return result;
}