//Solution for https://adventofcode.com/2016/day/9 (Ctrl+Click in VS to follow link)

using System.Text;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a compressed string, where (numberXnumber) indicates which piece of text after it 
//                is compressed/needs to be decompressed

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings("");

/*
    For example for Part I:

    ADVENT              contains no markers and decompresses to itself with no changes, 
                        resulting in a decompressed length of 6.
    A(1x5)BC            repeats only the B a total of 5 times, becoming ABBBBBC for a decompressed length of 7.
    (3x3)XYZ            becomes XYZXYZXYZ for a decompressed length of 9.
    A(2x2)BCD(2x2)EFG   doubles the BC and EF, becoming ABCBCDEFEFG for a decompressed length of 11.
    (6x1)(1x3)A         simply becomes (1x3)A - the (1x3) looks like a marker, 
                        but because it's within a data section of another marker, 
                        it is not treated any differently from the A that comes after it. 
                        It has a decompressed length of 6.
    X(8x2)(3x3)ABCY     becomes X(3x3)ABC(3x3)ABCY (for a decompressed length of 18), 
                        because the decompressed data from the (8x2) marker (the (3x3)ABC) 
                        is skipped and not processed further.

    Console.WriteLine(Decompress("ADVENT"));
    Console.WriteLine(Decompress("A(1x5)BC"));
    Console.WriteLine(Decompress("(3x3)XYZ"));
    Console.WriteLine(Decompress("A(2x2)BCD(2x2)EFG"));
    Console.WriteLine(Decompress("(6x1)(1x3)A"));
    Console.WriteLine(Decompress("X(8x2)(3x3)ABCY"));
*/

// ** Your task: calculate the LENGTH of the expansion

Console.WriteLine("Part 1 - Decompressed length:" + Decompress(myInput).Length);

//Initial version, which calculates the length based on the actual decompressed string
string Decompress(string pInput)
{
    StringBuilder sb = new StringBuilder();

    for (int i = 0; i < pInput.Length; i++)
    {
        //any non () part should just be appended to the end result
        if (pInput[i] != '(') sb.Append(pInput[i]);
        else
        {
            //if we encountered a (, we look up ), parse everything in between, so we know
            //what to repeat and how many times and then we perform those repetitions
            int rightIndex = pInput.IndexOf(')', i);
            string compressionMarker = pInput.Substring(i + 1, rightIndex - (i + 1));
            string[] markerParts = compressionMarker.Split('x');
            int captureLength = int.Parse(markerParts[0]);
            int repetitionCount =  int.Parse(markerParts[1]);

            string stringToRepeat = pInput.Substring(rightIndex + 1, captureLength);

            for (int j = 0; j < repetitionCount; j++) {
                sb.Append(stringToRepeat);
            }

            //Move the 'cursor' until after the captured part (don't add 1, the for loop does that)
            i = rightIndex+captureLength;
        }
    }

    return sb.ToString();
}

// ** Part 2: Now the decompression process is not allowed to skip decompressed parts.... so...
//
// To calculate the decompressed length of X(8x2)(3x3)ABCY we...
// 1. Count X (=1)
// 2. Now we encounted (8x2), so we need to calculate 2 * the decompressed length of (3x3)ABC
//    + the length of everything after it (which is Y)
// 3. Decompressed length of (3x3)ABC is 3 * the decompressed length of ABC (which is simply ABC)
// 4. So now we have 1 + 2 * 3 * ABC + length of Y = 1 = 20

Console.WriteLine("Part 2 - Decompressed length (no skip):" + CalculateDecompressedLength(myInput));

//In code:
long CalculateDecompressedLength(string pInput)
{
    long decompressedLength = 0;

    for (int i = 0; i < pInput.Length; i++)
    {
        //For every char not ( just count it as 1
        if (pInput[i] != '(') decompressedLength += 1;
        else
        {
            //if we find ( we search for ), deduct the capture part and repetition count
            //recursively get the decompressed length of the capture part and add it to our count
            int rightIndex = pInput.IndexOf(')', i);
            string compressionMarker = pInput.Substring(i + 1, rightIndex - (i + 1));
            string[] markerParts = compressionMarker.Split('x');
            int captureLength = int.Parse(markerParts[0]);
            int repetitionCount = int.Parse(markerParts[1]);

            string stringToRepeat = pInput.Substring(rightIndex + 1, captureLength);

            decompressedLength += repetitionCount * CalculateDecompressedLength(stringToRepeat);

            //Move the 'cursor' until after the captured part (don't add 1, the for loop does that)
            i = rightIndex + captureLength;
        }
    }

    return decompressedLength;
}
