class Drain : ASpell
{
    public override int manaCost => base.manaCost = 73;

    protected override void InnerCast(Player pSource, Boss pTarget)
    {
        Log.WriteLine("Slurp, " + pSource + " drains 2 health from target " + pTarget);
        pTarget.ChangeHealth(-2);
        pSource.ChangeHealth(2);
    }
}
