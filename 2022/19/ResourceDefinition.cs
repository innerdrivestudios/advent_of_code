
using System.Diagnostics;

/**
 * ResourceDefinition defines amounts of ore, clay, obsidian, geode.
 * This can be used to define robot costs, robot production, current accumulations, etc...
 */
class ResourceDefinition
{
	public int ore		{ private set; get; } = 0;
	public int clay		{ private set; get; } = 0;
    public int obsidian { private set; get; } = 0;
    public int geode	{ private set; get; } = 0;

	public ResourceDefinition() { }

    public ResourceDefinition (int pOre, int pClay, int pObsidian, int pGeode)
	{
		ore = pOre;
		clay = pClay;
		obsidian = pObsidian;
		geode = pGeode;
	}

	public ResourceDefinition Clone ()
	{
		return new ResourceDefinition(ore, clay, obsidian, geode);
	}

	static public ResourceDefinition operator+(ResourceDefinition pResourceDefinitionA, ResourceDefinition pResourceDefinitionB)
	{
		return new ResourceDefinition(
				pResourceDefinitionA.ore		+ pResourceDefinitionB.ore,
				pResourceDefinitionA.clay		+ pResourceDefinitionB.clay,
				pResourceDefinitionA.obsidian	+ pResourceDefinitionB.obsidian,
				pResourceDefinitionA.geode		+ pResourceDefinitionB.geode
			);
	}

    static public ResourceDefinition operator-(ResourceDefinition pResourceDefinitionA, ResourceDefinition pResourceDefinitionB)
    {
        return new ResourceDefinition(
                pResourceDefinitionA.ore		- pResourceDefinitionB.ore,
                pResourceDefinitionA.clay		- pResourceDefinitionB.clay,
                pResourceDefinitionA.obsidian	- pResourceDefinitionB.obsidian,
                pResourceDefinitionA.geode		- pResourceDefinitionB.geode
            );
    }

    static public ResourceDefinition operator *(ResourceDefinition pResourceDefinitionA, int pTimes)
	{
		return new ResourceDefinition(
				pResourceDefinitionA.ore		* pTimes,
				pResourceDefinitionA.clay		* pTimes,
				pResourceDefinitionA.obsidian	* pTimes,
				pResourceDefinitionA.geode		* pTimes
			);
	}

	static public ResourceDefinition operator *(int pTimes, ResourceDefinition pResourceDefinitionA)
	{
		return pResourceDefinitionA * pTimes;
	}

	static public bool operator >(ResourceDefinition pResourceDefinitionA, ResourceDefinition pResourceDefinitionB)
	{
		return
			pResourceDefinitionA.ore		> pResourceDefinitionB.ore		&&
			pResourceDefinitionA.clay		> pResourceDefinitionB.clay		&&
			pResourceDefinitionA.obsidian	> pResourceDefinitionB.obsidian &&
			pResourceDefinitionA.geode		> pResourceDefinitionB.geode;
	}

    static public bool operator <(ResourceDefinition pResourceDefinitionA, ResourceDefinition pResourceDefinitionB)
    {
		return pResourceDefinitionB > pResourceDefinitionA;
    }

    static public bool operator ==(ResourceDefinition pResourceDefinitionA, ResourceDefinition pResourceDefinitionB)
    {
        return
            pResourceDefinitionA.ore		== pResourceDefinitionB.ore &&
            pResourceDefinitionA.clay		== pResourceDefinitionB.clay &&
            pResourceDefinitionA.obsidian	== pResourceDefinitionB.obsidian &&
            pResourceDefinitionA.geode		== pResourceDefinitionB.geode;
    }

    static public bool operator !=(ResourceDefinition pResourceDefinitionA, ResourceDefinition pResourceDefinitionB)
	{
		return !(pResourceDefinitionA == pResourceDefinitionB);
	}

    static public bool operator >=(ResourceDefinition pResourceDefinitionA, ResourceDefinition pResourceDefinitionB)
	{
        return
            pResourceDefinitionA.ore >= pResourceDefinitionB.ore &&
            pResourceDefinitionA.clay >= pResourceDefinitionB.clay &&
            pResourceDefinitionA.obsidian >= pResourceDefinitionB.obsidian &&
            pResourceDefinitionA.geode >= pResourceDefinitionB.geode;
    }

    static public bool operator <=(ResourceDefinition pResourceDefinitionA, ResourceDefinition pResourceDefinitionB)
    {
        return
            pResourceDefinitionA.ore <= pResourceDefinitionB.ore &&
            pResourceDefinitionA.clay <= pResourceDefinitionB.clay &&
            pResourceDefinitionA.obsidian <= pResourceDefinitionB.obsidian &&
            pResourceDefinitionA.geode <= pResourceDefinitionB.geode;
    }

    public override string ToString()
    {
        return (ore, clay, obsidian, geode).ToString();
    }

	

}
