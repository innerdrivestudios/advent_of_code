class Box
{
    private int boxIndex;
    private List<string> lenses = new();
    private Dictionary<string, int> focalLengths = new();

    public Box (int pBoxIndex)
    {
        boxIndex = pBoxIndex;
    }

    public void Add (string pLens, int pFocalLenght)
    {
        if (!focalLengths.ContainsKey(pLens))
        {
            lenses.Add(pLens);
        }

        focalLengths[pLens] = pFocalLenght;
    }

    public void Remove(string pLens)
    {
        focalLengths.Remove(pLens);
        lenses.Remove(pLens);
    }

    public int FocalStrength()
    {
        int total = 0;
        for (int i = 0; i < lenses.Count; i++)
        {
            total += (boxIndex + 1) * (i + 1) * focalLengths[lenses[i]];
        }
        return total;
    }
}
