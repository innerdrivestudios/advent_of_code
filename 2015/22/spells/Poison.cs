class Poison : ASpell
{
    public override int manaCost => base.manaCost = 173;

    protected override void InnerCast(Player pSource, Boss pTarget)
    {
        activeTurns = 6;
    }

    protected override void InnerApplyEffect(Player pSource, Boss pTarget)
    {
        Log.WriteLine("Poison deals 3 damage to "+pTarget);
        pTarget.ChangeHealth(-3);
    }

    protected override void InnerRemoveEffect(Player pSource, Boss pTarget)
    {
        Log.WriteLine("Poison wears off");
    }

}
