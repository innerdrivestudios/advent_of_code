/**
 * Simple struct to contain a move (yes, with dirty public fields, I know :).
 */
struct Move{
    public int floorDelta;              //+1 or -1
    public int itemsDelta;              //Which items (bits) should move from the current floor to the next?

    public Move (int pFloorDelta, int pItemsDelta)
    {
        floorDelta = pFloorDelta;
        itemsDelta = pItemsDelta;   
    }

}

