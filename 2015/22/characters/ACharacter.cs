/**
 * Basic character class.
 * 
 * Each character has a health, armor and hitpoints.
 */
abstract class ACharacter
{
    public int hitpoints { get; private set; } = 0;
    public int health { get; private set; } = 0;
    public int armor = 0;

    public bool isAlive => health > 0;
    
    public ACharacter (int pHealth, int pArmor) { 
        health = hitpoints = pHealth; 
        armor = pArmor;
    }    

    public void ChangeHealth (int pChange)
    {
        if (health == 0) throw new InvalidOperationException(this + " is already dead.");

        //if we are doing damage, take the armor into account
        if (pChange < 0)
        {
            int damageDone = Math.Max(1, Math.Abs(pChange) - armor);
            health -= damageDone;
            Log.WriteLine(this + " took "+damageDone+" damage.");
        }
        //if we are healing just add it
        else if (pChange > 0)
        {
            health += pChange;
            Log.WriteLine(this + " healed for " + pChange + " points");
        }
        
        //but we can never go beyond zero or our original amount of hit points
        health = Math.Clamp(health, 0, hitpoints);

        if (health == 0) Log.WriteLine(this + " died!");
        else Log.WriteLine(this + " has "+health +" hitpoints left!");
    }

    public override string ToString()
    {
        return $"{hitpoints}_{health}_{armor}";
    }
}

