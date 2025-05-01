internal class Reindeer
{
	public string name;
	public int flyDuration;
	public int flySpeed;
	public int restDuration;
	
	public Reindeer(string pName, int pFlyDuration, int pFlySpeed, int pRestDuration)
	{
		name = pName;
		flyDuration = pFlyDuration;
		flySpeed = pFlySpeed;
		restDuration = pRestDuration;
	}

	public int GetDistanceTravelled (int pTime)
	{
        //first get duration of a whole fly 'cycle'
        int blockTime = (flyDuration + restDuration);
        //use int division to get a floored amount of 'whole' blocks
        int wholeBlocks = pTime / blockTime;                        

		int wholeBlockTime = wholeBlocks * blockTime;
		int leftOverTime = pTime - wholeBlockTime;

		//leftOverTime will be between 0 and blockTime, but out of blockTime we fly for a max of fly duration
		int flyingSecondsRemaining = Math.Min(leftOverTime, flyDuration);

		int totalTimeFlown = wholeBlocks * flyDuration + flyingSecondsRemaining;
		int distanceTravelled = totalTimeFlown * flySpeed;

		return distanceTravelled;
	}

}