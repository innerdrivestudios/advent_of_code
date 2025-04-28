class IntListNode : AbstractNode 
{
    public readonly List<AbstractNode> children = new List<AbstractNode>();

    public IntListNode (IntNode pChild)
    {
        children.Add(pChild);
    }

    public IntListNode (string pInput) : this (new StringReader(pInput.Replace(" ","")))
    {
    }

    public IntListNode (StringReader pParseStream)
    {
        if (pParseStream.Read() != '[') throw new InvalidDataException("[ expected!");

        char lookAhead = (char)pParseStream.Peek();

        while (lookAhead != ']')
        {
            do
            {
                if (lookAhead == '[')
                {
                    IntListNode childNode = new IntListNode(pParseStream);
                    children.Add(childNode);
                }
                else if (char.IsAsciiDigit(lookAhead))
                {
                    IntNode childNode = new IntNode(pParseStream);
                    children.Add(childNode);
                }

                lookAhead = (char)pParseStream.Peek();
                if (lookAhead == ',') pParseStream.Read(); else break;

            } while (true);

        }

        if (pParseStream.Read() != ']') throw new InvalidDataException("] expected!");

    }

	public override string ToString()
	{
		return "[" + string.Join (",", children) + "]";
	}

	public override int Compare(IntListNode pOther)
	{
        int maxElements = int.Min(children.Count, pOther.children.Count);

        for (int i = 0; i < maxElements; i++) { 
            int result = children[i].CompareTo(pOther.children[i]);
            if (result != 0) return result; 
        }

        return children.Count - pOther.children.Count; 
	}

	public override int Compare(IntNode pOther)
	{
        return CompareTo(pOther.PromoteToList());
	}

}
