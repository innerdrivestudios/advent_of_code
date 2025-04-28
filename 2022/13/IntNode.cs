using System.Text;

class IntNode : AbstractNode
{
    static StringBuilder number = new StringBuilder();

    public readonly int value;

    public IntNode(StringReader pParseStream)
    {
        number.Clear();

        while (char.IsAsciiDigit((char)pParseStream.Peek())) number.Append((char)pParseStream.Read());
        value = int.Parse(number.ToString());
    }

    public AbstractNode PromoteToList()
    {
        return new IntListNode(this);
    }

	public override string ToString()
	{
		return value.ToString();
	}

	public override int Compare(IntListNode pOther)
	{
        return PromoteToList().Compare(pOther);
	}

	public override int Compare(IntNode pOther)
	{
		return value.CompareTo(pOther.value);
	}

}
