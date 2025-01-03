using System.Collections.Generic;

public static class ListExtensions
{
    public static List<List<T>> GetPermutations<T>(this List<T> pList)
    {
        //If there is only 1 element in the given list, we are done
        if (pList.Count == 1)
        {
            return new List<List<T>>() { pList };
        }
        else
        {
            //For each element i in the list, get every permutations of the given list MINUS i
            //And add i back into each result... e.g. 1,2,3 -> add 1 to {2,3} & {3,2}, add 2 to {1,3} & {3,1}, etc

            List<List<T>> permutations = new List<List<T>>();

            for (int i = 0; i < pList.Count; i++)
            {
                List<T> subListToPermutate = new List<T>(pList);
                subListToPermutate.RemoveAt(i);

                List<List<T>> permutatedSubLists = GetPermutations(subListToPermutate);

                foreach (var subList in permutatedSubLists)
                {
                    subList.Add(pList[i]);
                    permutations.Add(subList);
                }

            }

            return permutations;
        }
    }

}
