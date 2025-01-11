class MagicMissile : ASpell
{
    public override int manaCost => base.manaCost = 53;

    protected override void InnerCast (Player pSource, Boss pTarget)
    {
        Log.WriteLine("Pieuw pieuw, " + pSource + " does 4 damage to " + pTarget);
        pTarget.ChangeHealth (-4);
    }
}
