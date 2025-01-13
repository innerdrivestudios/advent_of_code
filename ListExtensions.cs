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

    public static List<List<T>> GetPermutationsIterative<T>(this List<T> pList)
    {
		List<List<T>> result = new List<List<T>>();
		result.Add(pList);

		//how many times do we have to loop? Given 1,2,3,4
		//We start by cloning 1,2,3,4 and swapping every element 1vs1, 1vs2, 1vs3, 1vs4
		//Then we take the result of that (4 permutations), and start at the 2nd element,etc
		//We can skip the last element because that is just causes a swap of the 4th vs the 4th element in every list
		for (int i = 0; i < pList.Count - 1; i++)
		{
			//we need to loop through the current amount of permutations we want to process
			//since every step that processes a permutation results in a bunch of new ones, 
			//we need to make sure the for loop doesn't take them into account yet
			int currentCount = result.Count;
			for (int j = 0; j < currentCount; j++)
			{
				//for the amount of permutations to process, we keep 'popping' one 
				//and then we process that permutation which means that for every possible swap,
				//we create a copy of our popped permutation, perform the required swap and add it back to the list for the go around
				//(performance could be improved, but this is just an example)
				List<T> listToProcess = result[0];
				result.RemoveAt(0);

				for (int k = i; k < pList.Count; k++)
				{
					List<T> newList = new List<T>(listToProcess);
					T tmp = newList[i];
					newList[i] = newList[k];
					newList[k] = tmp;
					result.Add(newList);
				}
			}
		}

		return result;
	}


}
