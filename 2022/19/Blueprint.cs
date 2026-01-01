using System.Text.RegularExpressions;

/**
 * Blue prints defines how much the robots will cost us...
 */
class Blueprint
{
	public readonly int id;
	public readonly ResourceDefinition oreRobotCost;
    public readonly ResourceDefinition clayRobotCost;
    public readonly ResourceDefinition obsidianRobotCost;
    public readonly ResourceDefinition geodeRobotCost;

	Regex blueprintParser = new Regex(
		@"Blueprint (\d+): "+										//Group 1
		@"Each ore robot costs (\d+) ore. " +                       //Group 2
		@"Each clay robot costs (\d+) ore. " +                      //Group 3
		@"Each obsidian robot costs (\d+) ore and (\d+) clay. " +   //Group 4&5
		@"Each geode robot costs (\d+) ore and (\d+) obsidian.",	//Group 6&7
		RegexOptions.Compiled
	);

	public Blueprint (string pBlueprintDefinition)
	{
		Match match = blueprintParser.Match( pBlueprintDefinition );
		if (!match.Success) throw new FormatException(pBlueprintDefinition);

		int bluePrintId					= int.Parse(match.Groups[1].Value);
		int oreRobotOreCost				= int.Parse(match.Groups[2].Value);
		int clayRobotOreCost			= int.Parse(match.Groups[3].Value);
		int obsidianRobotOreCost		= int.Parse(match.Groups[4].Value);
		int obsidianRobotClayCost		= int.Parse(match.Groups[5].Value);
		int geodeRobotOreCost			= int.Parse(match.Groups[6].Value);
		int geodeRobotObsidianCost		= int.Parse(match.Groups[7].Value);

		id = bluePrintId;

		oreRobotCost = new ResourceDefinition(oreRobotOreCost, 0, 0, 0);
		clayRobotCost = new ResourceDefinition(clayRobotOreCost, 0, 0, 0);
		obsidianRobotCost = new ResourceDefinition(obsidianRobotOreCost, obsidianRobotClayCost, 0, 0);
		geodeRobotCost = new ResourceDefinition(geodeRobotOreCost, 0, geodeRobotObsidianCost, 0);
	}

}

