using Vec2i = Vec2<int>;

class DroneInfoProvider : IIntCodeIO
{
	public bool beingPulled = false;

	private Vec2i coordinateToTest = new Vec2i();
	private int coordinateIndex = 0;

    public long Read()
	{
		return coordinateToTest[coordinateIndex++];
	}

	public void Write(long value)
	{
		beingPulled = value == 1;
    }

	public void SetCoordinateToTest (Vec2i pCoordinate)
	{
		coordinateToTest = pCoordinate;
		coordinateIndex = 0;
	}

}
