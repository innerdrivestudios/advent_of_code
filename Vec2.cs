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

	public override bool Equals(object obj)
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

	public T ManhattanDistance()
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

	public Vec2<T> Mirror(Vec2<T> pMirror)
	{
		Vec2<T> delta = pMirror - this;
		Vec2<int> signedMirror = pMirror.Sign();
		//cancel out either the X or the Y and duplicated the distance
		delta.X *= T.CreateChecked(2 * signedMirror.X);
		delta.Y *= T.CreateChecked(2 * signedMirror.Y);
		return this + delta;
	}

	public void Min(Vec2<T> pOther)
	{
		X = T.Min(X, pOther.X);
		Y = T.Min(Y, pOther.Y);
	}

	public void Max(Vec2<T> pOther)
	{
		X = T.Max(X, pOther.X);
		Y = T.Max(Y, pOther.Y);
	}

	public static Vec2<T> Min(IEnumerable<Vec2<T>> pSource)
	{
		Vec2<T> min = pSource.First();
		foreach (var p in pSource) min.Min(p);
		return min;
	}

	public static Vec2<T> Max(IEnumerable<Vec2<T>> pSource)
	{
		Vec2<T> max = pSource.First();
		foreach (var p in pSource) max.Max(p);
		return max;
	}

	public T this[int index]
	{
		get
		{
			switch (index)
			{
				case 0: return X;
				case 1: return Y;
				default: throw new InvalidOperationException();
			}
		}
	}

	public T MaxAbsCoord()
	{
		return T.Max(T.Abs(X), T.Abs(Y));
	}

	public override string ToString()
	{
		return $"({X},{Y})";
	}

	// Creates two Vec2<T> that can be used with MatrixMultiply (()) to rotate a given vector.
	//
	// How does this work?
	//
	// Some math explanation...
	//
	// A coordinate (a, b) in a normal in XY axis system is basically a point
	// that is a unit steps along the x axis and y unit steps along the y axis.
	// Unit step being a step of size 1, e.g.
	//	stepping along the x axis -> (1,0)
	//	or stepping along the y axis -> (0,1)
	//
	// Another way to write this is to say that (a,b) = a * (1,0) + b * (0,1).
	// These vectors (1,0) and (0,1) are also called the basis vectors of our XY coordinate system.

	// If we want to rotate (a,b) around the origin,
	// we can rotate these basis vectors and then do R(a,b) = a * R(1,0) + b * R(0,1).
	// In other words, to rotate (a,b) to R(a,b) we can take a steps along the rotated vector R(1,0) + b steps along R(0,1).
	// 
	// Then the question becomes what are R(1,0) and R(0,1)?
	// That depends on the angle of rotation of course, but using trigonometry and the unit circle we can derive:
	// SohCahToa: Sinus is the opposite / hypothenuse, Cosine is the adjacent / hypothenuse
	// Looking at (1,0) and (0,1) rotated, this forms a triangle with hypothenuse 1
	// So let's say we rotate (1,0) over some angle... we get a triangle at point (x,y), 
	// with a hypothenuse of 1, so for x and y we can calculate:
	//
	// x = adjacent = cosine (angle) * hypothenuse = cosine (angle)
	// y = opposite = sine (angle) * hypothenuse = sinus(angle), i.e.:
	//
	// R(1,0) = (cos angle, sin angle)
	//
	// Similar for a line going up towards the y axis, but if we rotate that we have to adjust the signs:
	// 
	// R(0,1) = (-sin angle, cos angle)
	//
	// So to rotate (a,b) to R(a,b) we do a * (cos angle, sin angle) + b * (-sin angle, cos angle) 
	// R(a,b) = (a cos angle - b sin angle, a sin angle + b cos angle)
	//
	// In other words, given (a,b), if we want to ROTATE (a,b) by some degrees, we need to calculate
	// sin angle and cos angle so we can multiply each vector accordingly...
	//
	// This method returns x,y as Cos (angle), Sin (angle)
	public static Vec2<T> GetRotationVector(double pAngle, bool pInDegrees = true)
	{
		double radians = pInDegrees?double.DegreesToRadians(pAngle):pAngle;
		return new Vec2<T>((T)Convert.ChangeType(Math.Cos (radians), typeof(T)), (T)Convert.ChangeType(Math.Sin(radians), typeof(T)));
	} 

	//See GetRotationVector
	public Vec2<T> Rotate (Vec2<T> pRotationVector)
	{
		return new Vec2<T>(X * pRotationVector.X - Y * pRotationVector.Y, X * pRotationVector.Y + Y * pRotationVector.X);
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


}

