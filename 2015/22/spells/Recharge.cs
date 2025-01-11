class Recharge : ASpell
{
    public override int manaCost => base.manaCost = 229;

    protected override void InnerCast(Player pSource, Boss pTarget)
    {
        activeTurns = 5;
    }

    protected override void InnerApplyEffect(Player pSource, Boss pTarget)
    {
        Log.WriteLine("Recharge provides 101 mana to " + pSource);
        pSource.mana += 101;
    }

    protected override void InnerRemoveEffect(Player pSource, Boss pTarget)
    {
        Log.WriteLine("Recharge wears off");
    }

}
