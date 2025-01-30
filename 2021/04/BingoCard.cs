
class BingoCard
{
    //A Bingo card wraps a set of rows and colums with both open and closed numbers
    private List<(HashSet<int> open, HashSet<int> closed)> rows = new();
    private List<(HashSet<int> open, HashSet<int> closed)> columns = new();

    private int lastPlayedNumber = -1;
    private bool won = false;

    public BingoCard (Grid<int> pGrid)
    {
        //create all rows and columns (grid is square)
        for (int i = 0; i < pGrid.width; i++)
        {
            HashSet<int> rowData = new HashSet<int>();
            for (int x = 0; x < pGrid.width; x++) rowData.Add(pGrid[x, i]);
            
            HashSet<int> columnData = new HashSet<int>();
            for (int y = 0; y < pGrid.height; y++) columnData.Add(pGrid[i, y]);

            rows.Add((rowData, new HashSet<int>()));
            columns.Add((columnData, new HashSet<int>()));
        }
    }

    public bool Play(int pMove)
    {
        lastPlayedNumber = pMove;
        Play(rows, pMove);
        Play(columns, pMove);
        return won;
    }

    //For each row and column move the move from open to closed and see if any open are empty
    //if so, we've completed a line
    private void Play (List<(HashSet<int> open, HashSet<int> closed)> pLines, int pMove)
    {
        foreach (var line in pLines)
        {
            if (line.open.Contains(pMove))
            {
                line.open.Remove(pMove);
                line.closed.Add(pMove);

                if (line.open.Count == 0)
                {
                    won = true;
                }
            }
        }
    }

    public bool HasWon()
    {
        return won;
    }

    public int GetScore()
    {
        //find the sum of all unmarked numbers... and multiply with winning number...
        //note that it doesn't matter whether we are using the rows or columns for this
        return rows.Sum (x => x.open.Sum()) * lastPlayedNumber;
    }
}
