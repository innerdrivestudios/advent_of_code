//Solution for https://adventofcode.com/2015/day/15 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

//Your input: a list of ingredients with certain properties
string myInput = "Frosting: capacity 4, durability -2, flavor 0, texture 0, calories 5\r\nCandy: capacity 0, durability 5, flavor -1, texture 0, calories 8\r\nButterscotch: capacity -1, durability 0, flavor 5, texture 0, calories 6\r\nSugar: capacity 0, durability 0, flavor -2, texture 2, calories 1\r\n";

//Your task: combine a total of 100 "scoops" of the given ingredients to make the best cookie ever

//Step 1. Convert the given input to a list of <property values per cookie> (stored as an int []).
//Note that the name of the ingredient/property completely doesn't matter,
//only the values provided and they are always provided in the same order

List<int[]> ingredients = new List<int[]>();
ConvertInput(ingredients);

//Step 2. Now that we got all the data calculate the highest scores

Console.WriteLine("Part 1 - Highest score possible:"+ GetHighestScore(ingredients));
Console.WriteLine("Part 2 - Highest score possible with 500 calories:"+ GetHighestScore(ingredients, pRequiredCalories:500));

void ConvertInput(List<int[]> pIngredients)
{
	Regex regex = new Regex(@"\w+: \w+ (-?\d), \w+ (-?\d), \w+ (-?\d), \w+ (-?\d), \w+ (-?\d)\r\n");
	MatchCollection matches = regex.Matches(myInput);

	foreach (Match match in matches)
	{
		//group count also include the whole string (as first index)
		int[] ingredient = new int[match.Groups.Count - 1];
		for (int i = 0; i < ingredient.Length; i++)
		{
			ingredient[i] = int.Parse(match.Groups[i + 1].Value);
		}
		pIngredients.Add(ingredient);
	}
}

//Calculate the highest score possible for any combination of 100 teaspoons of all ingredients using a recursive approach
int GetHighestScore(List<int[]> pIngredients, int[] pTeaspoonsCounts = null, int pIngredientIndex = 0, int pRequiredCalories = -1)
{
	//We need to keep track of how much of each ingredient we've added so far to know our future options
    //At the start all of these values are zero...
	pTeaspoonsCounts = pTeaspoonsCounts ?? new int[pIngredients.Count];

    //We also need to know the total amount of ingredients added since the final total needs to consist of 100 teaspoons...
	int totalTeaspoonsSoFar = pTeaspoonsCounts.Sum();
    int leftOverTeaspoons = 100 - totalTeaspoonsSoFar;

    //for each ingredient except the last
    //(which is fully determined by totalTeaspoonsSoFar since the total amount of spoons needs to be a 100)
    //we copy the list of spoon counts that we already had, and iterate over all the possible values for the current ingredient
    //recursively doing the same for the next ingredient
    if (pIngredientIndex < pIngredients.Count - 1)
	{
		int bestScoreSoFar = 0;

        //Evaluate all options for the current ingredient
		for (int i = 0; i < leftOverTeaspoons; i++)
		{
			//Clone the current amounts
			int[] amountsCopy = new int[pTeaspoonsCounts.Length];
			Array.Copy(pTeaspoonsCounts, amountsCopy, amountsCopy.Length);

			//Set the value for the ingredient we are currently working with
			amountsCopy[pIngredientIndex] = i;

			//Get the recursive score by evaluating what we have so far plus the options for the next ingredients
			int score = GetHighestScore(pIngredients, amountsCopy, pIngredientIndex + 1, pRequiredCalories);
			bestScoreSoFar = Math.Max(bestScoreSoFar, score);
		}

		return bestScoreSoFar;
	}
    //If we have reached the last ingredient...
    else 
	{
        //... for which its amount is fully determined by all amounts before it...
        pTeaspoonsCounts[pTeaspoonsCounts.Length - 1] = leftOverTeaspoons;

		return GetDivisionScore(pIngredients, pTeaspoonsCounts, pRequiredCalories);
	}
}

int GetDivisionScore (List<int[]> pIngredients, int[] pAmounts = null, int pRequiredCalories = -1)
{
    //now we have all amounts, what is the score of this division of amount over the ingredients?
    //we need to calculate two things:
    // - a score which is based on multiplying property scores
    // - a calorie count which is based on simply adding the calorie values over the different ingredients

    int score = 1;
    int calories = 0;

	int propertyCount = pIngredients[0].Length;

    //for each property
    for (int i = 0; i < propertyCount; i++)
    {
        int propertyScore = 0;

        //calculate how high this property score over all ingredients, based on the amount of the ingredient used
        //times the property value for that ingredient
        for (int j = 0; j < pAmounts.Length; j++)
        {
            propertyScore += pAmounts[j] * pIngredients[j][i];
        }

        //what do we do with that propertyScore?
        //If it is part of the overall score of the cookie, use it to update the score value
        if (i < propertyCount - 1)
        {
            propertyScore = Math.Max(0, propertyScore);
            score *= propertyScore;
        }
        else //if the property describes the calorie count...
        {
            calories = propertyScore;
        }
    }

    //Now we have two values a score and a calorie count
    //Return the score based on the given values...

    if (pRequiredCalories > 0)
    {
        return calories == pRequiredCalories ? score : 0;
    }
    else
    {
        return score;
    }

}

