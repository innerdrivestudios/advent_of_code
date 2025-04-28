// Solution for https://adventofcode.com/2022/day/13 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

(IntListNode left, IntListNode right)[] pairs = File.ReadAllText(args[0])
    .ReplaceLineEndings(Environment.NewLine)
    .Split (Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
    .Chunk(2)
    .Select (x => (new IntListNode(x[0]), new IntListNode(x[1])))
    .ToArray();

// ** Part 1: Check which packets are in the right order...

int totalId = 0;

for (int i = 0; i < pairs.Length; i++)
{
    var pair = pairs[i];
    //Console.WriteLine("LEFT :" + pair.left.ToString());
    //Console.WriteLine("RIGHT:" + pair.right.ToString());

    bool correct = pair.left.Compare(pair.right) < 0;
    if (correct) totalId += (i+1);

	//Console.WriteLine("LEFT SMALLER THAN RIGHT?"+ correct);
    //Console.WriteLine();
}

Console.WriteLine("Part 1:" + totalId);

// ** Part 2: Put all packets in the correct order:

List<IntListNode> packets = new ();
foreach (var pair in pairs)
{
    packets.Add(pair.left);
    packets.Add (pair.right);
}

IntListNode decoderKey1 = new IntListNode("[[2]]");
IntListNode decoderKey2 = new IntListNode("[[6]]");
packets.Add(decoderKey1);
packets.Add(decoderKey2);

packets.Sort();

Console.WriteLine("Part 2:" + (packets.IndexOf(decoderKey1)+1) * (packets.IndexOf(decoderKey2) + 1));