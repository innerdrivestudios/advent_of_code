using System.Numerics;

public static class NumberUtil
{

	public static void CollapseRanges<T> (List<(T, T)> pRanges) where T: INumber<T> {
		// Now we want to collapse / join overlapping ranges ...
		// Fastest way to do that is to first sort on the start of the range...

		pRanges.Sort((x, y) => x.Item1.CompareTo(y.Item1));

		// And then actually collapse the ranges ...

		for (int i = 0; i < pRanges.Count - 1; i++)
		{
			for (int j = i + 1; j < pRanges.Count;)
			{
				// if the end of the first range we are checking is equal or
				// goes past the start of the second, merge them

				if (pRanges[i].Item2 >= pRanges[j].Item1)
				{
					pRanges[i] = (pRanges[i].Item1, T.Max(pRanges[i].Item2, pRanges[j].Item2));
					pRanges.RemoveAt(j);
				}
				else
				{
					j++;
				}
			}
		}
	}


	// Euclid's GCD algorithm
	//
	// Why or how does this work?
	// First how...
	//
	// We take pA % pB:
	//	- if pA < pB -> pA
	//  - if pA > pB -> we get the remainder...
	//  - then we swap the numbers each time until pB is zero...
	// 
	// So basically after the first try, we are constantly getting the 
	// remainder of the smaller number pA vs the bigger number pB and then swapping pA with pB
	// So IF the remainder is 0, pB will be zero and we return the LAST value we had before
	// it became 0.

	// That is HOW it works.
	// But WHY does it work?
	//
	// The idea is this... if A and B have pA GCD(A,B) (e.g. x), we can express:
	// A as pA * x and B as pB * x.

	// In other words we have two numbers that BOTH can be expressed as pA multiple of x.
	// Let's say we have 432 and 126...
	// (just like in this https://youtu.be/_rRu1jg7Kus video)
	//
	// Given pA GCD(432,126) we know we can make:
	// 432 = x * gcd and 126 = y * gcd

	// But also if we subtract them from each other:
	// 432 - 126 = x * gcd - y * gcd = (x - y) * gcd

	// In other words, whatever we are left with when we subtract 432 - 126
	// is pA multiple of the gcd. We are basically comparing the min and max,
	// taking their difference and expressing it as pA multiple of the gcd:

	// For example:
	// 432 - 126 = 306 = some z * the gcd
	// 
	// But if 126 = some x * the gcd and 306 = some z * the gcd
	// then again the difference is also some w * gcd
	//
	// In other words we can rinse and repeat:
	//
	// We take 306 - 126 = 180
	// 180 - 126 = 54
	// 
	// 54? Ok so in other words, IF there is pA common factor with which we can measure
	// both distances... it is 54 or less...
	//
	// But which numbers should we take now? 432 vs 54 or 126 vs 54?
	// We reduced 432 to 54 by subtracting 126, so now we need to continue with
	// 54 vs 126.
	//
	// BUT repeatedly subtracting pA value until we end up below that value...
	// ... we can do that faster using modulo! In other words... 432 % 126 = 54 !
	//
	// So to check which x allows us to say:
	// 126 = x * pA and 54 = x * pB, we again do 126 - 54 = x * (pA-pB)
	// 126 - 54 = 72, 72 - 54 = 18 OR AGAIN 126 % 54 = 18
	//
	// So now we have 18 vs 54 again we check is there pA remainder?
	// 54 % 18 = 0 HEY THERE IS NO REMAINDER... in other words... 
	//
	// We have found our measuring stick -> 18
	//
	// In code:

	public static T GCD<T>(T pA, T pB) where T : INumber<T>
    {
        while (!T.IsZero(pB))
        {
            T temp = pB;
            pB = pA % pB;
            pA = temp;
        }

        return pA;
    }

    // Along with the GCD we also often require the LEAST COMMON MULTIPLE.
    // The term itself is already confusing, it is also called LOWEST COMMON MULTIPLE.
    //
    // But basically it answers the question, given numbers pA and pB,
    // what is the lowest number for which there are x and y so that:
    // pA * x = pB * y
    // 
    // Now it is easy to see that there is at least pA common multiple
    // (but maybe not the lowest) by setting x = pB and y = pA:
    // pA * pB = pB * pA (tada !)
    //
    // But is it the lowest? That depends on whether pA and pB can be divided by something.
    // Because if pA & pB can both be divided by z then pA * pB can ALSO be divided by z, 
    // which means pA * pB was not the lowest.
    //
    // Well, what is the biggest number that divides both pA and pB? Exactly GCD(pA,pB)!

    public static T LCM<T>(T pA, T pB) where T : INumber<T>
    {
        //changed order of pA*pB/gcd to pA/gcd*pB to prevent overflow
        //in more cases even though it isn't strictly needed here
        return pA / GCD(pA, pB) * pB;
    }


