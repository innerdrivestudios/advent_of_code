// Moved everything into a separate class,
// since the code is very similar but verbose 

class Part2 : Part1
{
	protected override string order => "J23456789TQKA";

    // Get Rank just got more complicated :)
    override protected Hands GetRank(string pHand)
    {
        // First get our score of the hand without any jokers
        string handWithoutJokers = pHand.Replace("J", "");

        // And use that hand to find out how many jokers we actually have
        int jokerCount = 5 - handWithoutJokers.Length;

        // Now get the base rank of our hand without jokers...  
        Hands rank = base.GetRank(handWithoutJokers);

        // So now we know the base rank, and we know how many jokers we have...
        // in other words we know how we can improve our score

        // What is the rank we can have?
        // HighCard - 0 room for jokers                  
        // Full house - 0 room for jokers                
        // Five of a Kind - 0 room for jokers            
        // Four of a kind - 1 space for jokers
        // Two pair - 1 space for jokers
        // Three of a kind - max 2 jokers
        // One pair - max 3 jokers
        // NoMatch - max 5 jokers

        // Upgrade the rank as far as possible:
        // Is it pretty? No... is it effective? Yeah ;)

        if (rank == Hands.OnePair && jokerCount == 1) rank = Hands.ThreeOfAKind;
        else if (rank == Hands.OnePair && jokerCount == 2) rank = Hands.FourOfAKind;
        else if (rank == Hands.OnePair && jokerCount == 3) rank = Hands.FiveOfAKind;

        else if (rank == Hands.TwoPair && jokerCount == 1) rank = Hands.FullHouse;

        else if (rank == Hands.ThreeOfAKind && jokerCount == 1) rank = Hands.FourOfAKind;
        else if (rank == Hands.ThreeOfAKind && jokerCount == 2) rank = Hands.FiveOfAKind;

        else if (rank == Hands.FourOfAKind && jokerCount == 1) rank = Hands.FiveOfAKind;

        else if (rank == Hands.NoMatch && jokerCount == 1) rank = Hands.OnePair;
        else if (rank == Hands.NoMatch && jokerCount == 2) rank = Hands.ThreeOfAKind;
        else if (rank == Hands.NoMatch && jokerCount == 3) rank = Hands.FourOfAKind;
        else if (rank == Hands.NoMatch && jokerCount == 4) rank = Hands.FiveOfAKind;
        else if (rank == Hands.NoMatch && jokerCount == 5) rank = Hands.FiveOfAKind;

        if (rank == Hands.NoMatch) Console.WriteLine(pHand);

        return rank;
    }


}
