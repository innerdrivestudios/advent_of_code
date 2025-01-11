//Solution for https://adventofcode.com/2015/day/19 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

//Your input: a bunch of grammar rules and a sentence to create with this grammar (all the way at the end)
string myInput = "Al => ThF\r\nAl => ThRnFAr\r\nB => BCa\r\nB => TiB\r\nB => TiRnFAr\r\nCa => CaCa\r\nCa => PB\r\nCa => PRnFAr\r\nCa => SiRnFYFAr\r\nCa => SiRnMgAr\r\nCa => SiTh\r\nF => CaF\r\nF => PMg\r\nF => SiAl\r\nH => CRnAlAr\r\nH => CRnFYFYFAr\r\nH => CRnFYMgAr\r\nH => CRnMgYFAr\r\nH => HCa\r\nH => NRnFYFAr\r\nH => NRnMgAr\r\nH => NTh\r\nH => OB\r\nH => ORnFAr\r\nMg => BF\r\nMg => TiMg\r\nN => CRnFAr\r\nN => HSi\r\nO => CRnFYFAr\r\nO => CRnMgAr\r\nO => HP\r\nO => NRnFAr\r\nO => OTi\r\nP => CaP\r\nP => PTi\r\nP => SiRnFAr\r\nSi => CaSi\r\nTh => ThCa\r\nTi => BP\r\nTi => TiTi\r\ne => HF\r\ne => NAl\r\ne => OMg\r\n\r\nCRnCaSiRnBSiRnFArTiBPTiTiBFArPBCaSiThSiRnTiBPBPMgArCaSiRnTiMgArCaSiThCaSiRnFArRnSiRnFArTiTiBFArCaCaSiRnSiThCaCaSiRnMgArFYSiRnFYCaFArSiThCaSiThPBPTiMgArCaPRnSiAlArPBCaCaSiRnFYSiThCaRnFArArCaCaSiRnPBSiRnFArMgYCaCaCaCaSiThCaCaSiAlArCaCaSiRnPBSiAlArBCaCaCaCaSiThCaPBSiThPBPBCaSiRnFYFArSiThCaSiRnFArBCaCaSiRnFYFArSiThCaPBSiThCaSiRnPMgArRnFArPTiBCaPRnFArCaCaCaCaSiRnCaCaSiRnFYFArFArBCaSiThFArThSiThSiRnTiRnPMgArFArCaSiThCaPBCaSiRnBFArCaCaPRnCaCaPMgArSiRnFYFArCaSiThRnPBPMgAr\r\n";

//Your task: derive some subrules and check if the sentence can be created from the grammar

//Step 1.Process the input into the sentence and the grammar rules:

//- Split the rules from the sentence
string[] myInputParts = myInput.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries);

//- Extract all the rules
List<(string key, string value)> rules =
	myInputParts[0].
	Split("\r\n", StringSplitOptions.RemoveEmptyEntries).
	Select(x => x.Split(" => ", StringSplitOptions.RemoveEmptyEntries)).
	Select(x => (x[0], x[1])).
	ToList(); 

//- And extract the sentence
string sentence = myInputParts[1];

Console.WriteLine(
	"Part 1 - Distinct molecule count after 1 replacement:"+
	DistinctMoleculeCountAfterOneReplacement (sentence, rules)
);

//Part 2 still in progress :)


Console.ReadKey();

int DistinctMoleculeCountAfterOneReplacement(string pInput, List<(string, string)> pRules)
{
	HashSet<string> uniqueResults = new HashSet<string>();

	foreach (var rule in pRules)
	{
		ApplyRule(rule, pInput, uniqueResults);
	}

	return uniqueResults.Count;
}

void ApplyRule((string key, string value) pRule, string pInput, HashSet<string> pResults)
{
	Regex replacer = new Regex(pRule.key);
	MatchCollection matches = replacer.Matches(pInput);

	foreach (Match match in matches)
	{
		//apply rule to all matches one by one and not in one go otherwise we'll get the wrong results :)
		string resultString = replacer.Replace(pInput, pRule.value, 1, match.Index);
		pResults.Add(resultString);
	}
}



/*
void Part2(string pInput)
{
	int currentIndex = 0;

	int stack = 0;
	int stackCount = 0;

	while (currentIndex < pInput.Length) {
		int RnIndex = pInput.IndexOf("Rn", currentIndex);
		int ArIndex = pInput.IndexOf("Ar", currentIndex);

		if (RnIndex > -1 && RnIndex < ArIndex)
		{
			stack++;
			Console.WriteLine("Stack+:"+stack + " at " + RnIndex);
		}
		if (ArIndex > -1 && ArIndex < RnIndex)
		{
			stack--;
			Console.WriteLine("Stack-:" + stack + " at " + ArIndex);
		}

		if (stack < 0)
		{
            Console.WriteLine("Error");
        }

		if (stack == 0)
		{
			stackCount++;
            Console.WriteLine("STACKCOUNT:"+stackCount);
        }

		if (RnIndex == -1 && ArIndex == -1) break;
		else if (RnIndex == -1) currentIndex = ArIndex+1;
		else if (ArIndex == -1) currentIndex = RnIndex+1;
		else currentIndex = Math.Min(ArIndex, RnIndex)+1;


    }
}
*/
