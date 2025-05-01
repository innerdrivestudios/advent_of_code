// This is the full KnotHash algorithm from day 10 back from the dead
// (for more information see https://adventofcode.com/2017/day/10)

static class KnotHashLib
{
    private static int[] standardList = Enumerable.Range(0, 256).ToArray();

    public static void Test()
    {
        KnotHash(null, null);
    }

    private static void Swap(int[] pInputList, int pIndexA, int pIndexB)
    {
        //wrap both elements around the end of the list
        pIndexA %= pInputList.Length;
        pIndexB %= pInputList.Length;

        //basic swap
        int tmp = pInputList[pIndexA];
        pInputList[pIndexA] = pInputList[pIndexB];
        pInputList[pIndexB] = tmp;
    }

    private static void Reverse(int[] pInputList, int pReverseLength, int pStartIndex)
    {
        for (int i = 0; i < pReverseLength / 2; i++)
        {
            Swap(pInputList, i + pStartIndex, pStartIndex + pReverseLength - i - 1);
        }
    }

    private static void KnotHash(int[] pInputList, int[] pInputLengths, int pRepeatCount = 1)
    {
        int skipSize = 0;
        int currentPosition = 0;

        for (int i = 0; i < pRepeatCount; i++)
        {
            foreach (int inputLength in pInputLengths)
            {
                if (inputLength > 1 && inputLength <= pInputList.Length)
                {
                    Reverse(pInputList, inputLength, currentPosition);
                }

                currentPosition += inputLength + skipSize;
                skipSize++;
            }
        }
    }

    public static string KnotHash (string pInput)
    {
        int[] standardListClone = (int[])standardList.Clone();
        int[] convertedList = pInput.Trim().Select(x => (int)x).Concat([17, 31, 73, 47, 23]).ToArray();
        KnotHash(standardListClone, convertedList, 64);

        int[][] chunks = standardListClone.Chunk(16).ToArray();

        string hashString = "";
        for (int i = 0; i < chunks.Length; i++)
        {
            hashString += chunks[i].Aggregate((x, y) => x ^ y).ToString("x2"); //note the 2 (!!!) ($@#^$@(#!!)
        }

        return hashString;
    }

}

