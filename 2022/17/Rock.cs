using System.Collections;
using Vec2i = Vec2<int>;

class Rock : IEnumerable<Vec2i>
{
	//A rock consists of pieces with a local offset vs the rock's origin
	private HashSet<Vec2i> localOffsets = new();		

	//But a rock also has a "world position"
	private Vec2i _position = new Vec2i();
	public Vec2i position { 
		get
		{
			return _position;		
		} 

		private set
		{
			_position = value;
			//And everytime the rock world position is updated,
			//we calculate and cache the world position for each *piece*
			UpdateGlobals();
		}
	}

	private HashSet<Vec2i> globalPositions = new();	//localOffsets + position
	
	//Auto calculated once in the constructor based on the local offsets...
	public int width { get; private set; }


	// The local offsets use an X=right, Y=up coordinate system,
	// with no negatives (ie don't specify negative offsets, and make sure
	// the left most and bottom most piece have an x=0, y=0 position respectively,
	// we don't verify or check this!)
	public Rock (HashSet<Vec2i> pLocalOffsets)
	{
		localOffsets = pLocalOffsets;

		//e.g. 0,0 1,0 2,0 => width = 3
		width = localOffsets.Max (x => x.X) + 1;
	}

	public Rock Clone()
	{
		//Create a clone based on the local offsets, not these are shared amongst all rock 
		//instances and should not be modified!
		return new Rock(localOffsets);
	}

	public bool Contains (Vec2i pPoint)
	{
		return globalPositions.Contains(pPoint);
	}

	public void SetPosition(Vec2i pPosition)
	{
		position = pPosition;
	}

	private void UpdateGlobals()
	{
		globalPositions.Clear(); 
		foreach (Vec2i local in localOffsets)
		{
			globalPositions.Add (local + _position);
		}
	}

	public void Move (Vec2i pOffset)
	{
		position += pOffset;
	}

    public IEnumerator<Vec2i> GetEnumerator()
    {
        return globalPositions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return globalPositions.GetEnumerator();
    }
}

