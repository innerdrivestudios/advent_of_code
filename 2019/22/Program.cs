// Solution for https://adventofcode.com/2019/day/22 (Ctrl+Click in VS to follow link)

using System.Numerics;
using System.Text.RegularExpressions;

// ** Part 1: Let's first define the basic data and a bunch of helper methods...

int numberOfCardsPart1 = 10007;

// To do all the swapping efficiently, I'll define two buffers,
// which I'll swap while using them as a "front" and "back" buffer, 
// to avoid re-allocation.
//
// (Note added later: no at this point, I hadn't seen part 2 yet :) :.)

int[] source = new int[numberOfCardsPart1];
int[] buffer = new int[numberOfCardsPart1];

// First define all the helper methods to:
// - Fill the card list
// - Deal the card list into a new stack
// - Cut the card list
// - Increment the card list

void FillCardList()
{
    for (int i = 0; i < numberOfCardsPart1; i++)
    {
        source[i] = i;
    }
}

FillCardList();

// For testing:
// Console.WriteLine(string.Join (" ", source));

void DealIntoNewStack ()
{
    for (int i = 0; i < numberOfCardsPart1; i++)
    {
        buffer[numberOfCardsPart1 - 1 - i] = source[i];
    }

    int[] tmp = source;
    source = buffer;
    buffer = tmp;
}

// For testing:
// DealIntoNewStack();
// Console.WriteLine(string.Join(" ", source));

void CutNCards (long pN)
{
    pN = Wrap (numberOfCardsPart1 + pN, numberOfCardsPart1);

    for (int i = 0; i < pN; i++)
    {
        buffer[numberOfCardsPart1 - pN + i] = source[i];
    }

    for (int i = 0; i < numberOfCardsPart1 - pN; i++)
    {
        buffer[i] = source[pN + i];
    }

    int[] tmp = source;
    source = buffer;
    buffer = tmp;
}

// For testing:
// CutNCards(-4);
// Console.WriteLine(string.Join(" ", source));

void DealWithIncrement (int pIncrement)
{
    for (long i = 0, j = 0; i < numberOfCardsPart1; i++, j = Wrap (j + pIncrement, numberOfCardsPart1))
    {
        buffer[j] = source[i];            
    }

    int[] tmp = source;
    source = buffer;
    buffer = tmp;
}

long Wrap (long pNumber, long pModulo)
{
    return ((pNumber % pModulo) + pModulo) % pModulo;
}

// ** Ok, now that we got all methods, let's parse the input and apply it to our deck!

// (In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable)

// ** Your input: a bunch of card deck cut instructions

string[] myInput = File.ReadAllLines(args[0]);

Regex cutRegex = new Regex(@"cut (-?\d+)");
Regex dealRegex = new Regex(@"deal with increment (\d+)");

void ApplyAllGivenDeckOperations ()
{
    foreach (string input in myInput)
    {
        if (input == "deal into new stack")
        {
            DealIntoNewStack();
        }
        else
        {
            Match cutMatch = cutRegex.Match(input);
            if (cutMatch.Success)
            {
                CutNCards(int.Parse(cutMatch.Groups[1].Value));
            }
            else
            {
                Match dealMatch = dealRegex.Match(input);
                if (dealMatch.Success)
                {
                    DealWithIncrement(int.Parse(dealMatch.Groups[1].Value));
                }
            }
        }

        //Console.WriteLine(string.Join(" ", source));
    }
}

ApplyAllGivenDeckOperations();

Console.WriteLine("Part 1:" + source.ToList().IndexOf(2019));

// ** Part 2: ...
// ...
// ..
// WHAT DA ...

// Anyway, unfortunately for part 2 we need to apply our instructions
// a ridiculous amount of times, to a ridiculously large deck!
// In other words, within seconds, it became clear that using an "optimized" approach with
// a front and a back buffer is a laughable attempt in the face of this new challenge.
// There is no way we can ever create a deck that large,
// let alone apply the deck operations to it that amount of times.

// At this point I did a lot of research/experiments and I have no idea whether the 
// final approach is the best but it works well enough...

// The first thing I tested was whether there was a recurring pattern in applying 
// the given deck shuffles: for the "small" deck of 10007 cards it turned out there was:
// - after 10006 applications of the "shuffle" we would be back where we started.
// Unfortunately, that didn't bring me much since we could still not apply our shuffle
// the amount of times required.

