// Solution for https://adventofcode.com/2019/day/4 (Ctrl+Click in VS to follow link)

// Your input: a range of numbers

int[] myInput = "152085-670283".Split("-").Select (int.Parse).ToArray();

// Part 1 - Interpreting the giving range of numbers as passwords, deduct how many of them are valid
// according to the following rules:

// - It is a six - digit number.
// - The value is within the range given in your puzzle input.
// - Two adjacent digits are the same (like 22 in 122345).
// - Going from left to right, the digits never decrease;
//   they only ever increase or stay the same (like 111123 or 135679).

int validPasswordCount = 0;

for (int i = myInput[0]; i <= myInput[1]; i++) {
    validPasswordCount += IsValidPart1(i) ? 1 : 0;
}

bool IsValidPart1 (int pPassword)
{
	string password = pPassword.ToString();
	if (password.Length != 6) return false;

	int same = 0;

	for (int i = 0; i < password.Length-1; i++)
	{
		int delta = password[i+1] - password[i];
		if (delta == 0) same++;
		else if (delta < 0) return false;
	}

	return same > 0;
}

Console.WriteLine("Part 1 - Valid password count: " + validPasswordCount);

// Part 2 - Same for an additional rule, there have to be EXACTLY two matching digits

validPasswordCount = 0;

for (int i = myInput[0]; i <= myInput[1]; i++)
{
	validPasswordCount += IsValidPart2(i) ? 1 : 0;
}

// This could probably also be solved easier with regular expressions?

bool IsValidPart2(int pPassword)
{
	string password = pPassword.ToString();
	if (password.Length != 6) return false;

	int same = 0;
	bool runLengthFound = false;

	for (int i = 0; i < password.Length - 1; i++)
	{
		int delta = password[i + 1] - password[i];

		if (delta == 0)
		{
			same++;
		}
		else if (delta > 0)
		{
			// got 1!
			if (same == 1)
			{
				runLengthFound = true;
			}
			else //reset for the next try
			{
				same = 0;
			}
		}
		else if (delta < 0)
		{
			return false;
		}
	}

	//last same == 1 is a bit of a dirty hack, solves one last edge case at the last i
	return runLengthFound || same == 1;
}

Console.WriteLine("Part 2 - Valid password count: " + validPasswordCount);