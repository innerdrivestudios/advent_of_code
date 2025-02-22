// Moved everything into a separate class,
// since the code is very similar but verbose 

class Part1 : SolutionBase
{
    // Used by GetRank
	protected override string order => "23456789TJQKA";

    override protected Hands GetRank(string pHand)
    {
        Dictionary<char, long> cardCount = new();

        foreach (char c in pHand)
        {
            cardCount.TryGetValue(c, out long count);
            cardCount[c] = count + 1;
        }

        Hands rank = Hands.NoMatch;

        // 5 different cards
        if (cardCount.Count == 5) rank = Hands.HighCard;
        // One card has a count of 5
        else if (cardCount.Count(x => x.Value == 5) == 1) rank = Hands.FiveOfAKind;
        // One card has a count of four
        else if (cardCount.Count(x => x.Value == 4) == 1) rank = Hands.FourOfAKind;
        //One card is there 3 times, 
        else if (cardCount.Count(x => x.Value == 3) == 1) rank = (cardCount.Count(x => x.Value == 2) == 1) ? Hands.FullHouse : Hands.ThreeOfAKind;
        //If there is exactly a count of 2 for two cards
        else if (cardCount.Count(x => x.Value == 2) == 2) rank = Hands.TwoPair;
        //One has a count of three, but there is only one other card
        else if (cardCount.Count(x => x.Value == 2) == 1) rank = Hands.OnePair;

        return rank;

    }

}