// Looking at the puzzle description and the input over and over, I realized some of 
// the samples used a different set of shuffle operations, but still resulted in 
// almost the same end result.
// This reminded me of how we can multiply matrices with each other to transform 
// coordinate spaces and how, instead of repeating the matrix multiplications
// (for example to transform coordinates from object to world to camera space)
// all the time, we can actually premultiply these matrices into a single MVP
// matrix...
//
// Anyway we are not dealing with matrices here, but it set me on the path of
// researching whether these shuffles were linear transformations that we could combine...
//
// One way to test that of course is checking whether our resulting transformation:
// - gives the same answer for part 1
// - gives the identity transformation when applied 10007-1 times...

// Here is how I approached this:

// Dealing cards into a new stack, basically means (e.g. with 10 cards),
// card 0 becomes card 9 and card 9 becomes card 0.
//
// In code we would say:
//  newIndex = (numberOfCards - 1) + -1 * oldIndex
//
// However when we are talking about linear transformation,
// we want to write this as a function ax + b,
// so that we can combine different linear transformations into one.
// 
// In other words, after applying one linear transformation, a*x+b, 
// we need to be able to use this as the input (e.g pInput.a and pInput.b)
// for another transformation a*x+b:
//
// f(x) = ax + b => f (x = input.a * x + input.b) = a * (input.a * x + input.b) + b
//
// Given that, what is the basic transformation on x if we deal a new stack?
// f(x) = -1 * x + (numberOfCards - 1)  
//      = -1 * (input.a * x + input.b) + (numberOfCards - 1)
//      = -1 * input.a * x - input.b + numberOfCards - 1
//         -------A------- -------------B---------------
// In other words our new a = -1 * input.a and our new b = -1 * input.b + numberOfCards - 1 
// And on top of that (after a lot of debugging), I realized there is no way to store 
// the resulting (extremely large) numbers as a long, so I resorted to having to use
// BigIntegers for this purpose.
// 
// In code:

(BigInteger a, BigInteger b) NewStack((BigInteger a, BigInteger b) pInput, BigInteger pDeckSize)
{
	BigInteger newA = -1 * pInput.a;
	BigInteger newB = -1 * pInput.b + (pDeckSize - 1);

    //Normalize makes sure both newA and newB are within the deck bounds
	return Normalize((newA, newB), pDeckSize);
}

// To cut a deck we simply just shift the card through the deck, 
// making sure its index stays within the deck bounds.
// In other words, we just multiply the current index with 1 (= a = no change)
// and shift b by the cut:
//
// f(x) = 1 * x - cut (cut == shift)  
//      = 1 * (input.a * x + input.b) + - cut
//      = input.a * x + input.b - cut
//        -----A-----   -------B------

(BigInteger a, BigInteger b) Cut((BigInteger a, BigInteger b) pInput, BigInteger pCut, BigInteger pDeckSize)
{
	BigInteger newA = pInput.a;
	BigInteger newB = pInput.b - pCut;

	return Normalize((newA, newB), pDeckSize);
}

// To increment a deck we multiply the input with a number
// and wrap the result:
//
// f(x) = increment * (input.a * x + input.b)  
//      = increment * input.a * x + increment * input.b
//        ---------A-------------   ---------B---------

(BigInteger a, BigInteger b) Increment((BigInteger a, BigInteger b) pInput, BigInteger pIncrement, BigInteger pDeckSize)
{
	BigInteger newA = pInput.a * pIncrement;
	BigInteger newB = pInput.b * pIncrement;

	return Normalize((newA, newB), pDeckSize);
}

(BigInteger a, BigInteger b) Normalize((BigInteger a, BigInteger b) pInput, BigInteger pDeckSize)
{
	return (WrapBig(pInput.a, pDeckSize), WrapBig(pInput.b, pDeckSize));
}

BigInteger WrapBig(BigInteger pNumber, BigInteger pModulo)
{
    // Make sure pNumber is within the range [0..pModulo)
	return ((pNumber % pModulo) + pModulo) % pModulo;
}

// To combine all provided deck operations into one linear operation,
// we'll start by taking the linear identity transformation and
// passing it through all the defined transforms:

(BigInteger a, BigInteger b) CreateLinearMapping(BigInteger pDeckSize)
{
	(BigInteger a, BigInteger b) linearMapping = (1, 0);

	foreach (string input in myInput)
	{
		if (input == "deal into new stack")
		{
			linearMapping = NewStack(linearMapping, pDeckSize);
		}
		else
		{
			Match cutMatch = cutRegex.Match(input);
			if (cutMatch.Success)
			{
				linearMapping = Cut(linearMapping, BigInteger.Parse(cutMatch.Groups[1].Value), pDeckSize);
			}
			else
			{
				Match dealMatch = dealRegex.Match(input);
				if (dealMatch.Success)
				{
					linearMapping = Increment(linearMapping, BigInteger.Parse(dealMatch.Groups[1].Value), pDeckSize);
				}
			}
		}
	}

	return linearMapping;
}

