class Part2
{
	CircularList<int> marbles;								// all marbles
	int nextMarble = 0;                                     // next marble value
	int playerAmount;										// how many players we have
	long[] playerScores;									// keep track of all the scores

	public long Run(int pPlayerAmount, int pLastMarble)
	{
		marbles = new CircularList<int>();
		nextMarble = 0;
		playerAmount = pPlayerAmount;
		playerScores = new long[pPlayerAmount];         
		
		//Initialize the first element in the list
		marbles.Add(nextMarble++);

		for (int i = 0; i < pLastMarble; i++) InsertMarble();

		return playerScores.Max();
	}

	void InsertMarble()
	{
		if (nextMarble % 23 == 0)
		{
            marbles.Move(-7);
			int currentPlayer = ((nextMarble % playerAmount));
			playerScores[currentPlayer] += nextMarble + marbles.Current;
			marbles.Remove();
		}
		else
		{
			marbles.Move(1);
			marbles.Add(nextMarble);	
		}

		nextMarble++;
	}

}

