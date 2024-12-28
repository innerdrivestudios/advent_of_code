//Solution for https://adventofcode.com/2015/day/8 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

string myInput = "\"sjdivfriyaaqa\\xd2v\\\"k\\\"mpcu\\\"yyu\\\"en\"\r\n\"vcqc\"\r\n\"zbcwgmbpijcxu\\\"yins\\\"sfxn\"\r\n\"yumngprx\"\r\n\"bbdj\"\r\n\"czbggabkzo\\\"wsnw\\\"voklp\\\"s\"\r\n\"acwt\"\r\n\"aqttwnsohbzian\\\"evtllfxwkog\\\"cunzw\"\r\n\"ugvsgfv\"\r\n\"xlnillibxg\"\r\n\"kexh\\\"pmi\"\r\n\"syvugow\"\r\n\"m\\\"ktqnw\"\r\n\"yrbajyndte\\\\rm\"\r\n\"f\\\"kak\\x70sn\\xc4kjri\"\r\n\"yxthr\"\r\n\"alvumfsjni\\\"kohg\"\r\n\"trajs\\x5brom\\xf1yoijaumkem\\\"\\\"tahlzs\"\r\n\"\\\"oedr\\\"pwdbnnrc\"\r\n\"qsmzhnx\\\"\"\r\n\"\\\"msoytqimx\\\\tbklqz\"\r\n\"mjdfcgwdshrehgs\"\r\n\"\\\"rivyxahf\\\"\"\r\n\"ciagc\\x04bp\"\r\n\"xkfc\"\r\n\"xrgcripdu\\x4c\\xc4gszjhrvumvz\\\"mngbirb\"\r\n\"gvmae\\\"yiiujoqvr\\\"mkxmgbbut\\\"u\"\r\n\"ih\"\r\n\"ncrqlejehs\"\r\n\"mkno\\x43pcfdukmemycp\"\r\n\"uanzoqxkpsksbvdnkji\\\"feamp\"\r\n\"axoufpnbx\\\\ao\\x61pfj\\\"b\"\r\n\"dz\\\\ztawzdjy\"\r\n\"ihne\\\"enumvswypgf\"\r\n\"\\\"dgazthrphbshdo\\\\vuqoiy\\\"\"\r\n\"dlnmptzt\\\\zahwpylc\\\\b\\\"gmslrqysk\"\r\n\"mhxznyzcp\"\r\n\"rebr\\\"amvxw\\x5fmbnfpkkeghlntavj\"\r\n\"lades\\x47ncgdof\\\"\\\"jmbbk\"\r\n\"dwxuis\\xa5wdkx\\\\z\\\"admgnoddpgkt\\\\zs\"\r\n\"g\\\\k\\x27qsl\\x34hwfglcdxqbeclt\\xca\\\\\"\r\n\"lhyjky\\\\m\\\"pvnm\\\\xmynpxnlhndmahjl\"\r\n\"c\\\"uxabbgorrpprw\\\"xas\\\\vefkxioqpt\"\r\n\"rfrvjxpevcmma\\x71gtfipo\"\r\n\"fgh\\\"kcwoqwfnjgdlzfclprg\\\"q\"\r\n\"onxnwykrba\"\r\n\"hkkg\\x60f\\\"tjzsanpvarzgkfipl\"\r\n\"\\\"aintes\\\"ofq\\\"juiaqlqxmvpe\\\\a\"\r\n\"wiyczzs\\\"ciwk\"\r\n\"mfqeu\"\r\n\"v\\xe1z\\x7ftzalmvdmncfivrax\\\\rjwq\"\r\n\"k\\\"vtg\"\r\n\"exhrtdugeml\\xf0\"\r\n\"behnchkpld\"\r\n\"mhgxy\\\"mfcrg\\xc5gnp\\\"\\\"osqhj\"\r\n\"rlvjy\"\r\n\"awe\"\r\n\"ctwy\"\r\n\"vt\"\r\n\"\\x54t\"\r\n\"zugfmmfomz\"\r\n\"cv\\\"cvcvfaada\\x04fsuqjinbfh\\xa9cq\\xd2c\\\"d\"\r\n\"oj\"\r\n\"xazanf\\\"wbmcrn\"\r\n\"\\\\\\\\zkisyjpbzandqikqjqvee\"\r\n\"dpsnbzdwnxk\\\\v\"\r\n\"sj\\\"tuupr\\\\oyoh\"\r\n\"myvkgnw\\x81q\\xaaokt\\\\emgejbsyvxcl\\\\\\xee\"\r\n\"ejeuqvunjcirdkkpt\\\"nlns\"\r\n\"twmlvwxyvfyqqzu\"\r\n\"\\\"xwtzdp\\x98qkcis\\\"dm\\\\\\\"ep\\\"xyykq\"\r\n\"vvcq\\\\expok\"\r\n\"wgukjfanjgpdjb\"\r\n\"\\\"mjcjajnxy\\\\dcpc\"\r\n\"wdvgnecw\\\\ab\\x44klceduzgsvu\"\r\n\"dqtqkukr\\\"iacngufbqkdpxlwjjt\"\r\n\"\\\"xj\\\"\\x66qofsqzkoah\"\r\n\"nptiwwsqdep\"\r\n\"gsnlxql\\x30mjl\"\r\n\"yeezwokjwrhelny\\\"\"\r\n\"bjauamn\\\\izpmzqqasid\"\r\n\"tvjdbkn\\\"tiziw\\x82r\"\r\n\"w\"\r\n\"xwoakbbnjnypnaa\\xa9wft\\\"slrmoqkl\"\r\n\"vwxtnlvaaasyruykgygrvpiopzygf\\\"vq\"\r\n\"qdancvnvmhlmpj\\\\isdxs\"\r\n\"xzc\\\\elw\"\r\n\"b\\\"wxeqvy\\\"qf\\\"g\\xcaoklsucwicyw\\\"dovr\"\r\n\"yomlvvjdbngz\\\"rly\\\"afr\"\r\n\"bfb\\\"x\\\"aweuwbwmoa\\x13\\\"t\\\"zhr\"\r\n\"\\\"dmfoxb\\\"qvpjzzhykt\\xd2\\\"\\\"ryhxi\"\r\n\"psqef\\\"yu\\\\qiflie\\\"\\x79w\"\r\n\"arzewkej\\\"lqmh\\\\sayyusxxo\\\\\"\r\n\"vuvvp\"\r\n\"hc\\\"lg\\x6bcpupsewzklai\\\"l\"\r\n\"cjdfygc\\\"auorqybnuqghsh\\x10\"\r\n\"j\"\r\n\"wqjexk\\\"eyq\\\\lbroqhk\\\\dqzsqk\"\r\n\"dws\\\"ru\\\"dvxfiwapif\\\"oqwzmle\"\r\n\"agcykg\\\\jt\\\\vzklqjvknoe\"\r\n\"kksd\\\"jmslja\\\\z\\\"y\\\\b\\xaagpyojct\"\r\n\"nnpipxufvbfpoz\\\"jno\"\r\n\"dtw\"\r\n\"xlolvtahvgqkx\\\\dgnhj\\\\spsclpcxv\\\\\"\r\n\"mxea\\\\mbjpi\"\r\n\"lgbotkk\\\"zmxh\\\\\\\\qji\\\"jszulnjsxkqf\"\r\n\"lwckmhwhx\\\"gmftlb\\x91am\"\r\n\"xxdxqyxth\"\r\n\"\\\"lmqhwkjxmvayxy\"\r\n\"tf\"\r\n\"qy\"\r\n\"wdqmwxdztax\\\"m\\\"\\x09\\x11xdxmfwxmtqgwvf\"\r\n\"\\xcbnazlf\\\"ghziknszmsrahaf\"\r\n\"e\\x6aupmzhxlvwympgjjpdvo\\\"kylfa\"\r\n\"\\x81vhtlillb\\xactgoatva\"\r\n\"dvnlgr\"\r\n\"f\"\r\n\"xg\\xfacwizsadgeclm\"\r\n\"vnnrzbtw\\\"\\\\prod\\\\djbyppngwayy\\\"\"\r\n\"lrt\\xf4jahwvfz\"\r\n\"aqpnjtom\\\"ymkak\\\\dadfybqrso\\\\fwv\"\r\n\"gz\\\"aac\\\"mrbk\\\"ktommrojraqh\"\r\n\"wycamwoecsftepfnlcdkm\"\r\n\"nrhddblbuzlqsl\\x9cben\"\r\n\"vckxhyqkmqmdseazcykrbysm\"\r\n\"sil\\xbbtevmt\\\"gvrvybui\\\"faw\\\"j\"\r\n\"cjex\\\\tp\\x45pzf\"\r\n\"asjobvtxszfodgf\\\"ibftg\"\r\n\"gkyjyjdrxdcllnh\\\"sjcibenrdnxv\"\r\n\"oswsdpjyxpbwnqbcpl\\\"yrdvs\\\\zq\"\r\n\"\\\"\\\"tyowzc\\\\fycbp\\\"jbwrbvgui\"\r\n\"cbpcabqkdgzmpgcwjtrchxp\"\r\n\"iyrzfh\\x45gw\\\"fdlfpiaap\\x31xqq\"\r\n\"evgksznidz\"\r\n\"b\\\\w\\\\\"\r\n\"loufizbiy\\x57aim\\\"bgk\"\r\n\"qjfyk\"\r\n\"g\\\"anmloghvgr\\x07zwqougqhdz\"\r\n\"usbbmwcxd\\\\bdgg\"\r\n\"htitqcpczml\"\r\n\"eke\\\\cqvpexqqk\\\"to\\\"tqmljrpn\\xe6lji\\\"\"\r\n\"g\\xd2ifdsej\"\r\n\"h\\\"sk\\\"haajajpagtcqnzrfqn\\xe6btzo\"\r\n\"wfkuffdxlvm\\\\cvlyzlbyunclhmpp\"\r\n\"myaavh\\\"spue\"\r\n\"hqvez\\x68d\\\"eo\\\"eaioh\"\r\n\"s\\\"qd\\\"oyxxcglcdnuhk\"\r\n\"ilqvar\"\r\n\"srh\"\r\n\"puuifxrfmpc\\\"bvalwi\\x2blu\\\\\"\r\n\"yywlbutufzysbncw\\\\nqsfbhpz\\\"mngjq\"\r\n\"zbl\\\\jfcuop\"\r\n\"hjdouiragzvxsqkreup\\\\\"\r\n\"qi\"\r\n\"ckx\\\\funlj\\xa7ahi\"\r\n\"k\"\r\n\"ufrcnh\\\"ajteit\"\r\n\"cqv\\\"bgjozjj\\x60x\\xa8yhvmdvutchjotyuz\"\r\n\"hkuiet\\\"oku\\x8cfhumfpasl\"\r\n\"\\\"\\\\sbe\\x4d\"\r\n\"vhknazqt\"\r\n\"eyyizvzcahgflvmoowvs\\\\jhvygci\"\r\n\"kki\\x3ewcefkgtjap\\\"xtpxh\\\"lzepoqj\"\r\n\"wvtk\"\r\n\"\\\"ynet\"\r\n\"zh\\\\obk\\\"otagx\\x59txfzf\"\r\n\"ocowhxlx\\xe6zqg\\x63wx\\\\tclkhq\\\\vmaze\"\r\n\"w\\\"cf\"\r\n\"qpniprnrzrnvykghqnalr\"\r\n\"jctcqra\\\"\\x05dhlydpqamorqjsijt\\\\xjdgt\"\r\n\"sig\"\r\n\"qhlbidbflwxe\\\"xljbwls\\x20vht\"\r\n\"irmrebfla\\xefsg\\\"j\"\r\n\"nep\"\r\n\"hjuvsqlizeqobepf\"\r\n\"guzbcdp\\\"obyh\"\r\n\"\\\"mjagins\\xf9tqykaxy\\\"\"\r\n\"knvsdnmtr\\\"zervsb\"\r\n\"hzuy\"\r\n\"zza\\\"k\\\"buapb\\\\elm\\xfeya\"\r\n\"lrqar\\\"dfqwkaaqifig\\\"uixjsz\"\r\n\"\\\"azuo\\x40rmnlhhluwsbbdb\\x32pk\\\\yu\\\"pbcf\"\r\n\"dplkdyty\"\r\n\"rfoyciebwlwphcycmguc\"\r\n\"ivnmmiemhgytmlprq\\\\eh\"\r\n\"lhkyzaaothfdhmbpsqd\\\\yyw\"\r\n\"tnlzifupcjcaj\"\r\n\"\\\\qiyirsdrfpmu\\\\\\x15xusifaag\"\r\n\"\\\\lcomf\\\\s\"\r\n\"uramjivcirjhqcqcg\"\r\n\"kkbaklbxfxikffnuhtu\\xc6t\\\"d\"\r\n\"n\\xefai\"\r\n\"\\\"toy\\\"bnbpevuzoc\\\"muywq\\\"gz\\\"grbm\"\r\n\"\\\"muu\\\\wt\"\r\n\"\\\\srby\\\"ee\"\r\n\"erf\\\"gvw\\\"swfppf\"\r\n\"pbqcgtn\\\"iuianhcdazfvmidn\\\\nslhxdf\"\r\n\"uxbp\"\r\n\"up\\\\mgrcyaegiwmjufn\"\r\n\"nulscgcewj\\\\dvoyvhetdegzhs\\\"\"\r\n\"masv\\\"k\\\\rzrb\"\r\n\"qtx\\x79d\\\"xdxmbxrvhj\"\r\n\"fid\\\\otpkgjlh\\\"qgsvexrckqtn\\xf4\"\r\n\"tagzu\"\r\n\"bvl\\\\\\\"noseec\"\r\n\"\\\\xgicuuh\"\r\n\"w\\\"a\\\"npemf\"\r\n\"sxp\"\r\n\"nsmpktic\\x8awxftscdcvijjobnq\\\"gjd\"\r\n\"uks\\\"\\\"jxvyvfezz\\\"aynxoev\\\"cuoav\"\r\n\"m\"\r\n\"lkvokj\"\r\n\"vkfam\\\"yllr\\\"q\\x92o\\x4ebecnvhshhqe\\\\\"\r\n\"efdxcjkjverw\"\r\n\"lmqzadwhfdgmep\\x02tzfcbgrbfekhat\"\r\n\"cpbk\\x9azqegbpluczssouop\\x36ztpuoxsw\"\r\n\"cqwoczxdd\\\"erdjka\"\r\n\"cwvqnjgbw\\\\fxdlby\"\r\n\"mvtm\"\r\n\"lt\\\"bbqzpumplkg\"\r\n\"ntd\\xeeuwweucnuuslqfzfq\"\r\n\"y\\xabl\\\"dbebxjrlbmuoo\\\\\\x1au\"\r\n\"qjoqx\\\\a\"\r\n\"pu\\\"ekdnfpmly\\xbago\\\"\"\r\n\"fjhhdy\"\r\n\"arl\"\r\n\"xcywisim\\\"bwuwf\\\"\\\"raepeawwjub\"\r\n\"pbe\"\r\n\"dbnqfpzyaumxtqnd\\xc5dcqrkwyop\"\r\n\"ojv\\x40vtkwgkqepm\\x8bzft\\\\vedrry\"\r\n\"wggqkfbwqumsgajqwphjec\\\"mstxpwz\"\r\n\"zjkbem\"\r\n\"icpfqxbelxazlls\"\r\n\"pvpqs\\\\abcmtyielugfgcv\\\"tjxapxqxnx\"\r\n\"oqddwlvmtv\\\"\\x39lyybylfb\\\"jmngnpjrdw\"\r\n\"gisgbve\"\r\n\"\\\"aglg\"\r\n\"y\\\"\\\"ss\\xafvhxlrjv\"\r\n\"qbgqjsra\"\r\n\"ihshbjgqpdcljpmdwdprwloy\"\r\n\"djja\\\\wcdn\\\"svkrgpqn\\\"uz\\\"hc\\x43hj\"\r\n\"cbjm\"\r\n\"pnn\"\r\n\"pqvh\\\"noh\"\r\n\"\\\"\\\\fdktlp\"\r\n\"ncea\"\r\n\"pqgzphiyy\"\r\n\"\\xbedovhxuipaohlcvkwtxwmpz\\\"ckaif\\\"r\"\r\n\"arjuzbjowqciunfwgxtph\\\"vlhy\\\"n\"\r\n\"c\"\r\n\"nrpdxunulgudqzlhtae\"\r\n\"iefheu\\\"uru\\\"\"\r\n\"aqijysxuijud\\\"np\\\\opbichhudil\\xbesum\"\r\n\"pfpevmtstl\\\"lde\\\"bzr\\\"vspdxs\"\r\n\"vparfbdjwvzsocpnzhp\"\r\n\"g\\x4ffxaarafrsjthq\\\\\\xc1rw\"\r\n\"ng\\\\rqx\\\\gwpzucbh\\xafl\"\r\n\"rw\\\"nf\\\\dna\"\r\n\"jkkeahxurxla\\\\g\\xb3czrlsyimmwcwthr\"\r\n\"twaailoypu\\\"oas\\\"kpuuyedlaw\\\\\\xb0vzt\"\r\n\"hznex\\\\gdiqvtugi\"\r\n\"imdibsunjeswhk\"\r\n\"ta\\\\icileuzpxro\\\"cfmv\\\"mzp\"\r\n\"coykr\\x57luiysucfaflmilhlehmvzeiepo\"\r\n\"u\\x3dfh\\xd4yt\"\r\n\"piw\\x1bz\\\"eowy\\\"vfk\\\"wqiekw\"\r\n\"gan\\\"y\"\r\n\"p\\\"bevidoazcznr\\\"hddxuuq\\\"\"\r\n\"bwzucczznutbxe\"\r\n\"z\\\"viqgyqjisior\\\\iecosmjbknol\"\r\n\"dmlpcglcfkfsctxydjvayhymv\\x3c\\\\gp\"\r\n\"bfvkqrintbbvgfv\"\r\n\"xlzntrgdck\\\"cprc\\xadczyarbznqmuhxyuh\"\r\n\"uqdxnuwioc\\\"kdytxq\\\\ig\"\r\n\"xrafmucpmfi\"\r\n\"vr\\\"hltmfrge\"\r\n\"eonf\\\"nt\\\\wtcnsocs\"\r\n\"j\\xb7xoslyjeyjksplkqixncgkylkw\"\r\n\"njw\\\"pefgfbez\\x9axshdmplxzquqe\"\r\n\"di\\x58bvptfsafirpc\"\r\n\"l\\x1fkco\"\r\n\"x\"\r\n\"mprndo\\\"n\"\r\n\"psegit\"\r\n\"svbdnkkuuqs\\\"sqxu\\\"oqcyz\\\"aizashk\"\r\n\"cwkljukxer\\\\\\\"\\\\nff\\\"esjwiyaoy\"\r\n\"ilxrkgbjjxpvhdtq\\\"cpiuoofdnkpp\"\r\n\"hlngi\\\"ulxep\\\\qohtmqnqjb\\\"rkgerho\"\r\n\"gxws\\\"bcgm\\\"p\"\r\n\"bv\\\"mds\\\\zhfusiepgrz\\\\b\\x32fscdzz\"\r\n\"l\\xfampwtme\\x69qvxnx\\\"\\\"\\xc4jruuymjxrpsv\"\r\n\"qqmxhrn\"\r\n\"xziq\\\\\\x18ybyv\\x9am\\\"neacoqjzytertisysza\"\r\n\"aqcbvlvcrzceeyx\\\\j\\\"\\\"x\"\r\n\"yjuhhb\"\r\n\"\\x5em\\\"squulpy\"\r\n\"dpbntplgmwb\"\r\n\"utsgfkm\\\\vbftjknlktpthoeo\"\r\n\"ccxjgiocmuhf\\\"ycnh\"\r\n\"lltj\\\"kbbxi\"\r\n";