// To actually apply the linear mapping and get a result, we'll use:
BigInteger ApplyLinearMapping((BigInteger a, BigInteger b) pLinearMapping, BigInteger pInput, BigInteger pDeckSize)
{
	return WrapBig(pLinearMapping.a * pInput + pLinearMapping.b, pDeckSize);
}

Console.WriteLine();
Console.WriteLine("Part 2:");

var linearMappingTest = CreateLinearMapping(numberOfCardsPart1);

// First test:
Console.WriteLine("Linear transform test:" + ApplyLinearMapping(linearMappingTest, 2019, numberOfCardsPart1));

// Now let's see if we also get the identity transform back when we apply this transform 10006 times to itself:

// First define a method to multiply to linear transformation with each other,
// (basically sequencing them to happen after another, where the order doesn't matter in this case):
//
// pInputA = ax + b, pInputB = cx + d ->        2x + 3           4x + 5
// Result = a (cx+d) + b -> acx + ad + b        2*4*x + 2*5 + 3
// Result = c (ax+b) + d -> acx + bc + d        2*4*x + 3*4 + 5
//
(BigInteger a, BigInteger b) MultiplyWith((BigInteger a, BigInteger b) pInputA, (BigInteger a, BigInteger b) pInputB, BigInteger pDeckSize)
{
	return Normalize((pInputA.a * pInputB.a, pInputA.a * pInputB.b + pInputA.b), pDeckSize);
}

// And a method to actually apply a specific linear mapping x times to itself
(BigInteger a, BigInteger b) ApplyLinearMappingTimes((BigInteger a, BigInteger b) pInput, BigInteger pTimes, BigInteger pDeckSize)
{
	var linearMapping = pInput;

	for (int i = 0; i < pTimes - 1; i++)
	{
		linearMapping = MultiplyWith(linearMapping, pInput, pDeckSize);
	}

	return linearMapping;
}

var combinedLinearMappingTest = ApplyLinearMappingTimes(linearMappingTest, numberOfCardsPart1-1, numberOfCardsPart1);
Console.WriteLine("Combined linear transform test (should be 1,0):" + combinedLinearMappingTest);

// So this works... now we need to create a transform which doesn't apply our input 10007 times but
// 101741582076661... how the hell do we do that?
// In the end I could not come up with another way than trying to factorize 101741582076661 as far as possible
// and then combining those transforms, i.e. 101741582076661 = 1083983 * 1042878 * 90 + 1

// In code:
BigInteger deckSize = 119315717514047;

var linearMapping = CreateLinearMapping(deckSize);
var a = ApplyLinearMappingTimes(linearMapping, 1083983, deckSize);
var b = ApplyLinearMappingTimes(a, 1042878, deckSize);
var c = ApplyLinearMappingTimes(b, 90, deckSize);
var d = MultiplyWith(c, linearMapping, deckSize);

// So now we have a linear transform which will apply all of our input shuffle actions to a deck 
// with size 119315717514047, 101741582076661 times...

// But we don't want to apply this linear transform to transform x into y,
// we want to know where y started, in other words, we want to get the inverse
// of y to x.

// Our final linear mapping is: (49563731917976, 11192648888132)
//
// Taking what we said above this means that we are NOT looking for:
// (linearMapping.a * 2020 + linearMapping.b) % deckSize = x?
//
// But we are looking for:
// (linearMapping.a * x + linearMapping.b) % deckSize == 2020

// In our specific situation:
// (49563731917976 * x + 11192648888132) % 119315717514047 == 2020 
//
// This means
// 49563731917976 * x + 11192648888132 ≡ 2020 MOD 119315717514047
// 49563731917976 * x ≡ 2020 - 11192648888132 MOD 119315717514047
// 49563731917976 * x ≡ 108123068627935 MOD 119315717514047
//
// In other words: 
// 49563731917976 * x + 119315717514047 * y = 108123068627935
//
// How we can solve this is by using a modular inverse IF and ONLY IF the 
// GCD of 49563731917976 and 119315717514047 is 1
// (See my explanation in the NumberUtil class.)

Console.WriteLine("Has modular inverse? "+ (NumberUtil.GCD(49563731917976, 119315717514047) == 1));
BigInteger modInverse = NumberUtil.EGCD(49563731917976, 119315717514047).x;

// But filling in this modInverse would give:
// 49563731917976 * x + 119315717514047 * y = 1 and we need:
// 49563731917976 * x + 119315717514047 * y = 108123068627935

// In other words:
BigInteger finalResult = WrapBig(modInverse * 108123068627935, 119315717514047);
Console.WriteLine("Part 2: " + finalResult);