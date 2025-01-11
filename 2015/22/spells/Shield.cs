class Shield : ASpell
{
    public override int manaCost => base.manaCost = 113;

    protected override void InnerCast(Player pSource, Boss pTarget)
    {
        activeTurns = 6;
        pSource.armor = 7;
        Log.WriteLine("Shield applied, player armor is now 7!");
    }

    protected override void InnerRemoveEffect(Player pSource, Boss pTarget)
    {
        Log.WriteLine("Shield wears off, player armor is now back to 0!");
        pSource.armor = 0;
    }

}
