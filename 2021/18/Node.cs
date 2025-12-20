class Node
{
    public Node parent = null;          
    public Node leftNode = null;
    public Node rightNode = null;

    public int value = 0;

    public enum NodeType { Nested, Value, Invalid };

    public NodeType nodeType
    {
        get
        {
            // If left and right are both null, we are a value node, if either is value but not the other, we have issues...
            return (leftNode == null && rightNode == null) ?
                NodeType.Value :
                (leftNode != null && rightNode != null) ? NodeType.Nested : NodeType.Invalid;
        }
    }

    public bool Explode(int pDepth = 0)
    {
        // If we represent a nested node with two value children above depth 4, we need to explode...
        if (pDepth >= 4 && leftNode != null && leftNode.nodeType == NodeType.Value && rightNode != null && rightNode.nodeType == NodeType.Value)
        {
            Node leftValueNode = FindLeftValueNode();
            Node rightValueNode = FindRightValueNode();

            if (leftValueNode != null) leftValueNode.value += leftNode.value;
            if (rightValueNode != null) rightValueNode.value += rightNode.value;

            // Reset ourselves to a value node with value zero...

            value = 0;
            leftNode = null;
            rightNode = null;

            // We exploded, so stop searching ...
            return true;
        }

        // If we did not explode yet, try left first and then right...
        // Note how the setup will only execute right explode if no left explode has triggered
        return (leftNode != null && leftNode.Explode(pDepth+1)) || (rightNode != null && rightNode.Explode(pDepth+1));
    }


    private Node FindLeftValueNode()
    {
        // These kind of methods are hard to understand without drawings, but basically,
        // if we want the VALUE node next to us, we first traverse upward as long as we are the left branch in our parent
        Node current = this;
        while (current.parent != null && current.parent.leftNode == current) current = current.parent;

        // as soon as we are not, we are the right branch and we take our PARENTS left branch ONCE
        if (current.parent != null) current = current.parent.leftNode; else return null;

        // then seek downward to find a value node...
        while (current.nodeType != NodeType.Value) current = current.rightNode;

        return current;
    }

    private Node FindRightValueNode()
    {
        // Same as above but then flipped...
        Node current = this;
        while (current.parent != null && current.parent.rightNode == current) current = current.parent;
        if (current.parent != null) current = current.parent.rightNode; else return null;
        while (current.nodeType != NodeType.Value) current = current.leftNode;
        return current;
    }

    // Splitting is easier...

    public bool Split()
    {
        // If we are a value node...
        if (nodeType == NodeType.Value)
        {
            //... check if we need to be split

            if (value < 10) return false;

            // Execute the split if needed:
            int leftValue = (int) Math.Floor(value / 2f);
            int rightValue = (int) Math.Ceiling(value / 2f);

            leftNode = new Node();
            leftNode.value = leftValue;
            leftNode.parent = this;

            rightNode = new Node();
            rightNode.value = rightValue;
            rightNode.parent = this;

            return true;
        }
        else // check if our children need to be split ...
        {
            return leftNode.Split() || rightNode.Split();
        }
    }

    // Return a new node that represents adding this + pOther
    // Note that this causes this and pOther to become part of a larger tree
    public Node AddNode (Node pOther)
    {
        // Create the new node with myself and the other as children
        Node newNode = new Node();
        newNode.leftNode = this;
        newNode.rightNode = pOther;

        // Correct the parents

        this.parent = newNode;
        pOther.parent = newNode;

        // Reduce before returning !

        while (newNode.Explode() || newNode.Split()) { }

        return newNode;
    }

    public long Magnitude ()
    {
        return nodeType == NodeType.Value ? value : ( 3L * leftNode.Magnitude() + 2L * rightNode.Magnitude() );
    }

    // Helper method for debugging ...
    public override string ToString()
    {
        if (nodeType == NodeType.Value) return value.ToString();
        return "[" + leftNode.ToString() + "," + rightNode.ToString() + "]";
    }

}
