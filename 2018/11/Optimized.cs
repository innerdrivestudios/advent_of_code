// One way to optimize this puzzle is to realize that the total for higher (x,y) pairs
// are a combination of previous (lower) (x,y) pairs. 
// For example the total for (x,y) is the:
// - the total for (x-1,y) + the total for (x, y-1) + the power for (x,y)
//   BUT this counts (x-1, y-1) twice, so
//   we need to subtract the total for (x-1, y-1) once to get the correct end result

// Let's see if we can put that into practice

class Optimized : BruteForce
{
	private int[,] cache = new int[301, 301];

	public Optimized(int pGridSerialNumber) : base(pGridSerialNumber)
	{
	}

	public override void Run()
	{
		FillTheCache();
		base.Run();
	}

	private void FillTheCache()
	{
		for (int y = 1; y < cache.GetLength(1); y++)
		{
			for (int x = 1; x < cache.GetLength(0); x++) { 
				//Get the sum of all power levels up to that specific x,y
				cache[x, y] = GetPowerLevel(x, y, gridSerialNumber)
					+ cache[x - 1, y]
					+ cache[x, y - 1]
					- cache[x - 1, y - 1];
			}
		}
	}

	//this one has been optimized based on the cache table
	protected override int GetSquarePowerLevel(int pX, int pY, int pSerial, int pSquareSize)
	{
		//to get the powerlevel at pX, pY for a size of pSquareSize
		//we take the sum at the bottom right corner point of the square
		//but remove the elements we don't want
		return cache[pX + pSquareSize - 1, pY + pSquareSize - 1]
			   - cache[pX - 1, pY + pSquareSize - 1]    //includes cache[pX-1, pY-1]
			   - cache[pX + pSquareSize - 1, pY - 1]    //includes cache[pX-1, pY-1] again
			   + cache[pX - 1, pY - 1];                 //undo 2nd removal of cache[pX-1, pY-1]
	
		//Note: the serial is now ignored!!
	}

}