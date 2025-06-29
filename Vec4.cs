using System.Numerics;

public struct Vec4<T> where T: INumber<T>
{
    public T X;
    public T Y;
    public T Z;
    public T W;

    public Vec4(T pX, T pY, T pZ, T pW)
    {
        X = pX; 
        Y = pY;
		Z = pZ;
		W = pW;
    }

    public static Vec4<T> operator +(Vec4<T> a, Vec4<T> b)
    {
        return new Vec4<T>(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
    }

	public static Vec4<T> operator -(Vec4<T> a, Vec4<T> b)
	{
		return new Vec4<T>(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
	}

	public static Vec4<T> operator /(Vec4<T> a, T b)
	{
		return new Vec4<T>(a.X/b, a.Y/b, a.Z/b, a.W/b);
	}

	public static Vec4<T> operator *(Vec4<T> a, T b)
	{
		return new Vec4<T>(a.X * b, a.Y * b, a.Z * b, a.W * b);
	}

	public static Vec4<T> operator *(T b, Vec4<T> a)
	{
		return new Vec4<T>(a.X * b, a.Y * b, a.Z * b, a.W * b);
	}

	public static T operator *(Vec4<T> a, Vec4<T> b)
	{
		return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
	}

	public static Vec4<T> operator -(Vec4<T> a)
	{
		return new Vec4<T>(-a.X, -a.Y, -a.Z, -a.W);
	}

    public static bool operator ==(Vec4<T> a, Vec4<T> b)
    {
        return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
    }

    public static bool operator !=(Vec4<T> a, Vec4<T> b)
    {
        return !(a == b);
    }

    public override string ToString()
    {
        return $"({X},{Y},{Z},{W})";
    }

	public T ManhattanDistance ()
	{
		return T.Abs(X) + T.Abs(Y) + T.Abs(Z) + T.Abs(W);
	}

	public Vec4<T> Abs()
	{
		return new Vec4<T>(T.Abs(X), T.Abs(Y), T.Abs(Z), T.Abs(W));
	}

	public Vec4<T> Sign()
	{
		return new Vec4<T>(
			T.CreateChecked(T.Sign(X)), 
			T.CreateChecked(T.Sign(Y)), 
			T.CreateChecked(T.Sign(Z)),
			T.CreateChecked(T.Sign(W))
		);
	}

	public double Magnitude()
	{
		double dx = double.CreateChecked(X);
		double dy = double.CreateChecked(Y);
		double dz = double.CreateChecked(Z);
		double dw = double.CreateChecked(W);
		return Math.Sqrt(dx * dx + dy * dy + dz * dz + dw * dw);
	}

	/*
	public Vec4<double> Normalize()
	{
		double dx = double.CreateChecked(X);
		double dy = double.CreateChecked(Y);
		double magnitude = Math.Sqrt(dx * dx + dy * dy);

		if (magnitude == 0)	return new Vec4<double>(0, 0); 
		return new Vec4<double>(dx / magnitude, dy / magnitude);
	}

	public bool ContainsWholeValues()
	{
		double deltaX = Math.Abs(int.CreateChecked(X) - double.CreateChecked(X));
		double deltaY = Math.Abs(int.CreateChecked(Y) - double.CreateChecked(Y));
		return deltaX < 0.3f && deltaY < 0.3f;
	}

	public Vec4<int> GetIntVector()
	{
		return new Vec4<int>((int)Math.Round(double.CreateChecked(X)), (int)Math.Round(double.CreateChecked(Y)));
	}
	*/
	
	public override bool Equals(object? obj)
	{
		if (obj is Vec4<T> other)
		{
			return this == other;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y, Z,W);
	}

	public T this[int index]
	{
		get
		{
			return index switch
			{
				0 => X,
				1 => Y,
				2 => Z,
				3 => W,
				_ => throw new IndexOutOfRangeException("Index must be 0, 1, 2 or 3.")
			};
		}
		set
		{
			switch (index)
			{
				case 0: X = value; break;
				case 1: Y = value; break;
				case 2: Z = value; break;
				case 3: W = value; break;
				default: throw new IndexOutOfRangeException("Index must be 0, 1, 2, 3.");
			}
		}
	}


}

