/**
 * Basic tree node class that allows easy up and down traversal of its children.
 */
class TreeNode
{
	public readonly Dictionary<string, TreeNode> children = new();
	public TreeNode parent { get; private set; }	

	public string name { get; private set; } 
	public int size { get; private set; }
	public bool isDirectory => children.Count > 0;

	private static TreeNode root = null;

	public static TreeNode GetRoot ()
	{
		if (root == null) root = new TreeNode("/");
		return root;
	}

	private TreeNode (string pName, int pSize = 0)
	{
		name = pName;
		size = pSize;
	}

	public TreeNode CreateNode(string pName, int pSize = 0)
	{
		TreeNode childNode = new TreeNode(pName, pSize);
		children[pName] = childNode;
		childNode.parent = this;
		return childNode;
	}

	public TreeNode GetNode(string pName)
	{
		if (pName == "/")
		{
			return root;
		}
		else if (pName == "..")
		{
			return parent;
		}
		else
		{
			return children[pName];
		}
	}

	// To calculate the directory sizes, we'll need to sum the size of all of its children recursively
	public int CalculateDirectorySizes()
	{
		if (isDirectory)
		{
			int childSize = 0;
			
			foreach (var child in children.Values)
			{
				childSize += child.CalculateDirectorySizes();
			}

			//Store the size of all our children as our size...
			size = childSize;

			return childSize;
		}
		else
		{
			// Simply return our own size
			return size;
		}
	}

	public int TotalSumOfAllDirectoriesBelow (int pCutOffValue)
	{
		int totalSum = 0;

		if (isDirectory && size <= pCutOffValue) totalSum += size;

		foreach (var child in children.Values)
		{
			if (child.isDirectory) //Small optimization, we can also leave it out...
			{
                totalSum += child.TotalSumOfAllDirectoriesBelow(pCutOffValue);
			}
		}

		return totalSum;
	}

	public TreeNode FindSmallestDirectorySized(int pSize)
	{
		if (!isDirectory) return null;

		TreeNode smallestNodeAbove = null;
			
		// First check if any child directory (which is always smaller than us)
		// can match the requested size ...
		foreach (var child in children.Values)
		{ 
			if (!child.isDirectory) continue;
			
			// Get the smallest matching directory amongst our children
			TreeNode smallestMatchingChildNode = 
				child.FindSmallestDirectorySized(pSize);

			// None found? 
			if (smallestMatchingChildNode == null) continue;

			// One found?
			if (
				smallestNodeAbove == null || 
				smallestMatchingChildNode.size < smallestNodeAbove.size
				)
			{
				smallestNodeAbove = smallestMatchingChildNode;
 			}
		}

		// If our size is bigger than the requested size and we haven't found 
		// a smaller child node yet...
		if (size >= pSize && smallestNodeAbove == null) smallestNodeAbove = this;
		
		return smallestNodeAbove;
	}

	public void Print()
	{
		InnerPrint(0);
	}

	private void InnerPrint(int pDepth)
	{
		Console.WriteLine(new String('\t', pDepth) + name + "(" + size + ")");

		foreach (TreeNode childNode in children.Values)
		{
			childNode.InnerPrint(pDepth + 1);
		}
	}

}
