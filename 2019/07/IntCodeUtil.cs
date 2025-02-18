static class IntCodeUtil
{
	static public int GetValue(int[] pProgram, int pIndex, int pParameterMode)
	{
		if (pParameterMode == 0)
		{
			return pProgram[pProgram[pIndex]];
		}
		else if (pParameterMode == 1)
		{
			return pProgram[pIndex];
		}

		throw new InvalidDataException("Invalid parameter mode: " + pParameterMode);
	}
}


