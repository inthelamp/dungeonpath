using Godot;
using System;

public class Mob : Living
{
	[Signal] public delegate void MobDie();

	//Start moving
	public override void Start()
	{
		MaxHp = this.GetMaxHpForLevel(Level);
		MaxMp = this.GetMaxMpForLevel(Level);
		CurrentHp = MaxHp;
		CurrentMp = MaxMp;
		var hp = (HealthPoint)GetNode("HealthPoint");
		hp.SetMax(MaxHp);
		hp.SetValue(CurrentHp);
		var mp = (MagicPoint)GetNode("MagicPoint");
		mp.SetMax(MaxMp);
		mp.SetValue(CurrentMp);			
	}

	public override void SetMobLockedOn(Player player)
	{
		TargetPlayer = player;
		SetVisibleOfStatusBars(true);
		IsLockedOn = true;	
	}

	public override void SetMobReleased()
	{
		if (TargetPlayer != null)
		{
			TargetPlayer = null;
		}

		IsLockedOn = false;			
		SetVisibleOfStatusBars(false);
	}

	protected void SetVisibleOfStatusBars(bool isVisible)
	{
		var hpBar = (TextureProgress)GetNode("HealthPoint");
		hpBar.Visible = isVisible;
		var mpBar = (TextureProgress)GetNode("MagicPoint");
		mpBar.Visible = isVisible;
	}

	public override void Die()
	{
		QueueFree();
	}	
	
	//Calculate the max health points for level
	//MaxHp = BASE_HP + (MAXIMUM_POSSIBLE_HP - BASE_HP) * Level /
	//MAXIMUM_LEVEL
	public override int GetMaxHpForLevel(int level)
	{
		double results = Constants.BASE_HP +
		(Constants.MAXIMUM_POSSIBLE_HP - Constants.BASE_HP) *
		level / Constants.MAXIMUM_LEVEL;
		return (int) Math.Floor(results);
	}

	//Calculate the max magic points for level
	//MaxMp = MaxHp * A
	//A is between 0.01 and 0.99 and its default value is 0.7.
	public override int GetMaxMpForLevel(int level)
	{
		return (int)Math.Floor(GetMaxHpForLevel(level) *
		Constants.MP_FORMULAR_LINEAR_A);
	}

	//It's about how much damage this mob can give player by the attack and 
	//the damage decrease the player's HP in the end.
	public override int GetAttackPoints()
	{
		return (int)(MaxHp/10);
	}

    //Give player some experience points when this mob is removed.
    //requiredExp is the required experience points for player to level up
    // and the experience points the mob gives player is to divide requiredExp by
    // the number of mobs for player to hunt for its level-up.
	public override float GetExp()
	{
		var level = Level + 1;
		double requiredExp = (Constants.EXP_FORMULAR_LINEAR_A * level)
		+ Math.Pow(level, Constants.EXP_FORMULAR_EXPONENT_B);
		return Convert.ToSingle(requiredExp / Constants.NUMBER_OF_MOBS_FOR_LEVEL_UP);
	}

	public override void GetAttacked(int damagePoints)
	{
		//Update current HP
		CurrentHp -= damagePoints;		

		var hp = (HealthPoint)GetNode("HealthPoint");
		hp.SetValue(Convert.ToSingle(CurrentHp));

		//Display the damage points which reduce HP.
		ShowDamagePoints(damagePoints);

		if (CurrentHp <= 0)
		{
			EmitSignal("MobDie", this);
		}
	}	

	//Hit player
	protected void Hit(Player player)
	{
		if (!IsInAttack) //Mob's attack has just occured.  
		{
			//Play attack sound
			var attackSound = (AudioStreamPlayer)GetNode("Sounds/Hit");
			attackSound.Play();		
			
			IsInAttack = true;
			var combatDelayTimer = (Timer)GetNode("CombatDelay");
			combatDelayTimer.Start();

			//Player should change its form to Circle Form for the hand-to-hand combat
			if (!player.IsCircleForm)
			{
				//Mob takes advantage of the first attack before player's recognition.
				//So the attack point is doubled.
				player.GetAttacked(GetAttackPoints() * 2);
				return;
			} 
			else
			{
				player.GetAttacked(GetAttackPoints());
				return;
			}
		} 
	}

	protected void ShowDamagePoints(int damagePoints)
	{
		//load damage points label	
		var damagePointsDisplayScene = (PackedScene)GD.Load(Constants.DAMAGE_POINTS_DISPLAY_FILENAME);
		if (damagePointsDisplayScene == null) 
		{
			return;  //Error handling
		}
		var damagePointsDisplay = (ShortMessage)damagePointsDisplayScene.Instance();
		
		//update the label
		damagePointsDisplay.SetText("-" + damagePoints.ToString());
		AddChild(damagePointsDisplay);
		damagePointsDisplay.Start();
	}

	protected void OnEnablerViewportExited(Godot.Object viewport)
	{
		if (TargetPlayer != null && TargetPlayer.TargetMob == this)
		{
			TargetPlayer.ReleaseTarget();
		}
	}

	//After this delay mob can attack.
	protected void OnCombatDelayTimeout()
	{
		IsInAttack = false; //This gives Mob a turn to attack player. 
	}	

	protected void OnDamagePointsDisplayTimeout()
	{
		var damagePointsDisplay = (Label)GetNode("DamagePoints");
		damagePointsDisplay.Visible = false;		
	}
}