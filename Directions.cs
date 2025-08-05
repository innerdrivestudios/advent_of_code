//A helper class to easily cycle through different objects...

class Directions<T> {
	public readonly List<T> directions;

	private int _index = 0;

	public Directions(T[] pDirections) { 
		directions = new (pDirections);
	}

	public int index
	{
		get { return _index; }
		set
		{
			_index = WrapValue(value);
		}
	}

	public T Next()
	{
		index++;
		return directions[index];
	}

	public T Previous()
	{
		index--;
		return directions[index];
	}

	public T Get(int index)
	{
		return directions[WrapValue(index)];
	}

	public T Current()
	{
		return directions[_index];
	}

	private int WrapValue (int value)
	{
		return ((value % directions.Count) + directions.Count) % directions.Count;
	}
	
}