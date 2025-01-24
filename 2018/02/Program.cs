// Solution for https://adventofcode.com/2018/day/2 (Ctrl+Click in VS to follow link)

// Your input: a bunch of weird strings

string myInput = "mvgowxqubnhaefjslkjlrptzyi\r\npvgowlqubnhaefmslkjdrpteyi\r\novgowoqubnhaefmslkjnrptzyi\r\ncvgowxqubnrxefmslkjdrptzyo\r\ncvgowxqubnhaefmsokjdrprzyf\r\ncvgowxqubnhjeflslkjgrptzyi\r\ncvgowxqvbnhaefmslkhdrotzyi\r\nhvgowxqmbnharfmslkjdrptzyi\r\ncvgoaxqubqhaefmslkjdrutzyi\r\ncvxowxqdbnhaefmslkjdgptzyi\r\ncvgikxqubnhaefmslkjdrptzyz\r\ncvgnwxqubnhaqfjslkjdrptzyi\r\ncqgowxqubnhaecmslkjgrptzyi\r\ncvpowxqucnhaefmslkjdrptzyz\r\nfvuoexqubnhaefmslkjdrptzyi\r\nsvgowxqubnhaefmsvkjdrttzyi\r\ncvgowxqubnhaefmblkjdfpbzyi\r\ncvkoyxqubnhaefsslkjdrptzyi\r\nbvgowxqublhaefmslkjdrptzfi\r\nxvgewxqubnhaefmslkjdrztzyi\r\ncvgowxqubzhaefmslkkrrptzyi\r\ncvgowxqubnhaefmslkudruuzyi\r\ncvgowxqubnhaefmvlkjdrptwyl\r\ncvgoyxqubnhaefmslkjvrotzyi\r\ncvgowxoubnhaewmslkjdrpbzyi\r\ncvgowxgubnhaefmslijdrptzxi\r\nlvgowxqkbnhaefmslkjdrptzqi\r\nxvgowxqubyhaefmflkjdrptzyi\r\nwvnowxgubnhaefmslkjdrptzyi\r\ncvgowxguwnhaefhslkjdrptzyi\r\ncvgowfquxnhaefmdlkjdrptzyi\r\ncvgywxqubnuaefmsldjdrpfzyi\r\ncvkowxqzbrhaefmslkjdrptzyi\r\ncviowxzubnhaefmslkjdrptqyi\r\ncvgowxqubnhaefmsozjdrptzyc\r\ncvglwxuubnhaewmslkjdrptzyi\r\ncvgowxquknhaebmsfkjdrptzyi\r\nvvgowxqubnhaesmslkjdrptzri\r\ncvgowxoubndaefmslkjdrftzyi\r\ncvgowxqubghaefmslkjdeptzyw\r\ncvgowxqubnhaetmhlkjdrpvzyi\r\ncvgowmquunhaefmslkjdrptzyt\r\ncvgooxqpbniaefmslkjdrptzyi\r\ncvgowxqubnhaeumslkjdkptiyi\r\ncvgrwxqsbnhaemmslkjdrptzyi\r\ncvrowxqubnhaefmslkjdrctcyi\r\ndvgcwxqubnhaefmslkjdrptzyq\r\ncugowxqubnhasfmmlkjdrptzyi\r\ncwgowxqobzhaefmslkjdrptzyi\r\ncvgowxquwnhaefmulkjdrptbyi\r\nnvgowxqmbnhaefmslyjdrptzyi\r\ncvgowxqubniakvmslkjdrptzyi\r\ncvyowxqubnhaefmslejdrptzyx\r\ncvgobxqubghaefeslkjdrptzyi\r\ncvgowxiubnhaebmslkjdfptzyi\r\ncvgosbqubnhaefmslkvdrptzyi\r\ncvgpwxqubnhaefvslkjdrptzyh\r\ncvgowxqubnyaefmslgjdsptzyi\r\ncvgowxqubnhaefmslkjdrprzzp\r\ncvgowxqubwhaemmslkjdrpazyi\r\ncvgowxqpbnhaemmslkjdrpczyi\r\ncvgoqxqubnhaelmslkjdrptzye\r\ncvgowxqubnhaefmslbjdrttzvi\r\ncvgowxqubnhlefmslkvurptzyi\r\ncvgowxqujngaefmslktdrptzyi\r\ncvgowxqubnhaefmsckjdcwtzyi\r\ncvcowxqubnhaetmslkjorptzyi\r\njvnowxqubnhaefmslkjdrptzyf\r\ncygowxqkbnhaefmslejdrptzyi\r\ncvmowxqubnhaefmslkjdritzoi\r\ncvgowxqubnpaefmslkjdrpnnyi\r\ncvgowxqubnhaefmolkjdrpnzyy\r\nuvgowxoubnhaefmslkjdrptzvi\r\ncvgowxbabehaefmslkjdrptzyi\r\ncvgokxqubnhaefmsckjdrjtzyi\r\ncvgoxwqubahaefmslkjdrptzyi\r\ncvgowxqusnhaefmslijdrptyyi\r\ncvgowxqubmhaeqmslkxdrptzyi\r\ncvgouxhubnhaefmslkjdrjtzyi\r\ncvgowxqubnhaefmslrjdqptzyk\r\ncvgowxiublhaefsslkjdrptzyi\r\ncvgowxqubnxgefmslkadrptzyi\r\novgowxqugshaefmslkjdrptzyi\r\ncvgowxquznhaeemslsjdrptzyi\r\ncvkowxqubnhaeomslkjdeptzyi\r\ncvgvwxqubxhaefmslkjdrptzyu\r\ncvglwxqybnhaefmslkjdrptzyb\r\ncvgowxqubnlfwfmslkjdrptzyi\r\ncvaowxqubnhaefmslkjdrvtzbi\r\ncvgowxqubnrmefaslkjdrptzyi\r\ncvgowxqubnhaefmsnkjdfpwzyi\r\ncvgawxqmbnhaefmsykjdrptzyi\r\nchgowmqubnhaefmslkjdrptwyi\r\ncogowxqubnhaefmslkjxrptzri\r\ncvgohxqubnoaesmslkjdrptzyi\r\ncvdowxqubnhaofmslkjdrpvzyi\r\nvvgowrqubnhaefmslkjdrpthyi\r\ncvgowxquknhuefmslkjdoptzyi\r\ncvyowxeubnhaefmslhjdrptzyi\r\ncvglwxqubnhaefmslkjdrptdyq\r\ncvgowxqubnhaefmsikgdrptayi\r\ncvgowxqubnhaefjhlkjdrpczyi\r\ncvgzwxkubnhaefmslkjdjptzyi\r\ncxgowxqubnhaefmslkjdrptwyy\r\ncvgowxqubnhaefeslkjdrmqzyi\r\ncvgowxvubnhaefmilijdrptzyi\r\ncvgowxqzbthaeomslkjdrptzyi\r\ncvgowhqubndaefmglkjdrptzyi\r\ncvgowxvubnhaeamylkjdrptzyi\r\ncvgowiqubnhgefmslkjdrctzyi\r\ncvgowxqubchaefmslksdritzyi\r\ncvgowxqubnhaefmsnkjdreyzyi\r\ncvgowxqubihaefmslkgdrutzyi\r\ncvgowxqjbnhaeamslkjdrptzwi\r\ncvgowxzubnhaefmsxkjdrrtzyi\r\ncvgowxqubyhaetmslnjdrptzyi\r\ncvgowxquhnhaebmslkjdxptzyi\r\ncvgowxqubnhanfmslujdxptzyi\r\ncvgowxqublhnefaslkjdrptzyi\r\ncvgmwxqtbnhaefmslkjsrptzyi\r\njvgowxqubnhaeamslkjdrpmzyi\r\ncvgowxqubhiaefmsljjdrptzyi\r\nsvgowxqubnhaefmswkjdrpozyi\r\ncvgowxqebnhaeqmslkjdiptzyi\r\ncveowxqubnhayzmslkjdrptzyi\r\ncvglwxqubnhaefmxlkjdiptzyi\r\ncvgowkqubdhaefmszkjdrptzyi\r\ncvgowxkxbnhaeffslkjdrptzyi\r\ncugowxqubnnaefmslujdrptzyi\r\ncqgowxwubnhaepmslkjdrptzyi\r\ncvgowxqubnhayfmmlkjwrptzyi\r\ncvgowxquenhaefmsskxdrptzyi\r\ncvgowxqubnhiefmsrkjdtptzyi\r\nmvgowxkubnhaefmjlkjdrptzyi\r\ncvgowkquunhaefmglkjdrptzyi\r\ncvgowxqubqhaexmslgjdrptzyi\r\njvgowxqubnhaefmslkjddptlyi\r\ncvgiwxqubnhaefmslkjdpptmyi\r\nczgowxqubntaevmslkjdrptzyi\r\ncvgotmqubnhaefmslkjdrpazyi\r\ncvgowxtubnhaefmslkqdtptzyi\r\ncvbowxqhnnhaefmslkjdrptzyi\r\ncvgowkqubshaefmstkjdrptzyi\r\ncvgowqqrbnaaefmslkjdrptzyi\r\ncvgoixqubnhaefmslkjdrpmryi\r\ncvgoxxqubnhaeimsxkjdrptzyi\r\ncvgowxqubzhaebmslkjyrptzyi\r\ncjgewxqubnhaefsslkjdrptzyi\r\ncvgowxqdbnkaefmslwjdrptzyi\r\ncvgowxqzbnhaeamslkjdrftzyi\r\ncvgoixqubnsaewmslkjdrptzyi\r\ncvgswxqubnhaxfmslkjdrptzni\r\ncvwowxmubnhgefmslkjdrptzyi\r\ncvggwxqubnhaefmslqjdbptzyi\r\ncvgzwxqjbnhaefaslkjdrptzyi\r\ncvgowzqubnharfmspkjdrptzyi\r\ncvgowxqubnhawfmslkjdeptzyb\r\ncvuowequbnhaefmslkjdrntzyi\r\ngvgowxqubnxaefmslkjdrjtzyi\r\ncvgowxqubnhmetmsldjdrptzyi\r\ncvgowxqubnhamfmsqkjdrptyyi\r\ncvgoqxqubnhaefmslkjtrpazyi\r\ncvgoexqubhhaefmslkjdrhtzyi\r\ncvgowwqubnhaeflslkjdrptzyf\r\ncvgowlpubnhaefmslkjdrptvyi\r\ncvgowxouunhaebmslkjdrptzyi\r\ncvdowhqubnhaefmslijdrptzyi\r\ncvgowxqubnkatfmslkjdrhtzyi\r\ncvgowxqpbnhxeumslkjdrptzyi\r\ncvgowxqubnhaefmsukjjrptzyn\r\ncvgowxqubnhmefmslzjdrvtzyi\r\ncvtowxqubihaefmclkjdrptzyi\r\nchgowcqubnhayfmslkjdrptzyi\r\ncvguwxqubnhaefmblkjarptzyi\r\ncvgowoqubnhaefmsikjdrytzyi\r\ncvgkwxqubnhaefmslkjdrptchi\r\ncvhowxqubnhaefmslkjdrvlzyi\r\ncvlowxfubnhaefmslkjkrptzyi\r\ncvgowxqubhhaefoslkjdrytzyi\r\ncvgowxsubqhaefmslpjdrptzyi\r\ncvgowxpubnhaefmslhjdrptzyb\r\ncvgowxqubnhrefmjlkddrptzyi\r\ncvgowxqubnhaxfmykkjdrptzyi\r\nmvgowxqubnhakfmslkjdrptnyi\r\ncwgowxqubnhaffmslkadrptzyi\r\nchgowxquwnhaefmslsjdrptzyi\r\ncvgowxqubnhaefmslkjdwpnsyi\r\ncvgawxqubnhaefmslkldyptzyi\r\ncvgowxqubnhiefmslkjdiprzyi\r\ncvgkqxqubnhaefcslkjdrptzyi\r\ncvgovoqubnhaefmslkjdrpuzyi\r\ncvgowxqubnhaefmszkjdrjtzyk\r\ncvgopxqubnhaefmslkjdqpnzyi\r\ncvgtwxqubnhaefmslkjnrptzri\r\ncvgowxqurnhaedmslfjdrptzyi\r\ncvpowxqubnhaefmswkjdrltzyi\r\ncvgowxqujnpaefmslkjdrptdyi\r\ncvgowgqubnhzifmslkjdrptzyi\r\nlvgowxqubnhaenmslkjdbptzyi\r\nebgowxqubnhaeymslkjdrptzyi\r\ncvgowxtubqhaefmslkedrptzyi\r\ncvgowxqubshaesmslkjdrptryi\r\ncvgowxqubnhaefmflkjmrpkzyi\r\ncvgowxqubngaefmslkjdrytzgi\r\ncvgowxqubnhaefmslklhzptzyi\r\ncveowxqubnhgefmslkjdrpezyi\r\ncvgowxqubnhaeomslkjdrqtzym\r\ncvgowxqubzhaefmslwjdrptfyi\r\ncmgowxqubnhaefmsdkjdrptzui\r\ncvlowxqubnhaefmslsjdrptzwi\r\ncvhowxpubnhaefmslkjhrptzyi\r\ncveosxqurnhaefmslkjdrptzyi\r\ncvgowxqubnhaefgsdkjdrptjyi\r\ncvgvwxqubnhaefmslzjdmptzyi\r\ncviowxqubnhalfmslkjdrptzyr\r\ncvgowxqubchqefmslkjdrptzoi\r\ncvgownqubnhaefmsyktdrptzyi\r\ncvgywxqubnuaefmslkjdrpfzyi\r\ncvgobxqunnhaefmslkjdrptzbi\r\ncvgowxqubshaefgslkjdrxtzyi\r\ncvghwxqubnhaefmslkjdrbtmyi\r\ncvhowxqubnhaefmslkjdrpnzys\r\ncvgowxqubnmaefmslejdrptzyq\r\ncvmrwxqubnhaefmslkjdrpzzyi\r\ncvgowxqubshaefmslkfdrptzyu\r\ncvgowqqubnhaefmslkodrpjzyi\r\ncvgnwnquknhaefmslkjdrptzyi\r\ncvgowxquxnhacfmflkjdrptzyi\r\novgowxqubnhaefmslkjmrmtzyi\r\ncvgowxqubneaefmslkedrptzqi\r\ncvgowxqubphweflslkjdrptzyi\r\ncvgowxqudnhaefmplkjdrptdyi\r\ncvwowxbubnhaefmslkjurptzyi\r\ncvgowxtubnhaefmslkjdrwwzyi\r\ncvgowxqubnhkefmslajdrptzyn\r\ncvgowxqxbphaefmslkjdrptzsi\r\ncvgowxquenhaefmslmjwrptzyi\r\nzvgowdqubnhaeftslkjdrptzyi\r\ncsgowxqubnhgefmslkjdrptzyy\r\ncvgolxqubahaefmslkjdrpvzyi\r\ncvgoqxquhwhaefmslkjdrptzyi\r\ncvgawxqubghaefmsrkjdrptzyi\r\ncvgozxqubnhaefmslkwdfptzyi\r\ncvgowxqubnhaefmslhjdkptzzi\r\ncvnowxqubnhaefmsqkjdrptqyi\r\ncvpowxqubnhaefmslkpdrptdyi\r\ncvgowxoubnhaermslkjdrctzyi\r\ncvgowxqubnheefmslkjdrctzyr\r\ncvgowxqunnhaqfhslkjdrptzyi\r\ncvgowxqulnhaefmslrjdrntzyi\r\n";

