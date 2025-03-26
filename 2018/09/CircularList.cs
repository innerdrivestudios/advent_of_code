class CircularList<T>
{
	private CircularListNode<T> current;
	private CircularListNode<T> start;
	private CircularListNode<T> end;

	public void Add (T pItem)
	{
		CircularListNode<T> newNode = new CircularListNode<T>(pItem);

		//Set up all initial fields if there is no current
		if (current == null)
		{
			start = end = current = newNode;	//  a <-- A --> a 
		}
		else 
		{
			//store whatever current was pointing at, since we inserting new after current
			var next = current.next;		
			//point current to the newNode
			current.next = newNode;
			//link up newNode to whatever current was pointing at previously
			newNode.next = next;
			//make sure the back link works as well for new node
			newNode.previous = current;
			next.previous = newNode;

			//if current was pointing at itself (eg single item), it now needs to point to newNode
			//as we are wrapping around
			if (current.previous == current) current.previous = newNode;

			//if current WAS the last node, the end is now the newNode
			if (current == end) end = newNode;
			
			//and current becomes the new node as well
			current = newNode;
			//start always point back towards the (updated) end
			start.previous = end;
		}
	}

	public void Remove()
	{
		var previous = current.previous;
		var next = current.next;
		previous.next = next;
		next.previous = previous;
		current = next;
	}

	public void PrintNode (CircularListNode<T> pNode)
	{
		Console.WriteLine("prev: " + pNode.previous.value + "\tcurrent:" + pNode.value + "\tnext:" + pNode.next.value);
	}

	public void Print ()
	{
		CircularListNode<T> iterator = start;

		do
		{
            Console.Write(iterator.value);
			if (iterator != end) Console.Write(",");
            iterator = iterator.next;
		} while (iterator != start);

        Console.WriteLine();
    }

	public void Move (int pSteps)
	{
		if (pSteps > 0) while (pSteps-- != 0) current = current.next;
		else			while (pSteps++ != 0) current = current.previous;
	}

	public T Current => current.value;
}

class CircularListNode<T>
{
	public CircularListNode<T> previous;
	public CircularListNode<T> next;
	public T value;

	public CircularListNode(T pValue)
	{
		value = pValue;
		previous = this;
		next = this;
	}
}



