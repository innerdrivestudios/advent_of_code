//Solution for https://adventofcode.com/2016/day/5 (Ctrl+Click in VS to follow link)

using System.Security.Cryptography;
using System.Text;

// In visual studio you can modify the char sequence used by going to
// Debug/Debug Properties and changing the command line arguments.
// This value given will be passed to the built-in args[0] variable.

//** Your input: some kind of weird char sequence
string myInput = args[0];

// ** Part 1: generate passwords using MD5 hashes created using
// your input and increasing integers that match certain requirements

string password = GeneratePassword(myInput);

// Use your input and an increasing integer to generate MD5 hashes
// Everytime your hash starts with 5 zeroes, take character 6 (index 5) 
// until we have 8 characters in total...
string GeneratePassword (string pInput)
{
    MD5 md5 = MD5.Create();

    byte[] buffer;
    int i = 0;

    string password = "";

    while (password.Length < 8)  
    {
        buffer = Encoding.ASCII.GetBytes(pInput + i);
        byte[] hash = md5.ComputeHash(buffer);
        string result = Convert.ToHexString(hash);

        if (result.StartsWith("00000")) { 
            password += result[5];

            Console.Clear();
            Console.WriteLine("Generating: " + password + " (" + (8-password.Length) + " chars to go)");
        }

        i++;
    }

    return password;
}

Console.Clear();
Console.WriteLine("Part 1 - Password: [" + password + "]");
Console.WriteLine("Press key to run part 2");
Console.ReadKey();

// ** Part 2 : same but different ;)

string betterPassword = GenerateBetterPassword(myInput);

// Instead of simply filling in the password from left to right,
// the hash now also indicates the position within the password to fill.
// You still look for hashes that begin with five zeroes;
// however, now, the sixth character represents the position (0-7),
// and the seventh character is the character to put in that position.

string GenerateBetterPassword(string pInput)
{
    MD5 md5 = MD5.Create();

    byte[] buffer;
    int i = 0;

    char[] password = "--------".ToCharArray();
    int itemsSolved = 0;

    while (itemsSolved < password.Length)
    {
        buffer = Encoding.ASCII.GetBytes(pInput + i);
        byte[] hash = md5.ComputeHash(buffer);
        string result = Convert.ToHexString(hash);

        if (result.StartsWith("00000"))
        {
            int passwordIndex = result[5] - '0';

            if (passwordIndex >= 0 && passwordIndex <= 7 && password[passwordIndex] == '-') {
                password[passwordIndex] = result[6];
                itemsSolved++;
                Console.Clear();
                Console.WriteLine("Generating: " + new string(password) + " (" + (8 - itemsSolved) + " chars to go)");
            }
        }

        i++;
    }

    return new string (password);
}

Console.Clear();
Console.WriteLine("Part 1 - Password: [" + password +"]");
Console.WriteLine("Part 2 - Password: [" + betterPassword +"]");
Console.ReadKey();