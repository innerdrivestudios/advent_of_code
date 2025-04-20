using System.Numerics;

public struct Vec3<T> where T: INumber<T>
{
    public T X;
    public T Y;
    public T Z;

    public Vec3(T pX, T pY, T pZ)
    {
        X = pX; 
        Y = pY;
		Z = pZ;
    }

    public static Vec3<T> operator +(Vec3<T> a, Vec3<T> b)
    {
        return new Vec3<T>(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

	public static Vec3<T> operator -(Vec3<T> a, Vec3<T> b)
	{
		return new Vec3<T>(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	}

	public static Vec3<T> operator /(Vec3<T> a, T b)
	{
		return new Vec3<T>(a.X/b, a.Y/b, a.Z/b);
	}

	public static Vec3<T> operator *(Vec3<T> a, T b)
	{
		return new Vec3<T>(a.X * b, a.Y * b, a.Z * b);
	}

	public static Vec3<T> operator *(T b, Vec3<T> a)
	{
		return new Vec3<T>(a.X * b, a.Y * b, a.Z * b);
	}

	public static T operator *(Vec3<T> a, Vec3<T> b)
	{
		return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
	}

	public static Vec3<T> operator -(Vec3<T> a)
	{
		return new Vec3<T>(-a.X, -a.Y, -a.Z);
	}

    public static bool operator ==(Vec3<T> a, Vec3<T> b)
    {
        return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
    }

    public static bool operator !=(Vec3<T> a, Vec3<T> b)
    {
        return !(a == b);
    }

    public override string ToString()
    {
        return $"({X},{Y},{Z})";
    }

	public T ManhattanDistance ()
	{
		return T.Abs(X) + T.Abs(Y) + T.Abs(Z);
	}

	public Vec3<T> Abs()
	{
		return new Vec3<T>(T.Abs(X), T.Abs(Y), T.Abs(Z));
	}

	public Vec3<T> Sign()
	{
		return new Vec3<T>(T.CreateChecked(T.Sign(X)), T.CreateChecked(T.Sign(Y)), T.CreateChecked(T.Sign(Z)));
	}

	public double Magnitude()
	{
		double dx = double.CreateChecked(X);
		double dy = double.CreateChecked(Y);
		double dz = double.CreateChecked(Z);
		return Math.Sqrt(dx * dx + dy * dy + dz * dz);
	}

	/*
	public Vec3<double> Normalize()
	{
		double dx = double.CreateChecked(X);
		double dy = double.CreateChecked(Y);
		double magnitude = Math.Sqrt(dx * dx + dy * dy);

		if (magnitude == 0)	return new Vec3<double>(0, 0); 
		return new Vec3<double>(dx / magnitude, dy / magnitude);
	}

	public bool ContainsWholeValues()
	{
		double deltaX = Math.Abs(int.CreateChecked(X) - double.CreateChecked(X));
		double deltaY = Math.Abs(int.CreateChecked(Y) - double.CreateChecked(Y));
		return deltaX < 0.3f && deltaY < 0.3f;
	}

	public Vec3<int> GetIntVector()
	{
		return new Vec3<int>((int)Math.Round(double.CreateChecked(X)), (int)Math.Round(double.CreateChecked(Y)));
	}
	*/
	
	public override bool Equals(object? obj)
	{
		if (obj is Vec3<T> other)
		{
			return this == other;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y, Z);
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
				_ => throw new IndexOutOfRangeException("Index must be 0, 1 or 2.")
			};
		}
		set
		{
			switch (index)
			{
				case 0: X = value; break;
				case 1: Y = value; break;
				case 2: Z = value; break;
				default: throw new IndexOutOfRangeException("Index must be 0, 1 or 2.");
			}
		}
	}


}

