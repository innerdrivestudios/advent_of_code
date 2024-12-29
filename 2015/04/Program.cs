//Solution for https://adventofcode.com/2015/day/4 (Ctrl+Click in VS to follow link)

using System.Security.Cryptography;
using System.Text;

//Your input: some kind of weird char sequence
string myInput = "bgvyzdsv";

//Your task: generate some MD5 hash based on your char sequence that matches specific requirement

Part1_FindAHashStartingWith00000 (myInput);
Part2_FindAHashStartingWith000000(myInput);
Console.ReadKey();

void Part1_FindAHashStartingWith00000(string pInput)
{
    MD5 md5 = MD5.Create();

    byte[] buffer;
    int i = 0;

    do
    {
        buffer = Encoding.ASCII.GetBytes(pInput + i);
        byte[] hash = md5.ComputeHash(buffer);
        string result = Convert.ToHexString(hash);

        if (result.StartsWith("00000"))
        {
            Console.WriteLine("Part 1:"+i);
            break;
        }
  
        i++;
    }
    while (true);
}

void Part2_FindAHashStartingWith000000(string pInput)
{
    MD5 md5 = MD5.Create();

    byte[] buffer;
    int i = 0;

    do
    {
        buffer = Encoding.ASCII.GetBytes(pInput + i);
        byte[] hash = md5.ComputeHash(buffer);
        string result = Convert.ToHexString(hash);

        if (result.StartsWith("000000"))
        {
            Console.WriteLine("Part 2:" + i);
            break;
        }

        i++;
    }
    while (true);
}
