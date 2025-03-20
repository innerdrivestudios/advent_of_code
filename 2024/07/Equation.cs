class Equation
{
	public long result;
	public long[] operands;

	public Equation(string equationString)
	{
		// 190: 10 19
		string[] equationParts = equationString.Split(": ");
		result = long.Parse(equationParts[0]);
		operands = equationParts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select (long.Parse).ToArray();
	}

	public bool EvaluatePart1()
	{
		int operatorCount = operands.Length-1;
		//2 operands? +/* +/* = 4
		int variations = (int)Math.Pow(2, operatorCount);

		for (int v = 0; v < variations; v++)
		{
			long total = operands[0];
			for (int i = 0; i < operatorCount; i++)
			{
				if ((v & 1 << i) == 0) 
				{ 
					total += operands[i + 1]; 
				}
				else
				{
					total *= operands[i + 1]; 
				}
			}
			if (total == result) return true;
        }
		return false;
	}

	public bool EvaluatePart2()
	{
		int operatorCount = operands.Length - 1;
		
		//+/*/|| (3) => 3 * 3 * ... 
		// 0    1    2		3		4     5			6     7     8     
		// ++   +*   +||    *+		**	  *||		||+	  ||*	||||
		int variations = (int)Math.Pow(3, operatorCount);

		for (int v = 0; v < variations; v++)
		{
			long total = operands[0];
			int testNumber = v;

			for (int i = 0; i < operatorCount; i++)
			{
				//so every value we testing needs to go 0,1,2,0,1,2

                if (testNumber%3 == 0) { total += operands[i + 1]; }
				else if (testNumber%3 == 1) { total *= operands[i + 1]; }
				else if (testNumber%3 == 2) { total = long.Parse("" + total + operands[i + 1]);}

				//in addition every next operand is basically a bit shift
				testNumber /= 3;
			}
			if (total == result) return true;
		}
		return false;
	}

}