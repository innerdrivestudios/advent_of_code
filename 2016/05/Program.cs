//Solution for https://adventofcode.com/2016/day/5 (Ctrl+Click in VS to follow link)

using System.Security.Cryptography;
using System.Text;

//Your input: some kind of weird char sequence
string myInput = "wtnhxymk";

//Your task: generate passwords using MD5 hashes created using your input and increasing integers
//           that match certain requirements
string password = GeneratePassword(myInput);
string betterPassword = GenerateBetterPassword(myInput);

Console.Clear();
Console.WriteLine("Part 1 - Password: [" + password + "]");
Console.WriteLine("Part 2 - Password: [" + betterPassword +"]");
Console.ReadKey();

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
            Console.WriteLine("Password: "+ password);
        }

        i++;
    }

    return password;
}


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
                Console.WriteLine("Better password: "+ new string (password));
            }
        }

        i++;
    }

    return new string (password);
}
