// Solution for https://adventofcode.com/2023/day/4 (Ctrl+Click in VS to follow link)

using ScratchCard = (System.Collections.Generic.HashSet<int> winning, System.Collections.Generic.HashSet<int> yours);

// In visual studio you can modify which file by going to Debug/Debug Properties
// and setting $(SolutionDir)input.txt as the command line, this will be passed to args[0]

// Your input: a list of scratchcards describing the numbers on the scratchcard and the numbers you have

string myInput = File.ReadAllText(args[0]);


// Similar to the Bingo card approach from 2021 day 4, we'll convert the input to a list of winning
// numbers and numbers you have...

// Card   x:  19 43 45 |  34 67 43 56 50 27

// Ok, this is getting a little unreadable at a certain point ;), just playing with LINQ tbh

List<ScratchCard> scratchcards =
	myInput
		//Split into card lines
		.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
		//Take everything from those lines AFTER the :
		.Select(cardLine => cardLine.Split(":")[1])
		//Split those lines into a winning-numbers/your-numbers string[]
		.Select(cardLine => cardLine.Split("|", StringSplitOptions.RemoveEmptyEntries))
		//Split the items from that string[] into two separate hashsets
		.Select(
			numberStrings => ( ConvertToHashSet(numberStrings[0]) , ConvertToHashSet(numberStrings[1]) ) 
		)
		.ToList();

HashSet<int> ConvertToHashSet (string pNumberString)
{
	return new HashSet<int>(
		pNumberString
			.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select(int.Parse)
	);
}

// Part 1 - Calculate how much these scratch cards are worth in total

long GetScratchCardValue (ScratchCard pScratchCard)
{
	int overlapCount = pScratchCard.winning.Intersect(pScratchCard.yours).Count();

	if (overlapCount > 0) return (long)Math.Pow(2, overlapCount - 1);
	else return 0;
}

Console.WriteLine("Part 1 - Value of all scratchcards: " + scratchcards.Sum(x => GetScratchCardValue(x)));

// Part 2 - Recursion baby yeah ! ;)
// Different kind of scoring mechanism!

// "There's no such thing as "points".
// Instead, scratchcards only cause you to win more scratchcards equal to the number of winning numbers you have.
// Specifically, you win copies of the scratchcards below the winning card equal to the number of matches.
// So, if card 10 were to have 5 matching numbers, you would win one copy each of cards 11, 12, 13, 14, and 15.
// Copies of scratchcards are scored like normal scratchcards and have the same card number as the card they copied. So, if you win a copy of card 10 and it has 5 matching numbers, it would then win a copy of the same cards that the original card 10 won: cards 11, 12, 13, 14, and 15. This process repeats until none of the copies cause you to win any more cards. (Cards will never make you copy a card past the end of the table.)

long GetWonScratchCardCount(List<ScratchCard> pScratchCards, int pIndex)
{
	ScratchCard scratchCard = pScratchCards[pIndex];

	long amountWon = scratchCard.winning.Intersect(scratchCard.yours).Count();

	//Score is one per scratchcard
	long score = 1;

	for (int i = pIndex + 1; i < Math.Min(pIndex + amountWon + 1, pScratchCards.Count); i++)
	{
		score += GetWonScratchCardCount(pScratchCards, i);
	}

	return score;
}

long totalAmountOfScratchCardsWon = 0;

for (int i = 0; i < scratchcards.Count; i++) {
	totalAmountOfScratchCardsWon += GetWonScratchCardCount(scratchcards, i);
}

Console.WriteLine("Part 2 - Total amount of scratchcards won: " + totalAmountOfScratchCardsWon);
