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
}
