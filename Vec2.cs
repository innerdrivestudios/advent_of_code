using System.Numerics;

public struct Vec2<T> where T: INumber<T>
{
    public T X;
    public T Y;

    public Vec2(T pX, T pY)
    {
        X = pX; 
        Y = pY;
    }

    public static Vec2<T> operator +(Vec2<T> a, Vec2<T> b)
    {
        return new Vec2<T>(a.X + b.X, a.Y + b.Y);
    }

	public static Vec2<T> operator -(Vec2<T> a, Vec2<T> b)
	{
		return new Vec2<T>(a.X - b.X, a.Y - b.Y);
	}

	public static Vec2<T> operator /(Vec2<T> a, T b)
	{
		return new Vec2<T>(a.X/b, a.Y/b);
	}

	public static Vec2<T> operator *(Vec2<T> a, T b)
	{
		return new Vec2<T>(a.X * b, a.Y * b);
	}

	public static Vec2<T> operator *(T b, Vec2<T> a)
	{
		return new Vec2<T>(a.X * b, a.Y * b);
	}

	public static T operator *(Vec2<T> a, Vec2<T> b)
	{
		return a.X * b.X + a.Y * b.Y;
	}

	public static Vec2<T> operator -(Vec2<T> a)
	{
		return new Vec2<T>(-a.X, -a.Y);
	}

    public static bool operator ==(Vec2<T> a, Vec2<T> b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(Vec2<T> a, Vec2<T> b)
    {
        return !(a == b);
    }

    public override string ToString()
    {
        return $"({X},{Y})";
    }

	public T ManhattanDistance ()
	{
		return T.Abs(X) + T.Abs(Y);
	}
}

