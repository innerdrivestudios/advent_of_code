/** 
 * State defines where we might be in the search process.
 * How much resources we have, how many robots we currently have etc...
 */
class State
{
	public Blueprint blueprint;

	public ResourceDefinition inventory { private set; get; } = new ResourceDefinition(0,0,0,0);

	public int oreRobotCount		{ private set; get; } = 0;
	public int clayRobotCount		{ private set; get; } = 0;
	public int obsidianRobotCount	{ private set; get; } = 0;
	public int geodeRobotCount		{ private set; get; } = 0;
	public int time					{ private set; get; } = 0;

	private bool oreRobotsInProduction		= false;
	private bool clayRobotsInProduction		= false;
	private bool obsidianRobotsInProduction	= false;
	private bool geodeRobotsInProduction	= false;

	public State (Blueprint pBluePrint, ResourceDefinition pInventory, int pOreRobotCount, int pClayRobotCount, int pObsidianRobotCount, int pGeodeRobotCount, int pTime) {
		blueprint = pBluePrint;
		inventory = pInventory;
		oreRobotCount = pOreRobotCount;
		clayRobotCount = pClayRobotCount;
		obsidianRobotCount = pObsidianRobotCount;
		geodeRobotCount = pGeodeRobotCount;
		time = pTime;	
	}


	public State Clone()
	{
		//Shallow copy of the inventory, automatically deepcloned when we simulate one minute...
		return new State (blueprint, inventory, oreRobotCount, clayRobotCount, obsidianRobotCount, geodeRobotCount, time);
	}

	public void SimulateOneMinute()
	{
		inventory += 
			oreRobotCount		* RobotDefinitions.ORE_ROBOT_PRODUCTION +
			clayRobotCount		* RobotDefinitions.CLAY_ROBOT_PRODUCTION +
			obsidianRobotCount	* RobotDefinitions.OBSIDIAN_ROBOT_PRODUCTION +
			geodeRobotCount		* RobotDefinitions.GEODE_ROBOT_PRODUCTION;

		// We assume only one robot is in production at any time...
		if (oreRobotsInProduction) oreRobotCount++;
		if (clayRobotsInProduction) clayRobotCount++;
		if (obsidianRobotsInProduction) obsidianRobotCount++;
		if (geodeRobotsInProduction) geodeRobotCount++;

		oreRobotsInProduction = clayRobotsInProduction = obsidianRobotsInProduction = geodeRobotsInProduction = false;

		time++;
	}

	public override string ToString()
	{
		return "Inventory:" + inventory.ToString() + " Robot counts: " + (oreRobotCount, clayRobotCount, obsidianRobotCount, geodeRobotCount) + " " + time;
	}

	public IEnumerable<State> GetPossibleNextStates ()
	{
		// What we can do next depends on our amount of resources...
		List<State> possibleNextStates = new List<State>();

		if (inventory >= blueprint.geodeRobotCost)
		{
			State newGeodeRobot = Clone();
			newGeodeRobot.inventory = newGeodeRobot.inventory - blueprint.geodeRobotCost;
			newGeodeRobot.geodeRobotsInProduction = true;
			possibleNextStates.Add(newGeodeRobot);
		}
		else if (inventory >= blueprint.obsidianRobotCost)
		{
			State newObsidianRobot = Clone();
			newObsidianRobot.inventory = newObsidianRobot.inventory - blueprint.obsidianRobotCost;
			newObsidianRobot.obsidianRobotsInProduction = true;
			possibleNextStates.Add(newObsidianRobot);
		}
		else
		{
            if (inventory >= blueprint.clayRobotCost)
            {
                State newClayRobot = Clone();
                newClayRobot.inventory = newClayRobot.inventory - blueprint.clayRobotCost;
                newClayRobot.clayRobotsInProduction = true;
                possibleNextStates.Add(newClayRobot);
            }

			if (inventory >= blueprint.oreRobotCost)
			{
				State newOreRobot = Clone();
				newOreRobot.inventory = newOreRobot.inventory - blueprint.oreRobotCost;
				newOreRobot.oreRobotsInProduction = true;
				possibleNextStates.Add(newOreRobot);
			}

			// Only wait if we cannot buy a geode robot...
			possibleNextStates.Add(Clone());    //One option is to do and buy nothing
		}

        //Console.WriteLine(possibleNextStates.Count);

        return possibleNextStates;
	}

	public int GetScore()
	{
		int score = geodeRobotCount * inventory.geode;
		return -score * (30 - time);
	}

	/*
	public bool IsWorseThan (State pOther)
	{
		return
			time > pOther.time &&
			//inventory < pOther.inventory &&
			//oreRobotCount < pOther.oreRobotCount &&
			//clayRobotCount < pOther.clayRobotCount &&
			//obsidianRobotCount < pOther.obsidianRobotCount &&
			geodeRobotCount < pOther.geodeRobotCount;
	}
	*/
}
