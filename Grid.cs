using System.Drawing;
using Vec2i = Vec2<int>;

/// <summary>
/// Represents a generic 2D grid for storing and manipulating elements of type T.
/// </summary>
public class Grid<T>
{
    // Internal 2D array to store the grid data.
    private T[,] data;

    /// <summary>
    /// Gets the width (number of columns) of the grid.
    /// </summary>
    public int width { get; private set; } = -1;

    /// <summary>
    /// Gets the height (number of rows) of the grid.
    /// </summary>
    public int height { get; private set; } = -1;

    /// <summary>
    /// Gets the total number of elements in the grid.
    /// </summary>
    public int totalElements => width * height;

    /// <summary>
    /// Delegate for customizing the print output of grid elements.
    /// </summary>
    public delegate string PrintCallBack(Vec2i position, T content);

    /// <summary>
    /// Delegate for converting string input into elements of type T.
    /// </summary>
    public delegate T ConversionCallback(Vec2i position, string content);

    /// <summary>
    /// Initializes a new instance of the <see cref="Grid{T}"/> class with specified dimensions.
    /// </summary>
    /// <param name="pWidth">The width of the grid.</param>
    /// <param name="pHeight">The height of the grid.</param>
    public Grid(int pWidth, int pHeight)
    {
        width = pWidth;
        height = pHeight;
        data = new T[pWidth, pHeight];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Grid{T}"/> class from a formatted string.
    /// </summary>
    /// <param name="pInput">The input string representing the grid.</param>
    /// <param name="pRowToken">The delimiter for separating rows.</param>
    /// <param name="pColumnToken">The delimiter for separating columns (optional).</param>
    /// <param name="pConversionCallback">Callback for converting string data to type T (optional).</param>
    public Grid(string pInput, string pRowToken, string pColumnToken = null, ConversionCallback pConversionCallback = null)
    {
        //split the whole input into lines
        string[] lines = pInput.Split(pRowToken, StringSplitOptions.RemoveEmptyEntries);
        height = lines.Length;

        //detect whether we are using a column token and based on that the 'width' of our grid        
        bool columnTokenNotNull = !string.IsNullOrEmpty(pColumnToken);
        if (columnTokenNotNull)
        {
            //width is determined by amount of split elements
            string[] testLine = lines[0].Split(pColumnToken, StringSplitOptions.RemoveEmptyEntries);
            width = testLine.Length;
        }
        else
        {
            //width is determined by chars in first line
            width = lines[0].Length;
        }

        //now fill the actual grid
        data = new T[width, height];
        for (int y = 0; y < height; y++)
        {
            string[] columns;

            //if we have a column token, split each line on the column token, otherwise interpret the line as a char array
            if (columnTokenNotNull)
            {
                columns = lines[y].Split(pColumnToken, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                columns = lines[y].ToCharArray().Select(x => x.ToString()).ToArray();
            }

            //now store the column data, based on whether a conversion callback was provided
            //if no conversion callback was provided we use the general purpose method Convert.ChangeType
            //which can easily convert strings to int's etc
            //if a conversion callback is provided we'll simply use that to generate the data
            for (int x = 0; x < columns.Length; x++)
            {
                if (pConversionCallback == null)
                {
                    data[x, y] = (T)Convert.ChangeType(columns[x], typeof(T));
                }
                else
                {
                    data[x, y] = pConversionCallback(new Vec2i(x, y), columns[x]);
                }
            }
        }
    }

    /// <summary>
    /// Prints the grid to the console.
    /// </summary>
    /// <param name="pColumnSeparator">String separating columns in the output.</param>
    /// <param name="pRowSeparator">String separating rows in the output.</param>
    /// <param name="pPrintCallback">Callback for formatting each grid element during print (optional).</param>
    public void Print(string pColumnSeparator = " ", string pRowSeparator = "\n", PrintCallBack pPrintCallback = null)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Console.Write(
                    (
                        pPrintCallback == null ? 
                            data[x, y] : 
                            pPrintCallback(new Vec2i(x, y), data[x, y])
                    ) 
                    + 
                    pColumnSeparator
                );
            }
            Console.Write(pRowSeparator);
        }
    }

    /// <summary>
    /// Iterates over all elements in the grid and executes a callback.
    /// </summary>
    /// <param name="pContentCallBack">Action to execute for each element and its position.</param>
    public void Foreach(Action<Vec2i, T> pContentCallBack)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                pContentCallBack(new Vec2i(x, y), data[x, y]);
            }
        }
    }

    /// <summary>
    /// Iterates over all elements in the given region and executes a callback.
    /// </summary>
    /// <param name="pTopLeft">The top left point of the region to iterate over</param>
    /// <param name="pWidthHeight">The width and height of the region to iterate over</param>
    /// <param name="pContentCallBack">Action to execute for each element and its position.</param>
    public void ForeachRegion(Vec2i pTopLeft, Vec2i pWidthHeight, Action<Vec2i, T> pContentCallBack)
	{
        ForeachRegion (pTopLeft.X, pTopLeft.Y, pTopLeft.X + pWidthHeight.X, pTopLeft.Y + pWidthHeight.Y, pContentCallBack);
	}

    /// <summary>
    /// Iterates over all elements in the given region and executes a callback.
    /// </summary>
    /// <param name="pBounds">The region to iterate over</param>
    public void ForeachRegion(Rectangle pBounds, Action<Vec2i, T> pContentCallBack)
    {
        ForeachRegion(pBounds.Left, pBounds.Top, pBounds.Right, pBounds.Bottom, pContentCallBack);
    }

    /// <summary>
    /// Iterates over all elements in the given region and executes a callback.
    /// </summary>
    /// <param name="pLeft">The left point in the region to copy</param>
    /// <param name="pTop">The top point in the region to copy</param>
    /// <param name="pRight">The right point in the region to copy</param>
    /// <param name="pBottom">The bottom point in the region to copy</param>
    /// 
    /// <param name="pContentCallBack">Action to execute for each element and its position.</param>

    public void ForeachRegion(int pLeft, int pTop, int pRight, int pBottom, Action<Vec2i, T> pContentCallBack)
    {
        for (int y = pTop; y < pBottom; y++)
        {
            for (int x = pLeft; x < pRight; x++)
            {
                pContentCallBack(new Vec2i(x, y), data[x, y]);
            }
        }
    }

    /// <summary>
    /// Gets or sets the element at the specified (x, y) coordinates.
    /// </summary>
    public T this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
            {
                throw new IndexOutOfRangeException($"Coordinates out of bounds {x},{y}.");
            }
            return data[x, y];
        }
        set
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
            {
                throw new IndexOutOfRangeException($"Coordinates out of bounds {x},{y}.");
            }
            data[x, y] = value;
        }
    }

    /// <summary>
    /// Gets or sets the element using a 1D index, calculated as (x, y).
    /// </summary>
    public T this[int index]
    {
        get
        {
            int x = index % width;
            int y = index / width;

            return this[x, y];
        }
        set
        {
            int x = index % width;
            int y = index / width;

            this[x, y] = value;
        }
    }

    /// <summary>
    /// Gets or sets the element using a Vec2i object for (x, y) coordinates.
    /// </summary>
    public T this[Vec2i index]
    {
        get => this[index.X, index.Y];
        set => this[index.X, index.Y] = value;
    }

    /// <summary>
    /// Checks if the specified Vec2i position is within the grid boundaries.
    /// </summary>
    /// <param name="index">The position to check.</param>
    /// <returns>True if inside, otherwise false.</returns>
    public bool IsInside(Vec2i index)
    {
        return IsInside(index.X, index.Y);
    }

    /// <summary>
    /// Checks if the specified (x, y) coordinates are within the grid boundaries.
    /// </summary>
    public bool IsInside(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

	/// <summary>
	/// Creates a deep copy of the grid's internal data.
	/// </summary>
	/// <returns>A new 2D array with the same contents.</returns>
	public Grid<T> Clone()
	{
		Grid<T> clone = new Grid<T>(width, height);
		clone.data = (T[,])data.Clone();

		return clone;
	}

	/// <summary>
	/// Return a copy of the given region of the current grid.
	/// <param name="pTopLeft">The top left point in the region to copy</param>
	/// <param name="pWidthHeight">The width and height of the region to copy</param>
	/// </summary>
	public Grid<T> Clone(Vec2i pTopLeft, Vec2i pWidthHeight)
	{
        Grid<T> clone = new Grid<T> (pWidthHeight.X, pWidthHeight.Y);

		for (int y = 0; y < pWidthHeight.Y; y++)
		{
			for (int x = 0; x < pWidthHeight.X; x++)
			{
				clone[x, y] = data[pTopLeft.X + x, pTopLeft.Y + y];
			}
		}

        return clone;
	}

    public HashSet<Vec2i> FloodFill(Vec2i pStart, Predicate<Vec2i> pIncludeWhen)
    {
        HashSet<Vec2i> visited = new HashSet<Vec2i>();
        Vec2i[] directions = [new(1, 0), new(-1, 0), new(0, 1), new(0, -1)];

        Queue<Vec2i> todoList = new Queue<Vec2i>();
        todoList.Enqueue(pStart);
        visited.Add(pStart);

        while (todoList.Count > 0)
        {
            Vec2i current = todoList.Dequeue();

            foreach (Vec2i direction in directions)
            {
                Vec2i nextPosition = current + direction;

                if (IsInside(nextPosition) && !visited.Contains(nextPosition) && pIncludeWhen(nextPosition))
                {
                    todoList.Enqueue(nextPosition);
                    visited.Add(nextPosition);
                }
            }
        }

        return visited;
    }

    public List<HashSet<Vec2i>> GetRegions (HashSet<T> pValuesToInclude) 
    {
        HashSet<Vec2i> visited = new();
        List<HashSet<Vec2i>> regions = new ();

        Foreach(
            (position, value) =>
            {
                if (pValuesToInclude.Contains(value) && !visited.Contains(position))
                {
                    HashSet<Vec2i> filledPositions = FloodFill(position, x => pValuesToInclude.Contains(this[x]));
                    visited.UnionWith(filledPositions);
                    regions.Add(filledPositions);
                }
            }
        );

        return regions;
    }

    // By default (when printing for example), X is to the right, Y is down, so rotate (1) is a clockwise rotation.
    // This performs an IN place rotation of all grid elements (and yes that was hard ;)).
    // Also it didn't end up very readable or pretty.
    //
    // In hindsight I should have hardcoded the different cases (90 vs 180) with
    // loop unrolling to avoid the awkward scaling - center + center etc.
    //
    // Anyway it was a good exercise and example of how not to do it, but it works now.
    
	public void Rotate (int pXtoYRotations)
	{
        if (width != height) throw new Exception("Can only rotate square grids");
        if (pXtoYRotations % 4 == 0) return;

        // the easiest way to deal with even/odd grid sizes is to scale everything if the center
        // falls between to integer values... so if even: scale by 2, if odd, ok we can process that:
        bool evenSize = (width % 2) == 0;
        int scale = evenSize ? 2 : 1;

        Vec2i center = new Vec2i(width-1, height-1) / (2 / scale);
        Vec2i zeroOffset = new Vec2i(0, 0);

        Vec2i rotationVector = Vec2i.GetRotationVector(90 * pXtoYRotations);

        //If we are rotating by 90 degrees, we have 4 quadrants to rotate,
        //if we are rotating by 180 degrees, we have 2 halves to rotate
        bool oneEighty = pXtoYRotations % 2 == 0;
        int sectors = oneEighty ? 2 : 4;

        int widthToRotate = (width-1)/2;
        int heightToRotate = oneEighty ? (height-1) : (height-1)/2;

        //if we are odd sized, we rotate the upper left quadrant, but avoid rotating the center cross twice
        //this is really hard to understand without drawing a 4x4 and 5x5 on paper twice,
        //once for 90 degrees, once for 180 degrees and note which indices we need to skip
        if (!evenSize && sectors == 4) heightToRotate--;

        for (int x = 0; x <= widthToRotate; x++)
        {			
            for (int y = 0; y <= heightToRotate; y++)
            {
                //for odd sized 180 degree rotations, the restrictions are even more
                if (!evenSize && sectors == 2 && x == widthToRotate && y >= x) continue;

                //always skip the center
				Vec2i currentCoordOffset = new Vec2i(x, y) * scale - center;
                if (currentCoordOffset == zeroOffset) continue;

				T currentValue = this[(center + currentCoordOffset) / scale];

                // In place rotation 2 or 4 times based on 90 or 180 degree angles, 
                // where we keep the replaced value to use as the next value
				for (int i = 0; i < sectors; i++)
                {
                    Vec2i newCoordOffset = currentCoordOffset.Rotate(rotationVector);
                    Vec2i newCoord = (center + newCoordOffset) / scale;
					T newContent = this[newCoord];
                    this[newCoord] = currentValue;
                    currentValue = newContent;
                    currentCoordOffset = newCoordOffset;
				}
            }
        }
	}

	public void FlipHorizontal()
	{
		int w = width;
		int h = height;

		for (int y = 0; y < h; y++)
		{
			for (int x = 0; x < w / 2; x++)
			{
				int rx = w - 1 - x;

				var a = new Vec2i(x, y);
				var b = new Vec2i(rx, y);

				T tmp = this[a];
				this[a] = this[b];
				this[b] = tmp;
			}
		}
	}

	public void FlipVertical()
	{
		int w = width;
		int h = height;

		for (int y = 0; y < h / 2; y++)
		{
			int ry = h - 1 - y;

			for (int x = 0; x < w; x++)
			{
				var a = new Vec2i(x, y);
				var b = new Vec2i(x, ry);

				T tmp = this[a];
				this[a] = this[b];
				this[b] = tmp;
			}
		}
	}

    public Grid<T> Duplicate (int pXCopies, int pYCopies)
    {
        Grid<T> duplicate = new Grid<T>(width * pXCopies, height * pYCopies);

        for (int x = 0; x < pXCopies;  x++)
        {
            for (int y = 0;y < pYCopies; y++)
            {
                this.Foreach(
                    (pos, value) =>
                    {
                        duplicate[pos.X + width * x, pos.Y + y * height] = value;
                    }
                );
            }
        }

        return duplicate;
    }

}
