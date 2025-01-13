//Solution for https://adventofcode.com/2015/day/24 (Ctrl+Click in VS to follow link)

//Input: list of package weights

string myInput = "1\r\n3\r\n5\r\n11\r\n13\r\n17\r\n19\r\n23\r\n29\r\n31\r\n37\r\n41\r\n43\r\n47\r\n53\r\n59\r\n67\r\n71\r\n73\r\n79\r\n83\r\n89\r\n97\r\n101\r\n103\r\n107\r\n109\r\n113";
//string myInput = "11\r\n10\r\n9\r\n8\r\n7\r\n5\r\n4\r\n3\r\n2\r\n1";

//Task: create 3 different groups of packages of equal weights, with group 1 having the least amount of packages

//First get all the numbers and make sure they are sorted to speed up the algorithm
List<int> packages = myInput.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
packages.Sort((a,b) => b-a);

Console.WriteLine("Part 1: Quantum entanglement of the ideal config:" + CalculateIdealQuantumEntanglement(packages, 3));
Console.WriteLine("Part 2: Quantum entanglement of the ideal config:" + CalculateIdealQuantumEntanglement(packages, 4));

Console.ReadKey();

long CalculateIdealQuantumEntanglement(List<int> pPackages, int pNumberOfGroups)
{
    //Getting all permutations and testing those takes too much time, so we use a smarter approach
    List<List<int>> possibleConfigurations = new List<List<int>>();
    GetPossibleConfigurationsOfWeightX(pPackages, pPackages.Sum() / pNumberOfGroups, possibleConfigurations);

    //Find the minimum package length
    int minPackageLength = possibleConfigurations.Min (a => a.Count);
    possibleConfigurations = possibleConfigurations.Where (a => a.Count == minPackageLength).ToList();

    //Find optimum quantum entanglement score ;)
    long optimumQuantumScore = possibleConfigurations.Min(a => CalculateQuantumEntanglement(a));

	return optimumQuantumScore;
}

//also see the worked_example.txt
//This method expects a descending sorted list of package weights, so it can go through all the provided
//packages in an order fashion in order to try and establish the requested weight.

void GetPossibleConfigurationsOfWeightX (List<int> pPackages, int pWeight, List<List<int>> pFoundConfigurations, List<int> pConfigurationSoFar = null)
{
    pConfigurationSoFar = pConfigurationSoFar ?? new List<int>();

    //See if we can even make the requested configuration by combining everything so far, 
    //otherwise do an early exit
    int maxWeight = pPackages.Sum() + pConfigurationSoFar.Sum();
    if (maxWeight < pWeight) return;

    //Console.WriteLine(String.Join(" ", pConfigurationSoFar));
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