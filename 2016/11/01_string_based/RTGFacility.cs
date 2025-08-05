using System.Text;

class RTGFacility {

    //List [0] is floor 0, List[1] is floor 1 etc the HashSet describes the things on that floor
    //Experimented with both Lists and HashSets, in practice in this setup didn't make a shitton of difference...
    private List<HashSet<string>> floorsWithItems = new();

    //Where are we?
    public int elevatorFloor { get; private set; }

    //How many moves did we make to get into the current state?
    public int movesMade { get; private set; }

    //What is the total amount of items we need to get to the last floor?
    private int maxItems = 0;

    //Unfortunately to speed up Dijkstra we need a unique representation of our state...
    //Couldn't think of a better way than this (except for going full blown bit arithmatic)
    private StringBuilder stringBuilder = new StringBuilder();
    private string cachedStringRep = null;

    //Add a floor if the items on it are valid
    //Note that we don't check duplicates between floors etc!
    public void AddFloor (HashSet<string> pItems) { 
        if (TestFloor(pItems))
        {
            //Keep track of the total amount of items over all floors
            maxItems += pItems.Count;
            floorsWithItems.Add(pItems);
        }
        else
        {
            throw new InvalidDataException("The initial floor data is not valid for floor:"+string.Join(",", pItems));
        }
    }

    // Any state that has at least 1 RTG, where there is at least 1 chip unpowered by its own RTG is invalid.
    private bool TestFloor(IEnumerable<string> pItems)
    {
        // This can be done way more concise with LINQ, but that's even slower...

        // Get the rtgCount...
        int rtgCount = 0;
        foreach (string item in pItems)
        {
            if (item[1] == 'G') rtgCount++;
        }
       
        // If there are no generators, we are always safe ...
        if (rtgCount == 0) return true;

        // If there are generators, but no chips without there generator, we are still safe...
        // Basically the next line says, find chips (x[1] == 'M') where their generator is not in the items set

        foreach (string item in pItems)
        {
            if (item[1] == 'M')
            {
                if (!pItems.Contains("" + item[0] + "G"))
                {
                    return false;
                }
            }
        }

        return true;
    }

    //returns all possible valid changes from this state to a next state
    public List<Move> GetPossibleMoves ()
    {
        List<Move> possibleMoves = new List<Move>();

        //Convert items to list so we can iterate easier ... bit slow, but let's see how it works...
        List<string> currentFloorItems = floorsWithItems[elevatorFloor].ToList();

        for (int i = 0; i < currentFloorItems.Count; i++)
        {
            //We can move 1 item at a time...
            AddValidMovesFor(possibleMoves, new Move(0, [currentFloorItems[i]]));

            for (int j = i + 1; j < currentFloorItems.Count; j++)
            {
                //Or 2 items at a time...
                AddValidMovesFor(possibleMoves, new Move(0, [currentFloorItems[i], currentFloorItems[j]]));
            }
        }

        return possibleMoves;
    }

    private void AddValidMovesFor(List<Move> pPossibleMoves, Move pMove)
    {
        if (elevatorFloor > 0)
        {
            Move downMove = pMove;                  //Clone the input (Move is a struct)
            downMove.floorDelta = -1;
            
            if (
                IsValid (downMove) && 
                downMove.itemsDelta.Count == 1      //Count == 1 -> never bring two items back down
                                                    //This is a life saver, the difference between completing and not completing!
               )
            {
                pPossibleMoves.Add(downMove);
            }
        }

        if (elevatorFloor < floorsWithItems.Count - 1)
        {
            Move upMove = pMove;                    //Clone the input (Move is a struct)
            upMove.floorDelta = 1;

            if (IsValid(upMove))
            {
                pPossibleMoves.Add(upMove);
            }
        }
    }

    private bool IsValid (Move pMove)
    {
        //Both the current floor without the moving items needs to be valid
        //And the next floor with the moving items needs to be valid
        return
            TestFloor(floorsWithItems[elevatorFloor].Except(pMove.itemsDelta)) &&
            TestFloor(floorsWithItems[elevatorFloor + pMove.floorDelta].Union(pMove.itemsDelta));
    }

    //this assumes the given move is valid (e.g. requested through GetPossibleMoves())!
    public RTGFacility MakeMove (Move pMove)
    {
        RTGFacility clone = new RTGFacility();

        foreach (HashSet<string> items in floorsWithItems)
        {
            clone.AddFloor(items);
        }

        clone.elevatorFloor = elevatorFloor;
        clone.movesMade = movesMade+1;

        clone.floorsWithItems[elevatorFloor] = 
            new HashSet<string>(floorsWithItems[elevatorFloor].Except(pMove.itemsDelta));
        clone.floorsWithItems[elevatorFloor+pMove.floorDelta] = 
            new HashSet<string>(floorsWithItems[elevatorFloor + pMove.floorDelta].Union(pMove.itemsDelta));

        clone.elevatorFloor += pMove.floorDelta;

        return clone;
    }

    public override string ToString()
    {
        if (cachedStringRep != null) return cachedStringRep;

        stringBuilder.Clear();
        stringBuilder.AppendLine("" + elevatorFloor);

        for (int i = 0; i < floorsWithItems.Count; i++)
        {
            stringBuilder.AppendLine("" + i);
            List<string> items = floorsWithItems[i].ToList();
            items.Sort();
            foreach (var item in items) { stringBuilder.Append(item); }
        }

        cachedStringRep = stringBuilder.ToString();

        return cachedStringRep;
    }

    public bool IsSolution ()
    {
        return elevatorFloor == floorsWithItems.Count - 1 && floorsWithItems[elevatorFloor].Count == maxItems;
    }

    public int GetRating ()
    {
        // This made the algorithm go really fast, but too greedy and not return the right answer.

        /**
        int rating = 0;

        for (int i = 0;i < floorsWithItems.Count;i++)
        {
            rating += floorsWithItems[i].Count * (floorsWithItems.Count - i + 1) ;
        }

        return rating;
        */

        //This seemed like a good heuristic, but made the algorithm run pretty slow still (right answer though!)
        //return movesMade + (floorsWithItems.Count - elevatorFloor);

        //This made the algorithm run pretty fast, with the right answer but can't really explain it, which is annoying
        //return movesMade * (floorsWithItems.Count - elevatorFloor);

        //Heuristic to guess how many steps it will take to get all the items on lower floors to the upper floor
        //This basically says well to get any item from a floor below the max will require at least x steps
        //where x is the difference between where the elevator is now the top floor
        //the * 2 is questionable though :)
        int costEstimate = 0;

        for (int i = elevatorFloor; i < floorsWithItems.Count; i++)
        {
            costEstimate += floorsWithItems[i].Count * (floorsWithItems.Count - 1 - elevatorFloor) * 2;
        }

        //A*
        return movesMade + costEstimate;
    }

}
