class Battle {
	static Random random = new Random();

	public Player player { get; private set; }
	public Boss boss { get; private set; }

	public enum Winner { Player, Boss, Undecided };
	private Winner winner = Winner.Undecided;

	public int totalManaSpent { get; private set; } = 0;
	public int extraDamagePerTurn = 0;

	public Battle (Player pPlayer, Boss pBoss)
	{
		player = pPlayer;
		boss = pBoss;
	}

	public Winner GetBattleStatus ()
	{
		return winner;
	}

	public void RunSingleTurn(Func<bool> pPlayerCastingSequence)
	{
		if (winner != Winner.Undecided) throw new InvalidOperationException("Battle has already ended!");

		winner = RunSingleTurnHelper (pPlayerCastingSequence);
	}

    private Winner RunSingleTurnHelper(Func<bool> pPlayerCastingSequence)
    {
        Log.WriteLine("===========================================================");
        Log.WriteLine("New round started!");
        Log.WriteLine("===========================================================\n");
		
        //Player goes first
        Log.WriteLine("-- Player turn --");
        PrintFightStats();

		player.ChangeHealth(-extraDamagePerTurn);
        if (player.health <= 0) return Winner.Boss;

        //Update the spell effects which might kill the boss
        UpdateSpellEffects();
        if (boss.health <= 0) return Winner.Player;

        //Run the player's turn who might lose due to lack of mana or kill the boss
        if (!pPlayerCastingSequence())
        {
            Log.WriteLine("Player does not have enough mana to cast any spell, whoops!");
            return Winner.Boss;
        }

		//Did the casting sequence kill the boss?
        if (boss.health <= 0) return Winner.Player;

        Log.WriteLine();

        //Boss goes second...
        Log.WriteLine("-- Boss turn --");
        PrintFightStats();

        //Update spell effects again, which might, again, kill the boss...
        UpdateSpellEffects();
        if (boss.health <= 0) return Winner.Player;

        //Boss attacks, which might kill the player
        DoBossTurn();
        if (player.health <= 0) return Winner.Boss;

		return Winner.Undecided;
    }

	//return true if a spell was cast, false if there was no spell left to cast
    public bool InteractivePlayerCastingSequence()
	{
		List<ASpell> castableSpells = player.GetCastableSpells();
		if (castableSpells.Count == 0) return false;

		Log.WriteLine("What spell do you want to cast?");

		for (int i = 0; i < castableSpells.Count; i++)
		{
			Log.WriteLine(i + ") " + castableSpells[i] + " Mana cost:(" + castableSpells[i].manaCost + ")");
		}

		int spellToCast = -1;

		while (true)
		{
			ConsoleKeyInfo key = Console.ReadKey();
			Log.WriteLine();
			spellToCast = key.KeyChar - '0';

			if (spellToCast < 0 || spellToCast >= castableSpells.Count)
			{
				Log.WriteLine("Invalid choice.");
			}
			else
			{
				break;
			}
		}

		Log.WriteLine("Player casts " + castableSpells[spellToCast] + ".");
		castableSpells[spellToCast].Cast(player, boss);
        totalManaSpent += castableSpells[spellToCast].manaCost;

        return true;
	}

    //return true if a spell was cast, false if there was no spell left to cast
    public bool AutoRandomCastingSequence()
	{
		List<ASpell> castableSpells = player.GetCastableSpells();
		if (castableSpells.Count == 0) return false;

		int spellToCast = random.Next(0, castableSpells.Count);

		Log.WriteLine("Player casts " + castableSpells[spellToCast] + ".");
		castableSpells[spellToCast].Cast(player, boss);
		totalManaSpent += castableSpells[spellToCast].manaCost;

		return true;
	}

	public bool ForceSpellCast (int pIndex)
	{
        List<ASpell> castableSpells = player.GetCastableSpells();
        if (castableSpells.Count == 0) return false;

		if (pIndex < 0 || pIndex >= castableSpells.Count) return false;

		int spellToCast = pIndex;

        Log.WriteLine("Player casts " + castableSpells[spellToCast] + ".");
        castableSpells[spellToCast].Cast(player, boss);
        totalManaSpent += castableSpells[spellToCast].manaCost;
        return true;
	}

    void PrintFightStats()
    {
        Log.WriteLine($"- Player has {player.health} hit points, {player.armor} armor, {player.mana} mana");
        Log.WriteLine($"- Boss has {boss.health} hit points");
        Log.WriteLine();
    }

    void DoBossTurn()
	{
		Log.WriteLine("Boss attacks for 8 damage");
		player.ChangeHealth(-boss.damage);
	}

	void UpdateSpellEffects()
	{
		List<ASpell> activeSpells = player.GetActiveSpells();
		if (activeSpells.Count == 0) return;

		foreach (ASpell spell in activeSpells) spell.ApplyEffect(player, boss);
        Log.WriteLine();
    }

	public Battle Clone ()
	{
		Battle clone = new Battle(player.Clone(), boss.Clone());
		clone.winner = winner;
		clone.totalManaSpent = totalManaSpent;
		clone.extraDamagePerTurn = extraDamagePerTurn;
        return clone;
	}

	//generate unique identifier for this battle
    public override string ToString()
    {
		//This is the data that matters for the unique state 
		return $"{player}_{boss}_{totalManaSpent}";
    }

}
