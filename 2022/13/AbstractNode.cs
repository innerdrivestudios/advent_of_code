abstract class AbstractNode : IComparable<AbstractNode>
{
	public int CompareTo(AbstractNode pOther)
	{
		if (pOther is IntListNode intListNode) return Compare(intListNode);
		else if (pOther is IntNode intNode) return Compare(intNode);
		throw new Exception("AbstractNode type not handled");
	}

	public abstract int Compare(IntListNode pOther);
	public abstract int Compare(IntNode pOther);
}

