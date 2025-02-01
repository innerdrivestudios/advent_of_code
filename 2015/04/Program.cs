//Solution for https://adventofcode.com/2015/day/4 (Ctrl+Click in VS to follow link)

using System.Security.Cryptography;
using System.Text;

// ** Your input: Some kind of weird char sequence

// In visual studio you can modify the char sequence used by going to
// Debug/Debug Properties and changing the command line arguments.
// This value given will be passed to the built-in args[0] variable.

string myInput = args[0]; 

// ** Part 1 & 2: Generate some MD5 hash based on your char sequence that matches specific requirements

Console.WriteLine(
    "Part 1 (Find lowest number to generate a hash starting with 00000) = " +
    FindLowestNumberToGenerateAHashStartingWith(myInput, "00000")
);

Console.WriteLine(
	"Part 2 (Find lowest number to generate a hash starting with 000000) = " +
	FindLowestNumberToGenerateAHashStartingWith(myInput, "000000")
);

Console.ReadKey();

int FindLowestNumberToGenerateAHashStartingWith (string pInput, string pStartsWith)
{
    MD5 md5 = MD5.Create();

    byte[] buffer;
    int i = 0;

    do
    {
        buffer = Encoding.ASCII.GetBytes(pInput + i);
        byte[] hash = md5.ComputeHash(buffer);
        string result = Convert.ToHexString(hash);

        if (result.StartsWith(pStartsWith)) return i;
  
        i++;
    }
    while (true);
}
