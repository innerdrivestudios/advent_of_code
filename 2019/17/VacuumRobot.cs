//We're using integer coordinates
using System.Text;
using System.Xml.Schema;

class VacuumRobot : IIntCodeIO
{
	private StringBuilder innerBuffer = new StringBuilder();

	private const char newline = (char)10;

	// See main script
	private string movementInstructions =
        "A,A,B,C,B,A,C,B,C,A" + newline +			//MAIN
        "L,6,R,12,L,6,L,8,L,8" + newline +			//A
        "L,6,R,12,R,8,L,8" + newline +				//B
		"L,4,L,4,L,6" + newline +					//C
		"n" + newline;								//n

	private int pointer = 0;

    public long Read()
	{
		return movementInstructions[pointer++];
	}

	public void Write(long value)
	{
		if (value < 256)
		{
			innerBuffer.Append((char)value);
		}
		else
		{
            innerBuffer.Append(value);
        }
    }

	public string GetOutput()
	{
		return innerBuffer.ToString();
	}

	public void Reset()
	{
		innerBuffer.Clear();
	}

}
