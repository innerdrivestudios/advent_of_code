/**
 * In addition to health, Player has mana and spells to cast with mana.
 */
class Player : ACharacter
{
    public int mana { get; set; }

    private List<ASpell> spells = new List<ASpell>();

    public int spellCount => spells.Count;

    public Player(int pHealth, int pArmor, int pMana) : base(pHealth, pArmor)
    {
        mana = pMana;
    }

    public void AddSpell (ASpell pSpell)
    {
        spells.Add(pSpell);
    }

    public List<ASpell> GetCastableSpells()
    {
        return spells.Where (x => x.manaCost <= mana && !x.isActive).ToList();
    }

    public List<ASpell> GetActiveSpells()
    {
        return spells.Where (x => x.isActive).ToList();
    }

    public Player Clone ()
    {
        Player clone = (Player)this.MemberwiseClone();

        //override the spell list with clones of the spells as well, little bit redudant to clone the player first
        clone.spells = new List<ASpell>();
        foreach (ASpell spell in spells) { clone.spells.Add(spell.Clone()); }

        return clone;
    }

    public override string ToString()
    {
        return "[Player:"+base.ToString() + "_"+mana+ "_" + string.Join("_", spells)+"]";
    }

}
