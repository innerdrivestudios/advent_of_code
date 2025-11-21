public class NetworkInterface : IIntCodeIO
{
	public readonly int networkID;

	public static readonly List<NetworkInterface> network = new();

	private enum State { Booting, Booted};
	private State state = State.Booting;

	private Queue<long> inputQueue = new ();
	private Queue<long> outputQueue = new ();


	public bool hasEnded { get; private set; } = false;
	public bool isIdle => inputQueue.Count == 0;

	private static Queue<long> natMemory = new();
	private static int natPacketReceivedCount = 0;

	public NetworkInterface()
	{
		networkID = network.Count;
		network.Add(this);
	}

	public long Read()
	{
		if (state == State.Booting) { 
			state = State.Booted;
			return networkID;
		}

		if (inputQueue.Count == 0)
		{
			return -1;
		}
		else
		{
			return inputQueue.Dequeue();
		}
	}

	public void Write(long pValue)
	{
		outputQueue.Enqueue(pValue);

		if (outputQueue.Count == 3)
		{
			int receiverId = (int)outputQueue.Dequeue();

			if (receiverId == 255)
			{

				/** // Part 1

				outputQueue.Dequeue();
				Console.WriteLine("Part 1: " + .Dequeue());
				hasEnded = true;

				/**/

				natMemory.Clear();
				long y;
				natMemory.Enqueue(outputQueue.Dequeue());
				natMemory.Enqueue(y = outputQueue.Dequeue());
				natPacketReceivedCount++;

				if (natPacketReceivedCount == 1)
				Console.WriteLine($"Part 1: " + y);
			}
			else
			{
				NetworkInterface receiver = network[receiverId];
				receiver.Queue(outputQueue.Dequeue());
				receiver.Queue(outputQueue.Dequeue());
			}
		}
	}

	public void Queue (long pValue)
	{
		inputQueue.Enqueue(pValue);
	}

	public static bool HasNatMemory()
	{
		return natMemory.Count > 0;
	}

	public (long, long) ConsumeNatMemory()
	{
		long valueX = natMemory.Dequeue();
		long valueY = natMemory.Dequeue();

		inputQueue.Enqueue (valueX);
		inputQueue.Enqueue (valueY);

		return (valueX, valueY);
	}
}

