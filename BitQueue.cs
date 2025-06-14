/// <summary>
/// A helper class for writing bits into a queue and reading them back sequentially.
/// Supports writing individual bits or a group of bits, and reading bits into integral values.
/// </summary>
class BitQueue
{
    // Constant to avoid magic number usage for byte size
    private const int BYTE_SIZE = 8; 

    // Internal byte list used to store written bits.
    private List<byte> backingStore = new();

    // Tracks the number of bits written to the queue so far.
    private int bitsWritten = 0;

    // Tracks the number of bits read from the queue so far.
    public int bitsRead { get; private set; } = 0;

    /// <summary>
    /// Writes a single bit to the queue.
    /// </summary>
    /// <param name="pValue">The bit value to write: true for 1, false for 0.</param>
    /// 
    public void WriteBit(bool pValue)
    {
        if ((bitsWritten / BYTE_SIZE) + 1 > backingStore.Count)
            backingStore.Add(0);

        int bitIndex = 7 - (bitsWritten % BYTE_SIZE);
        backingStore[^1] |= (byte)((pValue ? 1 : 0) << bitIndex);
        bitsWritten++;
    }

    /// <summary>
    /// Writes a sequence of bits from an integer value into the queue.
    /// </summary>
    /// <param name="pBitValuesAsInt">An integer whose bits will be written.</param>
    /// <param name="pStartIndex">The bit index (0–31) to start reading from (most significant bit first).</param>
    /// <param name="pBitsToWrite">The number of bits to write starting from <paramref name="pStartIndex"/>.</param>
    /// <remarks>
    /// - This method reads from the most significant bits downward.
    /// - There is no bounds checking, eg WriteBits (0, 50, 20) will crash.
    /// </remarks>
    public void WriteBits(int pBitValuesAsInt, int pStartIndex, int pBitsToWrite)
    {
        //While we have bits to write
        //Take our bitvalueAsInt, compare it with 1 shifted by 31 - where we started
        //(ever increasing in every loop, e.g. we want to take the next bit and the next bit and etc)
        while (pBitsToWrite-- > 0) WriteBit((pBitValuesAsInt & (1 << (31 - pStartIndex++))) > 0);
    }

    /// <summary>
    /// Checks whether there are unread bits remaining in the queue.
    /// </summary>
    /// <returns>True if there are more bits to read; otherwise, false.</returns>
    public bool HasBits()
    {
        return bitsRead < bitsWritten;
    }

    /// <summary>
    /// Reads the next bit from the queue.
    /// </summary>
    /// <returns>True if the bit is 1; false if the bit is 0.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown if attempting to read past the available written bits.
    /// </exception>
    public bool ReadBit()
    {
        int byteIndex = bitsRead / BYTE_SIZE;
        int bitToRead = 7 - (bitsRead % BYTE_SIZE);

        bool value = (backingStore[byteIndex] & (1 << bitToRead)) > 0;
        bitsRead++;
        return value;
    }

    /// <summary>
    /// Reads a group of bits from the queue and returns them as a long.
    /// </summary>
    /// <param name="pBitCount">The number of bits to read (1 to 64).</param>
    /// <returns>The bits interpreted as an unsigned long integer.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if more than 64 bits are requested.
    /// </exception>
    public long ReadBits(int pBitCount)
    {
        if (pBitCount > 64)
            throw new ArgumentException("Can only read a max of 64 bits at a time");

        long result = 0;
        while (pBitCount-- > 0)
        {
            //Shift the result left while adding the result of the next bit
            result <<= 1;
            result |= (ReadBit() ? 1L : 0L);
        }

        return result;
    }

    /// <summary>
    /// Prints the internal bit buffer as a binary string to the console.
    /// For debugging and visual inspection.
    /// </summary>
    public void Print()
    {
        foreach (byte b in backingStore)
        {
            Console.Write(Convert.ToString(b, 2).PadLeft(8, '0'));
        }
        Console.WriteLine();
    }
}