string[] splitStrings = myInput
	.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

//Part 1
// - Count how many times a char occurs exactly 2 or 3 times in a string (count both things, but only once)
// - Multiply those occurances together

int twoTimesCount = 0;
int threeTimesCount = 0;

foreach (string s in splitStrings)
{
	//Count every element and check if any of them is exactly two or three
	if (s.Select(a => s.Count(b => a == b)).Any(c => c == 2)) twoTimesCount++;
	if (s.Select(a => s.Count(b => a == b)).Any(c => c == 3)) threeTimesCount++;
}

Console.WriteLine("Part 1 - Checksum: " + twoTimesCount * threeTimesCount);

// Part 2 
// - Find the strings that only differ by one character
// - Find the common characters in those two strings

// Iterate over all string pairs
for (int i = 0; i < splitStrings.Length-1; i++)
{
	for (int j = i + 1; j < splitStrings.Length; j++)
	{
		//count the difference, while building a difference string (lot of redundant work)
		int difference = 0;
		string a = splitStrings[i];
		string b = splitStrings[j];
		string equalChars = "";

		for (int k = 0; k < a.Length; k++)
		{
			int delta = a[k] == b[k] ? 0 : 1;
			if (delta == 0) equalChars += a[k];
			difference += delta;
		}

		if (difference == 1)
		{
            Console.WriteLine("Part 2 - Checksum: " + equalChars);
			return;
        }
    }
}