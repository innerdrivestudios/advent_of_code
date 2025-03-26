using System.Collections.Generic;

class Part1
{
	List<int> marbles = new List<int>();					// all marbles
	int nextMarble = 0;                                     // next marble value
	int currentMarbleIndex = 0;                             // indicates the current marble
	int playerAmount;										// how many players we have
	int[] playerScores;										// keep track of all the scores

	public int Run(int pPlayerAmount, int pLastMarble)
	{
		playerAmount = pPlayerAmount;
		playerScores = new int[pPlayerAmount];         
		
		//Initialize the first element in the list
		marbles.Add(nextMarble++);

		for (int i = 0; i < pLastMarble; i++) InsertMarble(); 

		return playerScores.Max();
	}

	void InsertMarble()
	{
		if (nextMarble % 23 == 0)
		{
			int removeIndex = ((currentMarbleIndex - 7) + marbles.Count) % marbles.Count;
			
			int currentPlayer = ((nextMarble % playerAmount));
			playerScores[currentPlayer] += nextMarble + marbles[removeIndex];

			marbles.RemoveAt(removeIndex);
			currentMarbleIndex = removeIndex % marbles.Count;
		}
		else
		{
			//take current index and add 1... now we are directly clockwise of the current element
			//but we want to go to the next element after that, so we need to add another one...
			//however, we also need to make sure we successfully wrap around to the start when necessary...

			//if currentMarbleIndex is >=0 and < marbles.Count-2, the result will be >= 2 and < marbles.Count 
			//if currentMarbleIndex is marbles.Count-2 (the before last marble), the result will be marbles.Count 
			//		and the new marble will be added at the end
			//if currentMarbleIndex is marbles.Count-1 (the last marble), the result will 1 which is also correct
			//		(note marble 0 never changes in the beginning due to these shifts)
			//and yes this took some trial and error ;)

			int insertIndex = ((currentMarbleIndex + 1) % marbles.Count) + 1;
			marbles.Insert(insertIndex, nextMarble);

			currentMarbleIndex = insertIndex;
		}

		nextMarble++;
	}

}