    public static T GetModularInverse<T>(T pA, T pModulo) where T : INumber<T>
    {
        // When we are looking for pA modular inverse
        // we are looking for pA number x that satisfies:
        //
        // (pA * x) % pModulo = 1
        //
        // Note: there is nothing to "get" about this,
        // this is simply the DEFINITION of MODULAR INVERSE.
        //
        // For example:
        // Given pA * x % pModulo = 1,
        // with pA = 3 and pModulo = 5,
        // the modular inverse is 2, since (3 * 2) % 5 = 1
        //
        // Things to note 1:
        //
        // x is always between [1..5) -> the moment we hit 5 we reset to 0 and start over
        // [] => inclusive               i.e. 3 % 5 == 3 * (5 + 1) % 5 since this is
        // () => exclusive               (3 * 15 + 3 * 1) % 5 -> 3 % 5
        // ( And in the general case x is between [1 .. pModulo) )
        //
        // Things to note 2:
        //
        // ONCE you've found pA number x for which A * x = 1 mod pModulo,
        // x is the inverse of A, but automatically A is also the inverse of x
        // In other words A should also be in the range [1..5) in this case,
        // and if it is not we should 'normalize' it, e.g using the previous example, IF:
        // (3 * x) % 5 == 1 is the basic thing we are looking for, but due to calculations
        // we ended up with (8 * x) % 5 == 1, then again we should simplify since:
        // (8 * x) % 5 == 1 -> ((5 + 3) * x) % 5 == 1 -> (5*x) % 5 + (3*x)%5 == 1
        // and any number x * 5 % 5 is 0, since 5 % 5 is 0.
        //
        // Things to note 3:
        //
        // Given pModulo, (pModulo-1) is an inverse of itself:
        // E.g. Modulo 5 ->	(4 * 4) % 5 = 1
        // E.g. Modulo 6 ->	(5 * 5) % 6 = 1
        // E.g. Modulo 7 ->	(6 * 6) % 7 = 1
        // etc
        // The underlying idea here is that (pModulo-1) % pModulo = -1 and -1 * -1 == 1
        //
        // Things to note 4:
        //
        // Given pA and pModulo, there is only an inverse if and only if:
        //
        // GCD(pA, pModulo) == 1.
        //
        // In other words:
        // there is only an inverse if pA and pModulo don't share any other factor except 1.
        // AND if GCD(...) == 1, there is an inverse!
        //
        // How can we understand/proof this?
        //
        // - First off IF there is an inverse this means that there is an invA for which:
        // A * invA == 1 mod pModulo.
        // 
        // - Given the more general equation:
        // (A * x) % M = remainder, the question is	can the remainder ever become 1?
        //
        // Rewriting this gives us, there is pA remainder for which:
        //
        // A * x = M * y + remainder
        //
        // Now assume A & M DO share pA factor z, e.g. A = pA * z and M = m * z,
        // then we can write: (pA * z) * x - (m * z) * y = remainder
        // Which we can rewrite to z * (pA * x - m * y) = remainder
        //
        // In other words: if the remainder is to become 1, z (the shared factor, the gcd),
        // cannot be anything else than 1.
        //
        // Now the other way around:
        // IF GCD(A, m) = 1 then there must be an inverse!

        // We can proof this using Bézout’s identity which is basically saying:
        //
        // There are x and y for which A * x + M * y = GCD (A, M)
        // 
        // Maybe not to completely proof this, but at least to make it believable,
        // have pA look at the explanation for the GCD up top.
        //
        // Let's go back to 432 and 126. 
        //
        // We said:
        //
        // 432 = 3*126 + 54
        // 126 = 2*54  + 18
        //
        // In other words:
        //
        // 54 = 432 - 3 * 126
        // 18 = 126 - 2 * 54
        //
        // 18 = 126 - 2 * (432 - 3 * 126)
        // 18 = 7 * 126 - 2 * 432
        // 
        // In other words:
        //
        // 18 = GCD(432,126) = 7 * 126 - 2 * 432

        // OK so now back to the part where we said:
        // IF GCD(A, B) = 1 then there IS an inverse.
        //
        // Let's fill in A = A and B = our MODULO value, then
        //
        // If the GCD (A, MODULO) = 1 then there is an x and y for which:
        //
        // A*x + Modulo*y = 1
        //
        // In other words: x is our inverse!
        //
        // Video references:
        // - https://www.youtube.com/watch?v=15oQQbAnr3Q
        // - https://youtu.be/_rRu1jg7Kus
        //
        // 

        T a = pA;
        T b = pModulo;

        while (!T.IsZero(b))
        {
            T temp = b;
            b = a % b;
            a = temp;

            if (!T.IsZero(b))
            {
                T times = a / b;
                T remainder = a - b * times;
                if (!T.IsZero(remainder)) {
                    Console.WriteLine(a + " = " + b + " * " + times + " + " + remainder);
                }
            }
        }

        return a;
    }

    public static (BigInteger x, BigInteger y, BigInteger g) EGCD(BigInteger a, BigInteger b)
    {
        if (b == 0)
            return (1, 0, a);       // base case: 1*a + 0*b = a

        var (x1, y1, g1) = EGCD(b, a % b);

        //Console.WriteLine((x1, y1, g1));
        // Back-substitute:
        // x = y1
        // y = x1 - (a / b) * y1
        return (y1, x1 - (a / b) * y1, g1);
    }

    /*
    Let's say we want to evaluate 432 and 125 to figure out what the GCD is, 
    but also to find x and y in 125 * x + 432 * y = 1
    (explain more later...)

    So given 432 and 125 we do (e.g. let's say we started out wrong...)

    125 = 0 * 432 + 125
    432 = 3 * 125 + 57
    125 = 2 * 57 + 11
    57 = 5 * 11 + 2
    11 = 5 * 2 + 1
    2 = 2 * 1 + 0

    
    Note that we always only need A & B eg 
    432 / 125
    125 / 57
    57 / 11
    11 / 5
    2 / 1

    The rest we can calculate!
    But to actually calculate x & y in c = ax + by
    we need to backsubstitute everything bottom up:

    1 = 11 - 5 * 2
                 2 = 57 - 5 * 11
                              11 = 125 + 2 * 57 
                                             57 = 432 - 3 * 125

    1 = 11 - 5 * (57 - 5 * (125 + 2 * (432 - 3 * 125)

    TODO FINISH!

    */

}