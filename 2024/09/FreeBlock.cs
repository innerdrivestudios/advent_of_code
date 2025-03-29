class FreeBlock
{
    public int startIndex;
    public int endIndex;
    public int size => endIndex - startIndex + 1;

    public FreeBlock(int startIndex, int endIndex)
    {
        this.startIndex = startIndex;
        this.endIndex = endIndex;

       // Console.WriteLine($"New block created from {startIndex} to {endIndex} with size {size}");
    }
}

