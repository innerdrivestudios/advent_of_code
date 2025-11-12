using System.Text;

class SpringDroid : IIntCodeIO
{
	private StringBuilder innerBuffer = new StringBuilder();

	private const char newline = (char)10;


	private string program;
    private int pointer = 0;

    public void SetProgram(string pProgramInstructions)
    {
		program = pProgramInstructions.ReplaceLineEndings(""+newline);
		pointer = 0;
		Reset();
    }

    public long Read()
	{
		return program[pointer++];
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
		pointer = 0;
	}

}
