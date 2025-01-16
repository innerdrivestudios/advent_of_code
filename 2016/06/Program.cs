//Solution for https://adventofcode.com/2016/day/6 (Ctrl+Click in VS to follow link)

string myTestInput = "eedadn\r\ndrvtee\r\neandsr\r\nraavrd\r\natevrs\r\ntsrnev\r\nsdttsa\r\nrasrtv\r\nnssdts\r\nntnada\r\nsvetve\r\ntesnvt\r\nvntsnd\r\nvrdear\r\ndvrsen\r\nenarar";
string myInput = "drhqjkbv\r\ndtmukohn\r\npblnptvr\r\nrggqrqre\r\nihknljci\r\nncoeigib\r\nqbmbrgzv\r\nzfahdmvg\r\nmblaahfc\r\nirafzbak\r\nwghpbuwg\r\npyijzopp\r\ndxpqsjkd\r\nnhrosahg\r\nekfkwiqi\r\nkhgwbxsz\r\nfkkinvci\r\nxlmjkxql\r\nrvzsdcve\r\njosyoqlp\r\nwnyzmbhh\r\nbvkkwyxp\r\nzdnlhnlv\r\nhbgmztkb\r\nyugdjalz\r\nittbvkxm\r\npbnxevdt\r\nfptnvjdb\r\ndsypeozk\r\ngcjmtlcc\r\npqsnuztm\r\nmeilciol\r\nuolptcbc\r\njueofzif\r\nmryrumjt\r\njxmdacbz\r\nplraftrm\r\nljgrwfiz\r\nligncqig\r\nhrcmfkwa\r\nkraqruef\r\nmawxovke\r\nfgwermvq\r\nipfzrpoz\r\nrhfgebjy\r\nltqkdjag\r\ndvgdhywz\r\nojnzjack\r\nktnqjzlc\r\nuvcyfogs\r\nanqjxrbb\r\nrqedwxhi\r\niliuzgff\r\nlanioopk\r\nkbcwlndi\r\nwuzfguwq\r\noclgdxdn\r\neuqmnjxb\r\nrwgdgwac\r\ngepgmuuw\r\nxmsfhugr\r\ncmmgvoza\r\nmrdnpirq\r\nmuorgsxa\r\nawdmmpth\r\nabwkhkot\r\nhlztzuhm\r\nqwbxfktf\r\ngkdslbhk\r\nrqppdpfj\r\njuuuxmwh\r\nsprwgdoi\r\neptrqcap\r\nfdtoubid\r\nlnkxrjrr\r\nyxxiuuzb\r\nkcbgcuno\r\nssszanxv\r\nxajngqze\r\ndqprwcbs\r\ntmehzhsh\r\neeooqhko\r\neacbmenw\r\nqpbwiznc\r\nhoofmvah\r\nnoicaaqv\r\nreqwbpnl\r\nctfbzydf\r\nkkjvsvob\r\npwkramtw\r\ntxsebrtx\r\nwpxomtjx\r\nxwcxfihp\r\niabaswyj\r\nubvgfrsw\r\nyhxvrlwg\r\nkyighkhi\r\nkfmcjeci\r\nwtxvnufi\r\neggyqgwj\r\nazywqbdk\r\negwprvgp\r\ngxtgzqek\r\nhbypquhd\r\noqbpykib\r\nbsaybmgo\r\nctzhjzgf\r\nuqcatiea\r\nfkquufln\r\nmzuepzcn\r\nrwteqddf\r\nuandcifd\r\nchqjvavf\r\nxrugbecb\r\nbynqmlhg\r\nkeruzmmy\r\ntpxbrwkw\r\nczawkslt\r\nduunzdgl\r\nmhhrgsfi\r\nlhiorqvn\r\nuroxbpki\r\nuyqvmdvr\r\nwbvzxinc\r\nhvjbywqh\r\nopztxiye\r\njnkvbsss\r\nawfhhjuh\r\nwldwnsms\r\nvzdtdfzz\r\nqcgjdsxf\r\nncungtbd\r\nbsfmblxe\r\niztafyde\r\nomxfacnb\r\nbfjgzohh\r\nidowhvnt\r\netxcropc\r\ndlebejbz\r\ndpgrvvyg\r\nyyetaiyd\r\nzmvvslxw\r\niolevdzw\r\npwrbpwyt\r\nncxnbdcs\r\nbizuessl\r\ncsomkqnr\r\ngukypdxo\r\nzzazkgze\r\nmhygipbc\r\nnidnvzql\r\noblusiue\r\nrnuvqhpy\r\nybeycdic\r\nsbzmvzxq\r\ngvvqcrug\r\nzkdknqod\r\nzkztprqv\r\nxlprotcp\r\nsndzhrmt\r\ncntdipcw\r\nsiiqfres\r\npnfcbzjo\r\ntwbgdnrv\r\nzzxnngor\r\nnmcmxqgt\r\njayxwvrm\r\npahclzsw\r\nztddtnyo\r\nodidwauv\r\ntzlaantg\r\nnttefszm\r\njahhbgpt\r\npvxsgjsl\r\ngewlnlqm\r\nbykgsqzo\r\nzqrrhrkl\r\nisxyqlbv\r\nwfzkqgvt\r\nmcwizlzj\r\nugtgyrez\r\nqfyzoall\r\ncfyqxoyx\r\nvdszizjm\r\nbgrcpovm\r\neinnyfdv\r\nuugncaps\r\nletylmon\r\nocltwlem\r\nowfskawk\r\nhvripclk\r\njmgrulzy\r\nkjxyhhle\r\nionkwbuu\r\ntmjsudwf\r\nkxocxtoa\r\nejbuoost\r\nnzqdzsdd\r\ndkuiiisp\r\ndxwhxbvj\r\nghipctmd\r\nmwmcfifl\r\nvfptvuxo\r\nzfqsjsbx\r\npeazobjt\r\nnlwomnpb\r\nndxbvgqn\r\nzonnhffl\r\nvjnyiejx\r\nghrcsxgl\r\nfsdiwdmy\r\njmhtpgis\r\nsscjmpev\r\noivfuctm\r\nzwtllrfo\r\nuuzpmnjx\r\nwgpccyiu\r\nujhilevq\r\nhjjieaag\r\ndfbfylpe\r\nypegnmyg\r\nvjnjrgcu\r\nenylcycn\r\nptlneeqo\r\nnfiplyjo\r\ndxcrmicb\r\nvdxohgvl\r\niycioixk\r\nrwqrjyyu\r\nrfatusnv\r\nmnyvdlhu\r\npiyhcljq\r\nedcykiom\r\nwrtwvqub\r\nejtepubt\r\nvegruhiu\r\nqmrlbprr\r\nwntjsebz\r\nhkdkuasw\r\njxkfblck\r\nknrwttyx\r\npmjitnry\r\nlrtbermt\r\nqkhtmeoe\r\nskmqdpek\r\nfmxlqqhn\r\nureaitwq\r\nqufcyfph\r\nwcidvwgt\r\nybkrmqem\r\nqhuacrls\r\nyvnsqqdb\r\nxpjchodx\r\nlcwzkemd\r\njokzbvsi\r\njjbklvqq\r\nxfswceep\r\nxsjwsymu\r\nslmsyksa\r\noaquyavd\r\nzxteczie\r\nyglonpuu\r\nrdfvsbno\r\nwgxhuxga\r\nkvpetmdr\r\nfjeoobow\r\natdqjabp\r\nprjazwst\r\nyqkisdog\r\nwvswvdtm\r\niogvloma\r\nsrxeqnqq\r\nipjfezkx\r\njoonxwtz\r\nyjovntqa\r\nsvbbkcvw\r\nmgyseuqr\r\ndxflpkvp\r\ngqbiytmk\r\nsiccxtsn\r\noepstegr\r\nuwjjhhqb\r\neosfjfhv\r\ndsaqqhda\r\nvudzwxak\r\nnjzjiowv\r\nanfpqwsz\r\ngeuqffcr\r\nvhhbkgeb\r\nflkqpzbn\r\nfgtdspvd\r\nyjhxwcps\r\naimfdnpv\r\nhhvklxlo\r\nyjoxsxhj\r\nbllophbc\r\nntclfhgs\r\ngtnsuqdp\r\ndazhoeap\r\nklczitkw\r\ntlkedeuy\r\ncvbuidmk\r\nvjubfgqg\r\nqimvfpxg\r\nhnqegigv\r\ncppyezxe\r\nczcmiytj\r\nypvezoca\r\nadjxiooc\r\nmdshbmjd\r\nurthwyqf\r\ndhoijcrh\r\nvxnnyszn\r\nttzkydfs\r\nlhnbywji\r\ntyiuyhxa\r\nfyryagxi\r\nhboupxaq\r\nurctvbue\r\ncirtbbfu\r\nbkoxlmkm\r\nrdeoosjs\r\nqemhixen\r\nzqfioppk\r\nvopwlhhe\r\ngmpihxop\r\naamsrrzs\r\nsdyssprk\r\nhmrqkghm\r\noevtvzwl\r\nbqufyyuu\r\nennrxvaf\r\nslmshjpz\r\nqgraeety\r\nmczjxfan\r\nwzwmupvu\r\nsqbkhwxg\r\nabbotwty\r\ntrafaoli\r\nbkuarvfz\r\nwuffaong\r\nfqevpper\r\neekwoblz\r\nspsztgee\r\nyjfbfeif\r\nqcjdtsez\r\nomujkwzt\r\nvjpfndxp\r\nhkpxvjix\r\neiigrazh\r\njmtdqwuu\r\njnsfvufm\r\nxhkdzgjf\r\nrvqigugc\r\nbivqnzgu\r\nydadmvyq\r\nghsohaqa\r\neulugttl\r\nnvhaafrh\r\nikdtvxpu\r\nfbmztykr\r\ngmhluyfq\r\nbiovnlho\r\nxinmgiwl\r\nxsvlnvnr\r\ngryuussb\r\neeqmavbo\r\nfuftdkgb\r\nwwmwybtx\r\ntxshabuj\r\newxcrjmj\r\nlskdajks\r\npabjhzen\r\nxzfmdhaj\r\ngojxghyk\r\niyqaryra\r\nbnlovokf\r\nmehlaadw\r\ntzqhtnhv\r\nkgacrpdt\r\nqxawodku\r\nfcrouumv\r\nyzqxkmgi\r\nxzspfhmp\r\nomhlnexu\r\nolstosyp\r\ngxslgwcn\r\njuamcglq\r\nirdvybpr\r\nncsacfpd\r\nhczoulhg\r\nkqjpowtq\r\nbzefqjnj\r\nqnrtwygz\r\nrztxjfyr\r\nlcpxloro\r\ngdibhhkc\r\nqgwuyhea\r\nuweyjukp\r\nhsljwmyy\r\nayhxycnx\r\nklzvtttr\r\nlgdmpcww\r\ncqvtjkyw\r\nrfyjuybh\r\ntskdjzrt\r\nmhwspcvf\r\nxplpnemj\r\nlrjvgjgs\r\ncrllldzy\r\nikczhybc\r\noolwtfoj\r\npqfgligu\r\ncdgktmmx\r\njbpatjkl\r\nkeqepeax\r\nqfirnsdi\r\njuyzjarn\r\ntuymbnri\r\njijsmffx\r\nxfnbxvzo\r\noxmscdkf\r\nhkfcgeuv\r\nmkkuxffi\r\nvjmccxrb\r\nwrpagtbd\r\nwcnmlred\r\npafkwcph\r\ntcvjkxyl\r\nstvhdkom\r\ncfzhzuif\r\navdzxwyd\r\nwdoyqfpy\r\ngtolniag\r\nnvznsjsi\r\ncabyykqf\r\nbxfoznta\r\nmobkmnwc\r\nsauywroh\r\nxexkiuyy\r\noyevexto\r\nvqegjclw\r\nxoifoyic\r\nrshyscwd\r\nhvreaslo\r\nnegmieaw\r\ndmfggrux\r\nyxmknfpx\r\nttyuarnj\r\ntiwewqnv\r\nfgmplqux\r\nfsdeavfu\r\nuvqczvae\r\ndydssjnh\r\nihlykqnj\r\nxygdlwae\r\nusyabtdd\r\nyqikfwvv\r\nnvqrjqzh\r\nrexbjqsu\r\nnzerccmq\r\napzjtxxq\r\ndgnykfrj\r\nizahycnb\r\nruubddeh\r\nxkrbsmak\r\neooaoaxz\r\nalwnxxey\r\nrpxrikkc\r\nsyafkuqf\r\nfqwavgfy\r\njlrielnd\r\nylmuftje\r\njigqlhyu\r\nfdbusljj\r\nltsvqjss\r\nryzsnxja\r\nsdvtoolp\r\nttvfgkan\r\nzzbzmsdh\r\noneyivvm\r\nlmkooris\r\nqbwaumyc\r\nupooekoi\r\noaplwhwf\r\nzvvsffan\r\nhjujkmzt\r\nksocowvu\r\nxmaxctph\r\nuohgaowz\r\nwyiqttrj\r\napdmkleo\r\nngirmkie\r\nzdmekxlr\r\nzghwmffq\r\nkkwtxmht\r\naopxxxla\r\ncwaxcndp\r\nmchejzuf\r\nqiisajma\r\npaqlvgwk\r\nneepulbv\r\nlocpxcge\r\nzqihibab\r\nmpwwkcnc\r\nacltdkmx\r\ntsveirfy\r\noyqnektx\r\nroljqjyy\r\ntppfxdlm\r\npdkdgzbj\r\nhqohdklw\r\nffaqesqq\r\ncluhwwfj\r\nbfwsfqjl\r\nyxeforet\r\nauhwgphh\r\nwqeumebp\r\nstagigac\r\ntypntncz\r\nhdlxeizg\r\ndaaredsw\r\ntbgzwdkl\r\njnncvszy\r\nvzzojziz\r\nuhuiphch\r\nsflgjnmx\r\nzjcebfsa\r\npsvusroj\r\nisyddacs\r\nqjnifltk\r\nlxsjtnwd\r\ngfrahlus\r\nvvzirpuo\r\nzxscyfrr\r\nasdpeiwa\r\nhzrjpvlj\r\nlubqxhin\r\nkauqzmng\r\nvlchbkuk\r\nbaivdnom\r\ncuwbwhml\r\ninwmiyfq\r\nywvdnsqq\r\nsohppxig\r\nvnvqqmjy\r\naaekdkci\r\nbyyylakp\r\nvwctbbrq\r\npguveyxu\r\nccbkkuqw\r\nqmtotaum\r\nbbghdbpn\r\nmiuprwir\r\nlkjlnmus\r\nzhvpqpwz\r\nxtxzqnix\r\nlxckscjy\r\nlkvlzrta\r\nfeoujqdq\r\nmyecxjgd\r\ndwkmzbor\r\ngfdlcijs\r\nabwlgpdc\r\nvwspenjl\r\ncigbugvr\r\nugjalecy\r\nklbkpbsg\r\nqraxrapw\r\nvyjcmvmy\r\nffhvruro\r\neluiytkn\r\n";

Console.WriteLine("Decoded test input:"+DecodeString(myTestInput, true));
Console.WriteLine("Part 1:"+DecodeString(myInput, true));
Console.WriteLine("Part 2:"+DecodeString(myInput, false));

Console.ReadKey();

string DecodeString(string pInput, bool pMax)
{
	//Get all separate garbage strings
	string[] garbage = pInput.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

	//assume every string has the same length, so get length based on the first string
	int characterCount = garbage[0].Length;

	string decodedString = "";

	//now for every "column"
	for (int character = 0; character < characterCount; character++)
	{
		//collect a count of each char		
		Dictionary<char, int> charCount = new();

		for (int word = 0; word < garbage.Length; word++)
		{
			char c = garbage[word][character];
			charCount.TryGetValue(c, out int count);
			charCount[c] = count + 1;
		}

		//and based on the max param find the max or min occurring char
		int maxOrMin = pMax ? charCount.Values.Max() : charCount.Values.Min();
		//from the dictionary select the key of the first item where the value matches our maxMin int
		decodedString += charCount.Where (x => x.Value == maxOrMin).First().Key;
	}

	return decodedString;
}

void Part2(string pInput)
{
}
