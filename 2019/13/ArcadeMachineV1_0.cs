using Vec2i = Vec2<int>;

public enum TileType { Empty = 0, Wall = 1, Block = 2, HorizonalPaddle = 3, Ball = 4};

public class ArcadeMachineV1_0 : IIntCodeIO
{
    private int multiplexer = 0;

    Dictionary<Vec2i, TileType> screenValues = new ();
    Vec2i tmp = new Vec2i();

    Vec2i min = new Vec2i(int.MaxValue, int.MaxValue);
    Vec2i max = new Vec2i(int.MinValue, int.MinValue);


    public ArcadeMachineV1_0()
    {
    }

    public virtual long Read()
    {
        throw new NotImplementedException();
    }

    public void Write(long pValue)
    {
        if (multiplexer == 0)
        {
            tmp.X = (int)pValue;
            min.X = int.Min(min.X, tmp.X);
            max.X = int.Max(max.X, tmp.X);
        }
        else if (multiplexer == 1)
        {
            tmp.Y = (int)pValue;
            min.Y = int.Min(min.Y, tmp.Y);
            max.Y = int.Max(max.Y, tmp.Y);
        }
        else if (multiplexer == 2) SetTile(tmp, (int)pValue);

        multiplexer = (multiplexer + 1) % 3;
    }

    virtual protected void SetTile (Vec2i pPosition, int pValue)
    {
        screenValues[tmp] = (TileType)pValue;
    }

    public int GetTileCount(TileType pTileType)
    {
        return screenValues.Count(x => x.Value == pTileType);
    }

    public (Vec2i, Vec2i) GetBounds()
    {
        return (min, max);
    }
}
