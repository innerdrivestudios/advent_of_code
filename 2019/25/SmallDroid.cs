using System.Text;

class SmallDroid : IIntCodeIO
{
	private StringBuilder innerBuffer = new StringBuilder();

	private const char newline = (char)10;

	private string program;
    private int pointer = 0;

	// Commands for the initial setup to pick up all pickup-able items (see grid.xlsx)
	private int currentCommandIndex = 0;
	private string[] commands =
	[
		"south", "take boulder", 
		"west", "take asterisk",
		"east",	"east",	"take food ration",
		"west",	"north", "east", "take candy cane",
		"north", "east", "north", "take mug", 
		"south", "west", "north", "take mutex",
		"north", "take prime number",
		"south", "south", "south", "east", "north", "take loom",
		"south", "east", "south", "east", "east", "inv"
	];

	// For the testing mode we have a set of default commands to use:
	private string[] items = [
			"boulder", "asterisk", "food ration", "candy cane", "mug", "mutex", "prime number", "loom"
		];

	// Queue we'll use to store commands to pick up and drop items
	private Queue<string> doorTestingCommands = new();
	// 8 items so 256 configurations to test
	private int configurationToTest = 0;

	public long Read()
	{
		//We are on auto play :)

		if (program == null || pointer >= program.Length)
		{
			pointer = 0;
			program = GetCommand() + newline;
            Console.WriteLine(program);
			//Console.ReadKey();
		}

		return program[pointer++];
	}

	private string GetCommand()
	{
		//return Console.ReadLine();
		if (currentCommandIndex < commands.Length) return commands[currentCommandIndex++];

		//Else we are in door testing mode!
		if (doorTestingCommands.Count == 0)
		{
			GenerateNextTestCommands();
		}
		
		return doorTestingCommands.Dequeue();
	}

	private void GenerateNextTestCommands()
	{
		doorTestingCommands.Clear();
		configurationToTest++;
        Console.WriteLine(configurationToTest);

		for (int i = 0; i < items.Length; i++)
		{

			doorTestingCommands.Enqueue(
				(((configurationToTest & (1 << i)) > 0) ? "take" : "drop") + " " + items[i]
			);
		}

		// Try to open the door...
		doorTestingCommands.Enqueue("north");
	}

	public void Write(long value)
	{
		if (value == newline)
		{
			string output = innerBuffer.ToString();
			innerBuffer.Clear();
			Console.WriteLine(output);

			return;
		}

		if (value < 256)
		{
			innerBuffer.Append((char)value);
		}
		else
		{
            innerBuffer.Append(value);
        }
    }

}
