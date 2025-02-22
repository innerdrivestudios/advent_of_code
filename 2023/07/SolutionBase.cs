abstract class SolutionBase
{
    protected abstract string order { get; }

    public long Run(List<(string hand, int bid)> pHands)
    {
        // Rank the hands, multiply them with the bids and sum them up

        pHands.Sort(CompareHands);

        long totalWinnings = 0;

        for (int i = 0; i < pHands.Count; i++)
        {
            totalWinnings += (long)((i + 1) * pHands[i].bid);
            //Console.WriteLine(pHands[i].hand + " => " + GetRank(pHands[i].hand));
            //Console.ReadKey();
        }

        return totalWinnings;
    }

    protected int CompareHands((string, int) pHand1, (string, int) pHand2)
    {
        Hands hands1Rank = GetRank(pHand1.Item1);
        Hands hands2Rank = GetRank(pHand2.Item1);

        if (hands1Rank != hands2Rank)
        {
            return hands1Rank - hands2Rank;
        }
        else
        {
            return CompareHandsOfEqualRank(pHand1.Item1, pHand2.Item1);
        }
    }

    abstract protected Hands GetRank(string pHand);

    // This assumes hands are equally long and have the same rank (not checked!)
    int CompareHandsOfEqualRank(string pHand1, string pHand2)
    {
        int i = 0;

        while (i < pHand1.Length)
        {
            //We cannot just subtract the chars themselves we need to compare them according to card order ....
            //If hand1 has an A and hand2 a 2, hand1 wins (is higher)
            int diff = order.IndexOf(pHand1[i]) - order.IndexOf(pHand2[i]);
            if (diff != 0) return diff;
            i++;
        }

        return 0;
    }

}
