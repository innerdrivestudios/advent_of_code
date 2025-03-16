// Solution for https://adventofcode.com/2020/day/4 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

// In visual studio you can modify which file by going to Debug/Debug Properties
// and setting "$(SolutionDir)input.txt" as the command line, this will be passed to args[0]

// ** Your input: a batch of password data

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);	

// First process the password data to a collection of key value pairs:
List<Dictionary<string, string>> passwordData =	myInput
	.Split(Environment.NewLine+ Environment.NewLine)							//Get the batches of data
	.Select (x => x.ReplaceLineEndings(" "))									//Combine a batch of strings into one string
	.Select (					
		x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries)				//Get the separate key:value from the string
		.Select (x => x.Split(":", StringSplitOptions.RemoveEmptyEntries))		//Split those parts on :
		.ToDictionary(x => x[0], x => x[1])										//And store them as key -> value pairs in a dictionary
	)
	.ToList();

// What we have at this point is a list of dictionaries, one per passport

// ** Part1: Check if they are valid:
// - valid passports have 8 keys or 7 where the missing key is the CID

int validPassportCount = 0;

foreach (var passport in passwordData)
{
	// passports are valid if they have more than 7 keys
	// or 7 keys and the one missing is cid
	if (
		passport.Keys.Count == 8 ||
		(passport.Keys.Count == 7 && !passport.ContainsKey("cid"))
	)
	{
		validPassportCount++;
	}
}

Console.WriteLine("Part 1 - Valid passport count: " + validPassportCount);

// Part 2 - Same thing, but now there are much more restrictions on the data

validPassportCount = 0;

foreach (var passport in passwordData)
{
	// passports are valid if they have more than 7 keys
	// or 7 keys and the one missing is cid
	// AND their data field are valid
	if (
		(
			passport.Keys.Count == 8 ||
			(passport.Keys.Count == 7 && !passport.ContainsKey("cid"))
		)
		&&
		HasValidData (passport)
	)
	{
		validPassportCount++;
	}
}

Console.WriteLine("Part 2 - Valid passport count: " + validPassportCount);

// All helper methods are down here:

//	byr(Birth Year) - four digits; at least 1920 and at most 2002.
//  iyr (Issue Year) - four digits; at least 2010 and at most 2020.
//  eyr (Expiration Year) - four digits; at least 2020 and at most 2030.
//  hgt (Height) - a number followed by either cm or in:
//      If cm, the number must be at least 150 and at most 193.
//      If in, the number must be at least 59 and at most 76.
//  hcl (Hair Color) - a # followed by exactly six characters 0-9 or a-f.
//  ecl (Eye Color) - exactly one of: amb blu brn gry grn hzl oth.
//  pid (Passport ID) - a nine-digit number, including leading zeroes.
//  cid (Country ID) - ignored, missing or not.

// e.g. byr:2030 hcl:#fffffd ecl:#a4a596 hgt:168cm
//		iyr:1936 eyr:2020 cid:296 pid:168786676

bool HasValidData (Dictionary<string, string> pPassport)
{
	return
		IsValidYear(pPassport["byr"], 1920, 2002) &&
		IsValidYear(pPassport["iyr"], 2010, 2020) &&
		IsValidYear(pPassport["eyr"], 2020, 2030) &&
		IsValidHeight(pPassport["hgt"]) &&
		IsValidHairColor(pPassport["hcl"]) &&
		IsValidEyeColor(pPassport["ecl"]) &&
		IsValidPassportID(pPassport["pid"]);
}

//	byr(Birth Year) - four digits; at least 1920 and at most 2002.
bool IsValidYear(string pYear, int pLower, int pUpper)
{
	if (int.TryParse (pYear, out int year)) {
		return year >= pLower && year <= pUpper;
	}
	else
	{
		return false;
	}
}

//  hgt (Height) - a number followed by either cm or in:
//      If cm, the number must be at least 150 and at most 193.
//      If in, the number must be at least 59 and at most 76.
bool IsValidHeight(string pHeight)
{
	//hgt: 168cm
	if (pHeight.EndsWith("cm"))
	{
		if(int.TryParse(pHeight.Substring(0, pHeight.IndexOf("cm")), out int height))
		{
			return height >= 150 && height <= 193;
		}
		else
		{
			return false;
		}
	}
	else if (pHeight.EndsWith("in"))
	{
		if (int.TryParse(pHeight.Substring(0, pHeight.IndexOf("in")), out int height))
		{
			return height >= 59 && height <= 76;
		}
		else
		{
			return false;
		}
	}

	return false;
}

//  hcl (Hair Color) - a # followed by exactly six characters 0-9 or a-f.
bool IsValidHairColor(string pHairColor)
{
	//^match start, $ match end, # followed by 6 digits or a-f in between
	Regex hairParser = new Regex(@"^#[0-9a-f]{6}$");
	return hairParser.IsMatch(pHairColor);
}

bool IsValidEyeColor(string pEyeColor)
{
	HashSet<string> validEyeColor = ["amb", "blu", "brn", "gry", "grn", "hzl", "oth"];
	return validEyeColor.Contains(pEyeColor);
}

//  pid (Passport ID) - a nine-digit number, including leading zeroes.

bool IsValidPassportID(string pPassportID)
{
	return int.TryParse(pPassportID, out _) && pPassportID.Length == 9;
}