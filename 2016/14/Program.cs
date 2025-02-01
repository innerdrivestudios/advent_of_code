//Solution for https://adventofcode.com/2016/day/14 (Ctrl+Click in VS to follow link)

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

// ** Your input: Some kind of weird char sequence

// In visual studio you can modify the char sequence used by going to
// Debug/Debug Properties and changing the command line arguments.
// This value given will be passed to the built-in args[0] variable.

string myInput = args[0];

// ** Part 1 :
//
// To generate keys, you first get a stream of random data by taking the MD5 of a pre-arranged salt
// (your puzzle input) and an increasing integer index (starting with 0, and represented in decimal);
// the resulting MD5 hash should be represented as a string of lowercase hexadecimal digits.
//
// However, not all of these MD5 hashes are keys, and you need 64 new keys for your one-time pad.
//
// A hash is a key only if:
//
// It contains three of the same character in a row, like 777.
// Only consider the first such triplet in a hash.
// One of the next 1000 hashes in the stream contains that same character five times in a row, like 77777.
// Considering future hashes for five-of-a-kind sequences does not cause those hashes to be skipped;
// instead, regardless of whether the current hash is a key, always resume testing for keys starting
// with the very next hash.

// Given the actual salt in your puzzle input, what index produces your 64th one-time pad key?

// Approach:
// Tried different approach actually, but the only valid way seems to actually generate a 1000 hashes
// per keyindex and then increasing the key index per valid hash. I tried optimizing this by storing
// all indices of triple chars so we only have to generate hashes for each keyIndex once, but since
// later lookaheads might make earlier entries valid, I couldn't find the actual lowest keyIndex using this approach.
// (This probably makes no sense whatsoever unless you tried it yourself).

// Anyway current approach:

/// <summary>
/// Finds the index of the key in a one-time pad sequence that meets the required criteria.
/// </summary>
/// 
/// <param name="pRequiredCorrectKeyCount">The number of correct keys needed before returning the index.</param>
/// <param name="pMaxDistance">The maximum distance within which a key should have a matching five-character sequence.</param>
/// <param name="pHashFunction">A hash function that takes an index and a list of previously computed hashes and returns a hash string.</param>
/// <returns>The index of the key that satisfies the required correct key count condition.</returns>

int GetOneTimePadKeyIndex (int pRequiredCorrectKeyCount, int pMaxDistance, Func<int, List<string>, string> pHashFunction)
{
    int currentCorrectKeyCount = 0;
    int keyIndex = 0;

    //We use a local hashCache to speed up the hash calculation
    List<string> hashCache = new ();

    //Match any character that repeats itself 2 times more
    Regex matchThree = new Regex(@"(.)\1{2}");

    while (true)
    {
        //Calculates a new hash or returns it from the cache
        string result = pHashFunction(keyIndex, hashCache);
        Match matchThreeResult = matchThree.Match(result);

        if (matchThreeResult.Success)
        {
            char found = matchThreeResult.Value[0];

            //Match the character found 5 times
            Regex matchFive = new Regex("(" + found + ")\\1{4}");

            //If we succeed within the distance, mark it this key as a correct one
            //(note that this generate a lot of unnecessary tests)
            for (int i = 1; i <= pMaxDistance; i++)
            {
                //Calculates a new hash or returns it from the cache to speed up the calculations
                string nextResult = pHashFunction(keyIndex + i, hashCache);

                if (matchFive.IsMatch(nextResult))
                {
                    Console.Write(".");
                    
                    currentCorrectKeyCount++;
                    if (currentCorrectKeyCount == pRequiredCorrectKeyCount) return keyIndex;

                    //Done checking
                    break;
                }
            }
        }
        keyIndex++;
    }
}

string GetHash(int pIndex, List<string> pHashCache)
{
    if (pIndex < pHashCache.Count) return pHashCache[pIndex];

    byte[] buffer = Encoding.ASCII.GetBytes(myInput + pIndex);
    byte[] hash = MD5.HashData(buffer);
    string result = Convert.ToHexString(hash).ToLower();
    pHashCache.Add(result);

    return result;
}

int requiredCorrectKeyCount = 64;
int maxDistance = 1000;

//Stopwatch stopwatch = Stopwatch.StartNew();

Console.WriteLine(
    $"Part 1 - {requiredCorrectKeyCount} keys found at key index:" +
    GetOneTimePadKeyIndex(requiredCorrectKeyCount, maxDistance, GetHash)
);

//Console.WriteLine("In " + stopwatch.ElapsedMilliseconds + " milliseconds.");

// ** Part 2: Exact same process, but using a different "stretched" hash function

string GetStretchedHash(int pIndex, List<string> pHashCache)
{
    if (pIndex < pHashCache.Count) return pHashCache[pIndex];

    byte[] buffer = Encoding.ASCII.GetBytes(myInput + pIndex);
    byte[] hash = MD5.HashData(buffer);
    string result = Convert.ToHexString(hash).ToLower();

    //unfortunately we can't just hash bytes, since everything need to be a lower case string
    for (int i = 0; i < 2016; i++)
    {
        hash = MD5.HashData(Encoding.ASCII.GetBytes(result));
        result = Convert.ToHexString(hash).ToLower();
    }

    pHashCache.Add(result);

    return result;
}

//stopwatch.Restart();

Console.WriteLine(
    $"Part 2 - {requiredCorrectKeyCount} keys found at key index:" +
    GetOneTimePadKeyIndex(requiredCorrectKeyCount, maxDistance, GetStretchedHash)
);

//Console.WriteLine("In " + stopwatch.ElapsedMilliseconds + " milliseconds.");

