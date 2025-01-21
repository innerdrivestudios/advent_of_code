class RTGFacility {

    //Floors are now simply a list of integers, describing the destribution of items of the floors
    //in 2 bit pairs (which you cannot see here, but becomes apparent when reading the rest of code)
    private List<int> floorsWithItems = new();

    //Where are we?
    public int elevatorFloor { get; private set; }

    //How many moves did we make to get into the current state?
    public int movesMade { get; private set; }

    //All items present value:
    //- if every item (every bit) is present on a floor, all the bits together will form a number, e.g.
    //  11 or 1111 or 111111 (depending on the number of items we configured the facility with)
    //so if the top "floor" in floorsWithItems equals this number we are done!
    private int allItemsPresentValue = 0;

    //Use this constructor for the initial setup to make sure the initial setup is verified
    public RTGFacility(List<int> pFloors)
    {
        //copy the passed in floors
        floorsWithItems = new List<int> (pFloors);

        //if we add all numbers, they'll add up to 11 or 1111 or 111111 (depending on the number of items)
        //so if any floor contains this number
        allItemsPresentValue = floorsWithItems.Sum();

        //Some sanity checks
        string totalValue = Convert.ToString(allItemsPresentValue, 2);
        if (totalValue.Contains('0')) throw new InvalidDataException("Invalid total, every items needs to be there");
        if (totalValue.Length % 2 == 1) throw new InvalidDataException("Invalid total, odd number of items");

        //do test if every floor is valid
        foreach (int floor in floorsWithItems)
        {
            if (!TestFloor(floor)) throw new InvalidDataException("Floor " + floor + "(" + Convert.ToString(floor, 2) + ") invalid");
        }

        //Console.WriteLine("RTGFacility correctly initialized with " + Convert.ToString(allItemsPresentValue, 2).Length + " items");
    }

    //Use this one for cloning, assuming any valid "move" made will not corrupt the validity of our facility :)
    private RTGFacility() { }

    // Any state that has at least 1 RTG, where there is at least 1 chip unpowered by its own RTG is invalid.
    private bool TestFloor(int pItems)
    {
        //Same principle as before, but now using bits ...

        //Check if there are any generators by checking whether any even bits are 1, 
        //eg 10 means there is a generator 01 means a chip 11 means both
        //but we need to check each 2 bits in the number

        //So for example in a number            1011001001
        //We need to split the number in pairs  10 11 00 10 01  
        //And then for each pair check what the value 10, 11, 00, 01 represents
        //Keep in mind the bit order is from high to low so the order is bit 8, bit 7, etc

        int itemsToTest = pItems;

        bool rtgFound = false;
        bool unpoweredChipsFound = false;

        while (itemsToTest > 0)
        {                                                   
            rtgFound |= (itemsToTest & 2) > 0;              //in a 00 set if the first bit (bit 2) is set we have a generator
            unpoweredChipsFound |= (itemsToTest & 3) == 1;  //in a 00 if anded with 3 (which is 11) and the result is 1, 
                                                            //there is a chip (eg the value is 01) but no generator

            //early exit
            if (rtgFound && unpoweredChipsFound) return false;

            //we test bits in blocks of 2
            itemsToTest >>= 2;
        }

        //if there is no unpoweredChip found return true
        return true;
    }

    //returns Move objects that toggle valid BITS that are 1 (and thus represent items that could move)
    public List<Move> GetPossibleMoves()
    {
        //First find all indices of bits on the current floor that are 1,
        //these represent items that could move
        List<int> bitIndices = new();
        int currentFloorItems = floorsWithItems[elevatorFloor];
        int index = 0;
        while (currentFloorItems > 0)
        {
            if ((currentFloorItems & 1) == 1) bitIndices.Add(index);
            index++;
            currentFloorItems >>= 1;
        }

        //Now use those indices to try and construct moves...
        List<Move> possibleMoves = new();

        for (int i = 0; i < bitIndices.Count; i++)
        {
            //We can move 1 item at a time...
            AddValidMovesFor(possibleMoves, new Move(0, 1 << bitIndices[i]));

            for (int j = i + 1; j < bitIndices.Count; j++)
            {
                AddValidMovesFor(possibleMoves, new Move(0, 1 << bitIndices[i] | 1 << bitIndices[j]));
            }

            //Last optimization, chips and generator pairs are independent
            //I would expect > 2 to already be a good optimization, but apparently that is too greedy
            if (possibleMoves.Count > 3) break;
            //Since I can't proof it will work in all situations, use at your own risk :)
        }
        return possibleMoves;
    }

    private bool AddValidMovesFor(List<Move> pPossibleMoves, Move pMove)
    {
        bool movesAdded = false;

        if (elevatorFloor > 0)
        {
            Move downMove = pMove;      //Clone the input (Move is a struct)
            downMove.floorDelta = -1;

            if (
                IsValid(downMove) &&
                GetBitCount (downMove.itemsDelta) == 1      //Count == 1 -> never bring two items back down
                                                            //This is a life saver, the difference between completing and not completing!
               )
            {
                pPossibleMoves.Add(downMove);
                movesAdded = true;
            }
        }

        if (elevatorFloor < floorsWithItems.Count - 1)
        {
            Move upMove = pMove;       //Clone the input (Move is a struct)
            upMove.floorDelta = 1;

            if (IsValid(upMove))
            {
                pPossibleMoves.Add(upMove);
                movesAdded = true;
            }
        }

        return movesAdded;
    }

    private bool IsValid(Move pMove)
    {
        //Both the current floor with the moving items REMOVED, needs to be valid
        //And the next floor with the moving items ADDED, needs to be valid
        return
            TestFloor(floorsWithItems[elevatorFloor] ^ pMove.itemsDelta) &&
            TestFloor(floorsWithItems[elevatorFloor + pMove.floorDelta] ^ pMove.itemsDelta);
    }

    //This assumes the given move is valid (e.g. requested through GetPossibleMoves())!
    public RTGFacility MakeMove(Move pMove)
    {
        //use the empty constructor to bypass all the checks!
        RTGFacility clone = new RTGFacility();
        clone.floorsWithItems = new List<int> (floorsWithItems);
        clone.allItemsPresentValue = allItemsPresentValue;

        clone.floorsWithItems[elevatorFloor] ^= pMove.itemsDelta;
        clone.floorsWithItems[elevatorFloor + pMove.floorDelta] ^= pMove.itemsDelta;
        clone.elevatorFloor += elevatorFloor + pMove.floorDelta;
        clone.movesMade = movesMade + 1;

        return clone;
    }

    /*
    public string GetUniqueIDAsString()
    {
        //This works for all floor and unlimited floors
        if (cachedRep != null) return cachedRep;
        stringBuilder.Clear();
        stringBuilder.Append(elevatorFloor);
        stringBuilder.Append("-");
        stringBuilder.AppendJoin("-", floorsWithItems);
        cachedRep = stringBuilder.ToString();
        return cachedRep;
    }
    */

    public long GetUniqueIDAsLong()
    {
        //This is optimized for this specific puzzle!
        return elevatorFloor << 62 |   //only need two bits for this
               floorsWithItems[0] |
               floorsWithItems[1] << 14 |
               floorsWithItems[2] << 28 |
               floorsWithItems[3] << 42;
    }

    public bool IsSolution ()
    {
        return elevatorFloor == floorsWithItems.Count - 1 && floorsWithItems[elevatorFloor] == allItemsPresentValue;
    }

    //A* Guestimate :)
    public int GetRating()
    {
        int costEstimate = 0;

        for (int i = elevatorFloor; i < floorsWithItems.Count; i++)
        {
            //estimate is the amount of bits/items that still need to be moved, from a floor to the last floor
            costEstimate += GetBitCount(floorsWithItems[i]) * (floorsWithItems.Count - 1 - i);
        }

        return movesMade + costEstimate;
    }

    //the amount of set bits indicate the amount of items stored
    private int GetBitCount (int pItems)
    {
        int bitCount = 0;
        int valueToTest = pItems;
        while (valueToTest > 0)
        {
            bitCount += (valueToTest & 1);
            valueToTest >>= 1;
        }
        return bitCount;
    }

}
