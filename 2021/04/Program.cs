// Solution for https://adventofcode.com/2021/day/4 (Ctrl+Click in VS to follow link)

// Your input: a batch of moves for a game of bingo and a bunch of bingo boards

// In visual studio you can modify which file by going to Debug/Debug Properties
// and setting $(SolutionDir)input.txt as the command line, this will be passed to args[0]

string myInput = File.ReadAllText(args[0]);

//Get all the separate blocks of text...
List<string> splitInput = myInput.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries).ToList();

//... the first element are all the "moves" made on each bingo card
int[] moves = splitInput[0].Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();  

//... and remove that one so we are left with blocks of text that contain bingo card data
splitInput.RemoveAt(0);

//Now we convert every string in the bingo card data to a BingoCard,
//using the Grid class as an intermediate helper to easily parse all the grid based data
//Lazy? Maybe a bit ;)
List<BingoCard> bingoCards = splitInput
    .Select( 
        bingoCardData => new BingoCard (
                            new Grid<int>(bingoCardData, "\r\n", " ") 
                         ) 
     )
    .ToList();

//Now for every move in the move list... make the move on all bingocards until one wins...
//Return null if none wins (which will crash the program yes)
BingoCard GetFirstWinningBingoCard ()
{
    foreach (int move in moves)
    {
        foreach (BingoCard bingoCard in bingoCards)
        {
            if (bingoCard.Play(move)) return bingoCard;
        }
    }

    return null;
}

Console.WriteLine("Part 1 - Bingo score: " + GetFirstWinningBingoCard().GetScore());

// For part 2, we mix it up a bit by getting the board that wins last:

BingoCard GetLastWinningBingoCard()
{
    BingoCard lastWinningBingoCard = null;

    //Slightly different approach, each time a bingo card wins, we store it and remove it from our list
    //so we don't continue to make moves on it
    foreach (int move in moves)
    {
        //Go backwards so we can remove items easily
        for (int i = bingoCards.Count - 1; i >= 0; i--)
        {
            BingoCard bingoCard = bingoCards[i];

            if (bingoCard.Play(move))
            {
                lastWinningBingoCard = bingoCard;
                bingoCards.RemoveAt(i);
            }
        }
    }

    return lastWinningBingoCard;
}

Console.WriteLine("Part 2 - Bingo score: " + GetLastWinningBingoCard().GetScore());

