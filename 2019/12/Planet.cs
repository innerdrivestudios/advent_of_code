
class Planet
{
	public Vec3<long> position { get; private set; }
	public Vec3<long> velocity { get; private set; }

	private Vec3<long> orgPosition;
	private Vec3<long> orgVelocity;

	private static int planetCount = 0;
    public int planetId { get; private set; } = -1;

	public Planet (Vec3<long> pPosition)
	{
		position = pPosition;
		planetId = planetCount++;

        Console.WriteLine($"Planet {planetId} created at position:" + position + " with velocity "+ velocity);
    
		orgPosition = pPosition;
		orgVelocity = velocity;
	}

	public void Reset()
	{
		position = orgPosition;
		velocity = orgVelocity;
    }

	public void UpdateVelocity (Planet pOther)
	{
		//To apply gravity, consider every pair of moons.
		//On each axis (x, y, and z), the velocity of each moon changes
		//by exactly +1 or -1 to pull the moons together.

		Vec3<long> delta = pOther.position - position;
		velocity += delta.Sign();
	}

	public void Simulate()
	{
		position += velocity;
	}

	public override string ToString()
	{
		return "Planet -> position:" + position + " velocity:" + velocity;
	}

	// The total energy for a single moon is its potential energy multiplied
	// by its kinetic energy.
	//  - A moon's potential energy is the sum of the
	//    absolute values of its x, y, and z position coordinates.
	//  - A moon's kinetic energy is the sum of the absolute values of its
	//   velocity coordinates. 

	public long GetTotalEnergy()
	{
        //Console.WriteLine(this);
        long potentialEnergy = position.Abs() * new Vec3<long>(1,1,1);
		long kineticEnergy= velocity.Abs() * new Vec3<long>(1,1,1);
		return potentialEnergy * kineticEnergy;
	}
}
