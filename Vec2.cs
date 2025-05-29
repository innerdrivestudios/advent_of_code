using System.Numerics;

public struct Vec2<T> where T : INumber<T>
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

	public Vec2<T> Abs()
	{
		return new Vec2<T>(T.Abs(X), T.Abs(Y));
	}

    public Vec2<int> Sign()
    {
        return new Vec2<int>(T.Sign(X), T.Sign(Y));
    }

    public Vec2<T> Scale(Vec2<T> pOther)
    {
        return new Vec2<T>(X * pOther.X, Y * pOther.Y);
    }


    public double Magnitude()
	{
		double dx = double.CreateChecked(X);
		double dy = double.CreateChecked(Y);
		return Math.Sqrt(dx * dx + dy * dy);
	}

	/*
	public Vec2<double> Normalize()
	{
		double dx = double.CreateChecked(X);
		double dy = double.CreateChecked(Y);
		double magnitude = Math.Sqrt(dx * dx + dy * dy);

		if (magnitude == 0)	return new Vec2<double>(0, 0); 
		return new Vec2<double>(dx / magnitude, dy / magnitude);
	}

	public bool ContainsWholeValues()
	{
		double deltaX = Math.Abs(int.CreateChecked(X) - double.CreateChecked(X));
		double deltaY = Math.Abs(int.CreateChecked(Y) - double.CreateChecked(Y));
		return deltaX < 0.3f && deltaY < 0.3f;
	}

	public Vec2<int> GetIntVector()
	{
		return new Vec2<int>((int)Math.Round(double.CreateChecked(X)), (int)Math.Round(double.CreateChecked(Y)));
	}
	*/
	
	public override bool Equals(object? obj)
	{
		if (obj is Vec2<T> other)
		{
			return this == other;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y);
	}

	public Vec2<T> Mirror (Vec2<T> pMirror)
	{
		Vec2<T> delta = pMirror - this;
		Vec2<int> signedMirror = pMirror.Sign();
		//cancel out either the X or the Y and duplicated the distance
		delta.X *= T.CreateChecked(2 * signedMirror.X);
		delta.Y *= T.CreateChecked(2 * signedMirror.Y);
		return this + delta;
	}
	
}