Part1(myInput);
Part2(myInput);
Console.ReadKey();

void Part1(string pInput)
{
    StringReader reader = new StringReader(pInput);

    string? codedLine = null;

    int totalDifference = 0;

    while ((codedLine = reader.ReadLine()) != null)
    {
        string processedLine = codedLine;

        //option 1: transform the strings and see how many chars we save
        processedLine = processedLine.Substring(1, processedLine.Length - 2);       //take out the first and last quotes
        processedLine = processedLine.Replace("\\\"", "\"");                        //replace any escaped " with the actual "
        processedLine = processedLine.Replace("\\\\", "\\");                        //replace any escaped \ with the actual
        processedLine = Regex.Replace(processedLine, @"\\x[0-9a-fA-F]{2}", "x");    //replace any \xAA sequence with a single char

        totalDifference += (codedLine.Length - processedLine.Length);
    }

    Console.WriteLine("Part 1:"+totalDifference);
}

void Part2(string pInput)
{
    StringReader reader = new StringReader(pInput);

    string? codedLine = null;

    int totalDifference = 0;

    while ((codedLine = reader.ReadLine()) != null)
    {
        //option 2: if we just follow the rules of what the string WOULD become
        //we don't even have to actually process the strings to count the changes
        totalDifference += codedLine.Count(a => a == '\"'); //every " (note " is escaped as well) becomes \"
        totalDifference += codedLine.Count(a => a == '\\'); //every \ (note \ is escaped as well) becomes \\
        totalDifference += 2;                               //every start and end " becomes \"
    }

    Console.WriteLine("Part 2:"+totalDifference);
}



