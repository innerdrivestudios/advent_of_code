class Particle
{
	public Vec2<int> position;
	public Vec2<int> velocity;

	public void Simulate(bool pForward)
	{
		position += velocity * (pForward?1:-1);
	}
}

