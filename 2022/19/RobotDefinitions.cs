// This helper class defines how much each type of robot produces...
static class RobotDefinitions
{
	public static readonly ResourceDefinition ORE_ROBOT_PRODUCTION		= new ResourceDefinition (1,0,0,0);
	public static readonly ResourceDefinition CLAY_ROBOT_PRODUCTION		= new ResourceDefinition (0,1,0,0);
	public static readonly ResourceDefinition OBSIDIAN_ROBOT_PRODUCTION	= new ResourceDefinition (0,0,1,0);
	public static readonly ResourceDefinition GEODE_ROBOT_PRODUCTION	= new ResourceDefinition (0,0,0,1);
}

