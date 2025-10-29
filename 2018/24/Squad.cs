using System.Text.RegularExpressions;

class Squad
{
    public int unitCount {
        get {
            return _unitCount;
        }
        
        set { _unitCount = Math.Max(0, value); } 
    }

    private int _unitCount;
    private int _originalUnitCount;

    public enum Type {  Immune, Infection, None }
    public Type type { get; }

    public int hitPointsPerUnit { get; }
    public HashSet<string> weaknesses { get; }
    public HashSet<string> immunities { get; }
    public int damage { get; }
    public string damageType { get; }
    public int initiative { get; }
    public int id { get; }

    public Squad currentTarget = null;
    public bool chosen = false;
    public int boost = 0;

    public int effectivePower => unitCount * (damage + boost);

    public Squad (string pSquadSpecification, Type pType, int pId)
    {
        type = pType;
        id = pId;

        Regex squadParser = new Regex(
                @"(\d+) units each with (\d+) hit points (?:\(([^)]+)\)\s)?with an attack that does (\d+) ([a-z]+) damage at initiative (\d+)"
        );

        // (?: \(([^)]+)\) )? ->
        // We want to make the whole thing optional so we need to enclose it in (), but we only want to capture the inner part
        // So we start with ?: in the outer braces to declare a non capturing group
        // Then within we have \(([^)]+)\) which is \( ( [^)]+ ) \)
        // In other words:
        //  \(      -> start with (
        //  (       -> start capturing group
        //  [^)]+   -> match everything that is not a )
        //  \)      -> match )

        Match match = squadParser.Match( pSquadSpecification );
        if (!match.Success)
        {
            Console.WriteLine("FAIL:" + pSquadSpecification);
            return;
        }

        weaknesses = new HashSet<string>();
        immunities = new HashSet<string>();

        unitCount = _originalUnitCount =  int.Parse(match.Groups[1].Value);
        hitPointsPerUnit = int.Parse(match.Groups[2].Value);

        if (match.Groups[3].Success)
        {
            //(weak to radiation; immune to fire, cold)
            string specs = match.Groups[3].Value;

            //[weak to radiation,immune to fire, cold]
            string[] specParts = specs.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (string specPart in specParts)
            {
                string[] specPartParts = specPart.Split(" to ", StringSplitOptions.TrimEntries);
                HashSet<string> hashSetToUse = specPartParts[0] == "weak" ? weaknesses : immunities;
                hashSetToUse.UnionWith(specPartParts[1].Split(", ").ToHashSet());
            }
        }

        damage = int.Parse(match.Groups[4].Value);
        damageType = match.Groups[5].Value;
        initiative = int.Parse(match.Groups[6].Value);
    }

    public override string ToString()
    {
        List<string> specList = new List<string>();

        if (weaknesses.Count > 0)
        {
            specList.Add("weak to " + string.Join(", ", weaknesses));
        }

        if (immunities.Count > 0)
        {
            specList.Add("immune to " + string.Join(", ", immunities));
        }

        string specs = string.Join("; ", specList);
        if (specList.Count > 0) specs = "(" + specs + ") ";

        return  $"{unitCount} units each " +
                $"with {hitPointsPerUnit} hit points "+
                $"{specs}"+
                $"with an attack that does {damage} {damageType} damage "+
                $"at initiative {initiative}";
    }

    public void Reset ()
    {
        unitCount = _originalUnitCount;
        chosen = false;
        currentTarget = null;
    }
}
