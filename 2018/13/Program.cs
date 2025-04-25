//Solution for https://adventofcode.com/2018/day/13 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of plants and their replacement rules

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings(Environment.NewLine);

// First create the track and carts

List<Cart> carts = new List<Cart>();
Grid<char> railroadTracks = new Grid<char>(myInput, Environment.NewLine, null, ProcessTrack);

char ProcessTrack (Vec2i pPosition, string pContent)
{
    char content = pContent[0];

    if (content == '^' || content == 'v')
    {
        //Create the cart, overwrite the contents with right type of track
        carts.Add(new Cart(pPosition, content));
        content = '|';
    }
    else if (content == '<' || content == '>')
    {
        //Create the cart, overwrite the contents with right type of track
        carts.Add(new Cart(pPosition, content));
        content = '-';
    }

    return content;
}

Console.WriteLine(carts.Count + " carts found...");

// Now we have all carts... time to simulate them driving down the track
// and tracking their collisions! 

// Let's define some helper methods first:

int SortAccordingToPosition(Cart pCartA, Cart pCartB)
{
    if (pCartA.position.Y == pCartB.position.Y) return pCartA.position.X.CompareTo(pCartB.position.X);
    else return pCartA.position.Y.CompareTo(pCartB.position.Y);
}

Vec2i SimulateTrackMovementUntilCollision()
{
    //Start with all the current positions...
    HashSet<Vec2i> cartPositions = carts.Select(x => x.position).ToHashSet();

    while (true)
    {
        carts.Sort(SortAccordingToPosition);

        foreach (Cart cart in carts)
        {
            //Update the position for each cart, checking if a collision happens
            cartPositions.Remove(cart.position);
            //Move the cart and add its new position back into the cartPositions list
            //If that fails, we have a collision...
            cart.Move(railroadTracks);
            if (!cartPositions.Add(cart.position)) return cart.position;
        }
    }
}

Console.WriteLine("Part 1: " + SimulateTrackMovementUntilCollision());

// ** Part 2: What is the location of the last cart at the end of the first
// tick where it is the only cart left?

foreach (Cart cart in carts) cart.Reset();

Vec2i SimulateTrackMovementUntilThereIsOneCartLeft()
{
    //Start with all the current positions mapped to each cart ...
    Dictionary<Vec2i, Cart> cartPositions2Cart = carts.ToDictionary(x => x.position);
    HashSet<Cart> stillInTheRace = new HashSet<Cart>(carts);

    while (true)
    {
        carts.Sort(SortAccordingToPosition);

        foreach (Cart cart in carts)
        {
            if (!stillInTheRace.Contains(cart)) continue;

            Vec2i oldPosition = cart.position;
            cart.Move(railroadTracks);

            if (cartPositions2Cart.ContainsKey(cart.position))
            {
                //Remove the cart at the new position
                stillInTheRace.Remove(cartPositions2Cart[cart.position]);
                //Remove ourselves
                stillInTheRace.Remove(cart);
                //Remove both of them from the dictionary as well
                cartPositions2Cart.Remove(oldPosition);
                cartPositions2Cart.Remove(cart.position);
                Console.WriteLine("Removed 2 carts, " + stillInTheRace.Count + " carts left.");
            }
            else
            {
                cartPositions2Cart.Remove(oldPosition);
                cartPositions2Cart.Add(cart.position, cart);
            }
        }
    
        //update the list of carts for the next round
        if (stillInTheRace.Count == 1) return stillInTheRace.First().position;
        else if (carts.Count != stillInTheRace.Count) carts = stillInTheRace.ToList();
    }
}

Console.WriteLine("Part 2: " + SimulateTrackMovementUntilThereIsOneCartLeft());
