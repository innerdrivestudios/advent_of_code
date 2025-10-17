using Vec2i = Vec2<int>;

class Node
{
    public int size;
    public int used;
    public int avail => size - used;
    public readonly Vec2i position;

    public Node(int pSize, int pUsed, Vec2i pPosition)
    {
        size = pSize;
        used = pUsed;
        position = pPosition;

        //avail = pAvail;
    }

    public override string ToString()
    {
        return $"{used}/{size}";
    }
}
