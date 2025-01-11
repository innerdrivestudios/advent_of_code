abstract class ASpell
{
    public virtual int manaCost { get; protected set; }

    protected int activeTurns;
    public bool isActive => activeTurns > 0;

    //Call this to activate/cast the initial spell, we pass in source and target all the time so that we can replace references
    //with clones of references if need be
    public void Cast (Player pSource, Boss pTarget)
    {
        if (isActive) { throw new InvalidOperationException ("Spell is already active!"); }

        if (pSource.mana >= manaCost)
        {
            pSource.mana -= manaCost;
        }
        else
        {
            throw new InvalidOperationException("No mana to cast this spell!");
        }

        InnerCast(pSource, pTarget);
    }

    //If InnerCast sets activeTurns > 0, for every turn activeTurns > 0, ApplyEffect should be called
    public void ApplyEffect(Player pSource, Boss pTarget)
    {
        if (!isActive) { throw new InvalidOperationException("Spell is not active!"); }

        InnerApplyEffect(pSource, pTarget);
        activeTurns--;
        Log.WriteLine(this + "'s timer is now " + activeTurns);

        if (activeTurns == 0) InnerRemoveEffect(pSource, pTarget);
    }

    //Activate the initial spell
    protected virtual void InnerCast(Player pSource, Boss pTarget) { }
    //Called every time ApplyEffect is called
    protected virtual void InnerApplyEffect(Player pSource, Boss pTarget) { }
    //Called once ApplyEffect is called and activeTurns becomes zero
    protected virtual void InnerRemoveEffect(Player pSource, Boss pTarget) { }

    public virtual ASpell Clone () {
        return (ASpell) this.MemberwiseClone();
    }

    public override string ToString()
    {
        return base.ToString()+"_"+manaCost+"_"+activeTurns;
    }
}

