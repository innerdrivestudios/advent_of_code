using Vec2i = Vec2<int>;

public class RobotIO : IIntCodeIO
{
    //Which coordinates have been mapped to which values?
    private Dictionary<Vec2i, int> mappedPanels = new ();
    //Current position...
    private Vec2i currentPosition = new Vec2i();
    //Direction helper
    private Directions<Vec2i> directions = new Directions<Vec2i>([new (0,-1), new (1,0), new (0,1), new (-1,0)]);
    //Used to figure out what we have to do with the input    
    private int currentInstruction = 0;

    public RobotIO()
    {
        Reset();
    }

    public void Reset()
    {
        mappedPanels.Clear();
        currentInstruction = 0;
        directions.index = 0;
        currentPosition= new Vec2i();
    }

    public long Read()
    {
        currentInstruction = 0;
        return GetValue();
    }

    public void Write(long pValue)
    {
        if (currentInstruction == 0)
        {
            SetValue((int)pValue);
        }
        else if (currentInstruction == 1)
        {
            //if value is 0 result is -1, if value is 1 result is 1
            directions.index += (int)(pValue * 2) - 1 ;
            currentPosition += directions.Current();
        }
        else
        {
            Console.WriteLine("Robot failure... too many instructions processed after read...");
        }

        currentInstruction++;
    }

    private int GetValue()
    {
        if (mappedPanels.ContainsKey(currentPosition)) return mappedPanels[currentPosition];

        mappedPanels[currentPosition] = 0;
        return mappedPanels[currentPosition];
    }

    public void SetValue (int pValue)
    {
        mappedPanels[currentPosition] = pValue;
    }

    public int GetPaintedPanelCount() { 
        return mappedPanels.Count; 
    }

    public Grid<char> GetPaintedPanel()
    {
        //Bit lazy but fast enough
        int minX = mappedPanels.Keys.Min (pos => pos.X);
        int minY = mappedPanels.Keys.Min (pos => pos.Y);
        int maxX = mappedPanels.Keys.Max (pos => pos.X);  
        int maxY = mappedPanels.Keys.Max (pos => pos.Y);

        Grid<char> grid = new Grid<char> (maxX-minX+1, maxY-minY+1);
        grid.Foreach((pos, value) => grid[pos] = ' ');
        
        Vec2i offset = new Vec2i (minX, minY);
        foreach (var pos in mappedPanels.Keys)
        {
            grid[pos - offset] = mappedPanels[pos] == 0?' ':'#';
        }

        return grid;
    }
    
}
