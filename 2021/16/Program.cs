//Solution for https://adventofcode.com/2021/day/16 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a hexadecimal hiding as ascii packet stream (or something like that ;))

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings("").ToLower();

// ** Part 1 & 2: Set up a bit queue and fill it...
BitQueue bitQueue = new BitQueue();
                                                             
//Convert needs:
// - a string
// - the base of the string (16 == hexadecimal)
//bitQueue.WriteBits needs:
// - the number we want to write
// - how many bits we want to skip (everything except the last four bytes == 32-4)
// - how many bits we want to write (4, the hexadecimal number between 0 and 15 inclusive)
foreach (var c in myInput) bitQueue.WriteBits(Convert.ToInt32("" + c, 16), 32 - 4, 4);

// Now implement ReadPacket exactly as described in the puzzle...
long ReadPacket(ref long pTotalVersionNumberCount)
{
    //Reading the header...
    long packetVersion = bitQueue.ReadBits(3);
    pTotalVersionNumberCount += packetVersion;
    long packetType = bitQueue.ReadBits(3);

    Console.WriteLine("Packet version:"+ packetVersion);
    Console.WriteLine("Packet type:"+ packetType);

    if (packetType == 4)        // LITERAL VALUE
    {
        long literal = ReadLiteral();
        Console.WriteLine("Literal read:" + literal);
        return literal;
    }
    else // OPERATOR (specific operator ignored)
    {
        List<long> values = new List<long>();

        bool lengthTypeId = bitQueue.ReadBit();

        if (!lengthTypeId)
        {
            long subPacketLength = bitQueue.ReadBits(15);
            Console.WriteLine("Found subpackets of total length " + subPacketLength);

            int currentPointer = bitQueue.bitsRead;
            while (bitQueue.bitsRead - currentPointer != subPacketLength)
            {
                values.Add(ReadPacket(ref pTotalVersionNumberCount));
            }
        }
        else
        {
            long subPacketCount = bitQueue.ReadBits(11);    
            Console.WriteLine("Found " + subPacketCount +" subpackets to read...");

            for (int i = 0; i < subPacketCount;i++)
            {
                values.Add(ReadPacket(ref pTotalVersionNumberCount));
            }
        }

        switch (packetType)
        {
            case 0: return values.Sum();
            case 1: return values.Aggregate((x, y) => x * y);
            case 2: return values.Min();
            case 3: return values.Max();
            case 5: return values[0] > values[1] ? 1 : 0;
            case 6: return values[0] < values[1] ? 1 : 0;
            case 7: return values[0] == values[1] ? 1 : 0;
        }
    }

    throw new Exception("Should get here...");
}

long ReadLiteral()
{
    long result = 0;

    while (true)
    {
        result <<= 4;
        long readBits = bitQueue.ReadBits(5);
        result |= (readBits & 0b1111);
        if ((readBits & 0b10000) == 0) break;
    }

    return result;
}

long totalVersionNumberCount = 0;
long valueOfSubPackets = ReadPacket(ref totalVersionNumberCount);

Console.WriteLine("Part 1:" + totalVersionNumberCount);
Console.WriteLine("Part 2:" + valueOfSubPackets);