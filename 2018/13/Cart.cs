using Vec2i = Vec2<int>;

class Cart
{
    public Vec2i position { get; private set; }

    private static string directionChars = ">v<^";
    
    private Directions<Vec2i> directions = new ([new(1,0), new(0,1), new(-1,0), new(0,-1)]);
    private int intersectionsCrossed = 0;
    
    private Vec2i originalPosition;
    private int originalDirection;

    public Cart (Vec2i pPosition, char pDirection)
    {
        position = pPosition;
        directions.index = directionChars.IndexOf(pDirection);

        originalPosition = pPosition;
        originalDirection = directions.index;
    }

    public void Reset()
    {
        position = originalPosition;
        directions.index = originalDirection;
        intersectionsCrossed = 0;
    }

    public void Move (Grid<char> pTracks)
    {
        position += directions.Current();

        //now what do we do next?
        //there are only 3 special cases:
        if (pTracks[position] == '+') HandleIntersection();
        else if (pTracks[position] == '/') HandleForwardSlashTurn();
        else if (pTracks[position] == '\\') HandleBackwardSlashTurn();
        //else just enjoy the track :)
    }

    private void HandleIntersection()
    {
        //0 is left (index += - 1), 1 is straight (index += 0), 2 is right (index += 1)
        directions.index += (intersectionsCrossed % 3) - 1;
        intersectionsCrossed++;
    }

    private void HandleForwardSlashTurn()
    {
        // How to handle /?
        // We can approach from 4 directions:
        // - moving to the right -> turn left
        // - moving to the left -> turn left
        // - moving to down -> turn right
        // - moving to up -> turn right

        if (directions.Current().X != 0) directions.index--;
        else directions.index++;
    }

    private void HandleBackwardSlashTurn()
    {
        // How to handle \?
        // We can approach from 4 directions:
        // - moving to the right -> turn right
        // - moving to the left -> turn right
        // - moving to down -> turn left
        // - moving to up -> turn left

        if (directions.Current().X != 0) directions.index++;
        else directions.index--;
    }
}

