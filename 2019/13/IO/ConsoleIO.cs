class ConsoleIO : IIntCodeIO
{
	public long Read()
	{
		while (true)
		{
			if (long.TryParse(Console.ReadLine(), out var value))
			{
				return value;
			}
			else
			{
				Console.WriteLine("Please enter a long...");
			}
		}
	}

	public void Write(long value)
	{
		Console.WriteLine(value);
	}
}

