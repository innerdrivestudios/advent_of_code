using Vec2i = Vec2<int>;

class NPC
{
	public enum NPCType { Elf, Goblin }

	public readonly NPCType type;
	public Vec2i position;
	
	public int hitPoints = 200;
	public int attackPower = 3;

	public NPC (NPCType pType, Vec2i pPosition)
	{
		type = pType;
		position = pPosition;
	}
}
