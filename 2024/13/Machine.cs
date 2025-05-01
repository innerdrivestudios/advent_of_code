using Vec2L = Vec2<long>;
using Vec2D = Vec2<double>;

class Machine
{
	private Vec2L buttonAMove = new Vec2L(0, 0);	//How far we move when we press button A
	private Vec2L buttonBMove = new Vec2L(0, 0);	//How far we move when we press button B

	private Vec2L priceLocation;

	public Machine(Vec2L pButtonAMove, Vec2L pButtonBMove, Vec2L pPriceLocation)
	{
		buttonAMove = pButtonAMove;
		buttonBMove = pButtonBMove;
		priceLocation = pPriceLocation;
	}

	public Vec2L FindCheapestMoveCombo(Vec2L pButtonCosts)
	{
		//First figure out how many time we can stomp button A before shooting past the target
		long maxA = long.Min(priceLocation.X / buttonAMove.X, priceLocation.Y / buttonAMove.Y);
		
		//With zero presses of B
		int maxB = 0;

		//Now set some initial value and see if we can find the price location and how "costly" that would be
		int currentCost = int.MaxValue;
		Vec2L moves = new Vec2L();

		while (maxA > 0)
		{
			Vec2L location = maxA * buttonAMove + maxB * buttonBMove;
			
			if (location.Equals(priceLocation))
			{
				long newCost = pButtonCosts.X * maxA + pButtonCosts.Y * maxB;
				if (newCost < currentCost) moves = new Vec2L(maxA, maxB);
			}

			//Keep evening out our A and B moves as we home in on the target location...
			if (location.X < priceLocation.X && location.Y < priceLocation.Y)
			{
				maxB++;
			}
			else
			{
				maxA--;
			}

		}

		return moves;
	}

	public Vec2L FindCheapestMoveComboOptimized (Vec2L pButtonCosts)
	{
        //We basically calculate the inverse of the matrix [a,b,c,d] and multiply our prize location with that
        //(https://www.mathcentre.ac.uk/resources/uploaded/sigma-matrices7-2009-1.pdf)
        
		double a = buttonAMove.X;
		double b = buttonAMove.Y;
		double c = buttonBMove.X;
		double d = buttonBMove.Y;

		double determinant = a * d - b * c;

        if (determinant == 0) return new Vec2L(0, 0);

		Vec2D aInverse = new Vec2D(d, -c) / determinant;
		Vec2D bInverse = new Vec2D(-b, a) / determinant;

		Vec2D priceLocationAsDouble = new Vec2D(priceLocation.X, priceLocation.Y);

		Vec2D answer = new Vec2D(priceLocationAsDouble * aInverse, priceLocationAsDouble * bInverse);

		//Rounding is very important which feels extremely fishy ;)
		Vec2L answerAsLong = new Vec2L((long)Math.Round(answer.X), (long)Math.Round(answer.Y));

		if (priceLocation.Equals(answerAsLong.X * buttonAMove + answerAsLong.Y * buttonBMove))
		{
			//Console.WriteLine("Answer:"+answerAsLong);
			//Console.WriteLine("-----------------");
			return answerAsLong;
        }
		else
		{
			//Console.WriteLine("No answer found, requested:" + priceLocation + " and got:" + answer);
			//Console.WriteLine("-----------------");
			return new Vec2L(0, 0);
		}
    }

    public Vec2L FindCheapestMoveComboOptimizedAlternative()
    {
		// This is also an optimized method, which basically does the same thing by
		// actually solving the equation for x and y:
		// x * A + y * B = target

        // x * (A.x,A.y) + y * (B.x, B.y) = target
        // (x * A.x,x * A.y) +  (y * B.x, y * B.y) = target
        // (x * A.x + y * B.x, x * A.y + y * B.y)  = (target.X, target.Y)

        // y = (target.X - x*A.x)/B.x
        // x * A.y *B.x - x* B.y * A.x = target.Y * B.x -  (B.y * target.X )  
        // x *  = (target.Y * B.x - B.y * target.X) / (A.y *B.x - B.y * A.x)

        double x = (priceLocation.Y * buttonBMove.X - buttonBMove.Y * priceLocation.X) / 
				   (buttonAMove.Y * buttonBMove.X - buttonBMove.Y * buttonAMove.X);
        //alternative solution method, didn't do the rest
        double y = (priceLocation.X - x * buttonAMove.X) / buttonBMove.X;

        Vec2L test = (long)x * buttonAMove + (long)y * buttonBMove;

        if (test.Equals(priceLocation))
        {
            return new Vec2L((long)x, (long)y);
        }
        else
        {
            return new Vec2L(0, 0);
        }

    }

}


