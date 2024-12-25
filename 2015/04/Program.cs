//Solution for https://adventofcode.com/2015/day/4 (Ctrl+Click in VS to follow link)
using System.Security.Cryptography;
using System.Text;

MD5 md5 = MD5.Create();

string myInput = "bgvyzdsv";

Part1(myInput);
Part2(myInput);
Console.ReadKey();

void Part1(string pInput)
{
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

void Part2(string pInput)
{
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
