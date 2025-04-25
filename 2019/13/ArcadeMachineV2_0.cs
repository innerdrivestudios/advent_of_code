using Vec2i = Vec2<int>;

public class ArcadeMachineV2_0 : ArcadeMachineV1_0 
{
    private Vec2i origin = new Vec2i();
    private Vec2i scoreMarker = new Vec2i(-1,0);
    private Vec2i paddlePosition = new Vec2i();
    private Vec2i ballPosition = new Vec2i();
    
    private Grid<char> screen;
    private int score = 0;
    private bool slowMode = false;

    private Dictionary<TileType, char> screenConversionTable = new()
    {
        {TileType.Empty, ' '},
        {TileType.HorizonalPaddle, '='},
        {TileType.Ball, 'O'},
        {TileType.Wall, '+'},
        {TileType.Block, 'X'}
    };

    public ArcadeMachineV2_0 (Vec2i pMin, Vec2i pMax, bool pSlowMode = false)
    {
        Vec2i screenSize = (pMax - pMin) + new Vec2i(1, 1);
        screen = new Grid<char>(screenSize.X, screenSize.Y);
        screen.Foreach((position, value) => screen[position-pMin] = ' ');
        origin = pMin;
        slowMode = pSlowMode;
    }

    protected override void SetTile(Vec2i pPosition, int pValue)
    {
        if (pPosition == scoreMarker)
        {
            score = pValue;
            if (slowMode) Console.Beep();
        }
        else
        {
            base.SetTile(pPosition, pValue);
            screen[pPosition - origin] = screenConversionTable[(TileType)pValue];

            if ((TileType)pValue == TileType.HorizonalPaddle) paddlePosition = pPosition;
            else if ((TileType)pValue == TileType.Ball) ballPosition = pPosition;
        }

        Console.SetCursorPosition(0, 3);
        Console.WriteLine("                                  ");
        Console.SetCursorPosition(0, 3);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Part 2 - Score:" + score);

        if (pPosition != scoreMarker) Display(pPosition - origin);

        Console.SetCursorPosition(0, 0);
        if (slowMode) Thread.Sleep(10);
    }

    private void Display(Vec2i pPosition)
    {
        Console.SetCursorPosition(pPosition.X, pPosition.Y + 4);

        switch (screen[pPosition])
        {
            case '+': Console.ForegroundColor = ConsoleColor.Yellow; break;
            case 'O': Console.ForegroundColor = ConsoleColor.Cyan; break;
            case 'X': Console.ForegroundColor = ConsoleColor.Green; break;
            case '=': Console.ForegroundColor = ConsoleColor.Magenta; break;
        }

        Console.Write(screen[pPosition]);
    }

    public override long Read()
    {
        return int.Sign(ballPosition.X - paddlePosition.X);
    }

    public int GetScore() { return score; }
}
