//We're using integer coordinates
using Vec2i = Vec2<int>;   

class RepairDroid : IIntCodeIO
{
    class Node
    {
        public int movementIndex;       //which directions we've already tested

        public Node(int pMovementIndex)
        {
            movementIndex = pMovementIndex;
        }
    }

    // Several positions we need to keep track of:

    // - This is where we plan to move, while waiting for feedback
    // (the start position was chosen so that the whole area fits...)
    private Vec2i startPosition = new Vec2i(22, 22);
    private Vec2i nextPosition = new ();
	// - This is where we currently are, initially next and current are the same
	private Vec2i currentPosition = new ();
	// - This stores the final location of oxygen
    private Vec2i oxygenPosition = new ();     
    
    // While exploring we need to keep track of where we've already been
    HashSet<Vec2i> visited = new ();  
    // And be able to differentiate walls from open space
    HashSet<Vec2i> walls = new();

	// There are 4 directions, north (1) , south (2), west (3), and east (4)
    // whose values match these direction vectors
    private Vec2i[] directionVectors = [new(0,0), new (0,-1), new (0,1), new(-1,0), new (1,0)];

	// And if we need to backtrack, we'll need to know their opposites
	//                                    1,2,3,4
	private int[] oppositeDirections = [0,2,1,4,3];

    // The repair droid can reply with any of the following status codes:

    // 0: The repair droid hit a wall. Its position has not changed.
    // 1: The repair droid has moved one step in the requested direction.
    // 2: The repair droid has moved one step in the requested direction;
    //    its new position is the location of the oxygen system.
    enum StatusCode { Wall = 0, Moved = 1, Moved_And_Oxygen_Found = 2 };

    // We'll keep track of any directions we still need to explore on a stack,
    // until we've explored every direction for every node on the stack
    private Stack<Node> stack = new ();

    // We start by accepting our initial nextPosition (e.g. we move onto the playing field)
    // which will create the first stack node.
    private StatusCode lastReply = StatusCode.Moved;

    public RepairDroid() {
        //Console.WriteLine("Resize the window and press enter");
        //Console.ReadKey();
        //Console.Clear();

        nextPosition = currentPosition = startPosition;
    }

    // The droid starts by reading what it needs to ...
    // 0    - exit program
    // 1-4  - north, south, west, east
    public long Read()
    {
        // Thread.Sleep (10);

        // We have moved to where we wanted to go...
        if (lastReply == StatusCode.Moved || lastReply == StatusCode.Moved_And_Oxygen_Found)
        {
            Plot(currentPosition, '.');
            currentPosition = nextPosition;
            Plot(currentPosition, 'D');

            if (lastReply == StatusCode.Moved_And_Oxygen_Found)
            {
                oxygenPosition = currentPosition;
                Plot(currentPosition, 'O');
                //Console.ReadKey();
            }
            
            // Store our new position and create a new stack node
            if (!visited.Contains(currentPosition))
            {
                visited.Add(currentPosition);
                stack.Push(new Node(0));
            }

        }
        else if (lastReply == StatusCode.Wall)
        {
            // When we hit a wall, we don't move, but we do need to record a wall
            visited.Add(nextPosition);
            walls.Add(nextPosition);
            Plot(nextPosition, 'X');
        }


        // If we haven't moved, we are still at the same location, 
        // so we don't update the currentPosition, we just continue with the top stack node

        // See if there are still directions left to visit ...
        Node node = stack.Peek();
        node.movementIndex++;

        while (node.movementIndex < directionVectors.Length)
        {
            nextPosition = currentPosition + directionVectors[node.movementIndex];

            if (visited.Contains(nextPosition))
            {
                node.movementIndex++;
            }
            else
            {
                return node.movementIndex;
            }
        }

        // No directions left to visit? Move to the previous location...
        if (node.movementIndex >= directionVectors.Length)
        {
            // Remove this node where we are currently 
            stack.Pop();

            if (stack.Count == 0)
            {
                Plot(oxygenPosition, 'O');
                Plot(startPosition, 'S');
                Plot(new Vec2i(0, 50), 'X');
                return 0;
            }

            // And undo the move that brought us here so we can move back to it
            int oppositeDirection = oppositeDirections[stack.Peek().movementIndex];
            nextPosition = currentPosition + directionVectors[oppositeDirection];
            return oppositeDirection;
        }

        return 0;
    }

    // Write provides feedback on whatever input (Read) we provided to the Droid
    public void Write(long value)
    {
        lastReply = (StatusCode)value;
    }

    public void Plot (Vec2i pLocation, char pChar)
    {
        //Console.SetCursorPosition(pLocation.X, pLocation.Y);
        //Console.Write(pChar);
    }

    public HashSet<Vec2i> GetVisited() { return visited; }
    public HashSet<Vec2i> GetWalls() { return walls; }
    public Vec2i GetStart() { return startPosition; }
    public Vec2i GetOxygen() { return oxygenPosition; }

}
