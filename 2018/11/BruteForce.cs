class BruteForce
{
	protected int gridSerialNumber;

	public BruteForce(int pGridSerialNumber)
	{
		gridSerialNumber = pGridSerialNumber;
	}

	public virtual void Run()
	{
		// ** Part 1: Find the 3x3 grid with the best power level...

		var bestXYPart1 = GetBestXYForAGridSizeOf(3);

		Console.WriteLine("Part 1: Best x,y = " + bestXYPart1.x + "," + bestXYPart1.y);

		// ** Part 2: Find the overall best power level for any grid size:

		var bestXYPart2 = GetBestXYOverall();

		Console.WriteLine("Part 2: Best x, y, gridsize = " + bestXYPart2);
	}

	protected int GetPowerLevel(int pX, int pY, int pSerial)
	{
		int rackID = pX + 10;
		int powerLevel = rackID * pY;
		powerLevel += pSerial;
		powerLevel *= rackID;
		powerLevel = (powerLevel / 100) % 10;
		powerLevel -= 5;
		return powerLevel;
	}

	protected virtual int GetSquarePowerLevel(int pX, int pY, int pSerial, int pSquareSize)
	{
		int powerLevel = 0;
		for (int x = 0; x < pSquareSize; x++)
		{
			for (int y = 0; y < pSquareSize; y++)
			{
				powerLevel += GetPowerLevel(pX + x, pY + y, pSerial);
			}
		}
		return powerLevel;
	}

	(int x, int y, int powerLevel) GetBestXYForAGridSizeOf(int pSize)
	{

		int maxPowerLevel = 0;
		int bestX = -1;
		int bestY = -1;

		for (int x = 1; x <= 301 - pSize; x++)
		{
			for (int y = 1; y <= 301 - pSize; y++)
			{
				int powerLevel = GetSquarePowerLevel(x, y, gridSerialNumber, pSize);

				if (powerLevel > maxPowerLevel)
				{
					maxPowerLevel = powerLevel;
					bestX = x;
					bestY = y;
				}
			}
		}

		return (bestX, bestY, maxPowerLevel);
	}

	(int x, int y, int gridSize) GetBestXYOverall()
	{
		int bestX = -1;
		int bestY = -1;
		int maxPowerLevel = -1;
		int bestGridSize = -1;

		for (int gridSize = 1; gridSize <= 300; gridSize++)
		{
			Console.WriteLine("Processing grid size:" + gridSize);
			var result = GetBestXYForAGridSizeOf(gridSize);

			if (result.powerLevel > maxPowerLevel)
			{
				bestX = result.x;
				bestY = result.y;
				maxPowerLevel = result.powerLevel;
				bestGridSize = gridSize;
			}
		}

		return (bestX, bestY, bestGridSize);
	}
}
