/**
 * In addition to health the Boss has a fixed amount of damage it can do.
 */
class Boss : ACharacter
{
    public int damage {  get; set; }    

    public Boss(int pHealth, int pArmor, int pDamage) : base(pHealth, pArmor)
    {
        damage = pDamage;
    }

    public Boss Clone()
    {
        return (Boss)this.MemberwiseClone();
    }

    public override string ToString()
    {
        return "[Boss:"+base.ToString() + "_" + damage+"]";
    }
}
