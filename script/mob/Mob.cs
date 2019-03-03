/*************************************************************************/
/*  Mob.cs                                                               */
/*************************************************************************/
/*                       This file is part of:                           */
/*                           DungeonPath                                 */
/*             https://github.com/inthelamp/dungeonpath                  */
/*************************************************************************/
/* Copyright (c) 2018-2019 Dong Won Kim.                                 */
/*                                                                       */
/* Permission is hereby granted, free of charge, to any person obtaining */
/* a copy of this software and associated documentation files (the       */
/* "Software"), to deal in the Software without restriction, including   */
/* without limitation the rights to use, copy, modify, merge, publish,   */
/* distribute, sublicense, and/or sell copies of the Software, and to    */
/* permit persons to whom the Software is furnished to do so, subject to */
/* the following conditions:                                             */
/*                                                                       */
/* The above copyright notice and this permission notice shall be        */
/* included in all copies or substantial portions of the Software.       */
/*                                                                       */
/* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,       */
/* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF    */
/* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.*/
/* IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY  */
/* CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,  */
/* TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE     */
/* SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                */
/*************************************************************************/
using Godot;
using System;

public class Mob : Living
{
	[Signal] public delegate void MobDie();

	//Start moving
	public override void Start()
	{
		MaxHP = this.GetMaxHPForLevel(Level);
		MaxMP = this.GetMaxMPForLevel(Level);
		CurrentHP = MaxHP;
		CurrentMP = MaxMP;
		var hp = (HealthPoint)GetNode("HealthPoint");
		hp.SetMax(MaxHP);
		hp.SetValue(CurrentHP);
		var mp = (MagicPoint)GetNode("MagicPoint");
		mp.SetMax(MaxMP);
		mp.SetValue(CurrentMP);
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
	//MaxHP = BaseHP + (MaximumPossibleHP - BaseHP) * Level /
	//MaximumLevel
	public override int GetMaxHPForLevel(int level)
	{
		double results = Constants.BaseHP +
		(Constants.MaximumPossibleHP - Constants.BaseHP) *
		level / Constants.MaximumLevel;
		return (int) Math.Floor(results);
	}

	//Calculate the max magic points for level
	//MaxMP = MaxHP * A
	//A is between 0.01 and 0.99 and its default value is 0.7.
	public override int GetMaxMPForLevel(int level)
	{
		return (int)Math.Floor(GetMaxHPForLevel(level) *
		Constants.MPFormularLinearA);
	}

	//It's about how much damage this mob can give player by the attack and
	//the damage decrease the player's HP in the end.
	public override int GetAttackPoints()
	{
		return (int)(MaxHP/10);
	}

    //Give player some experience points when this mob is removed.
    //requiredExp is the required experience points for player to level up
    // and the experience points the mob gives player is to divide requiredExp by
    // the number of mobs for player to hunt for its level-up.
	public override float GetEXP()
	{
		var level = Level + 1;
		double requiredExp = (Constants.EXPFormularLinearA * level)
		+ Math.Pow(level, Constants.EXPFormularExponentB);
		return Convert.ToSingle(requiredExp / Constants.NumberOfMobsForLevelUp);
	}

	public override void GetAttacked(int damagePoints)
	{
		//Update current HP
		CurrentHP -= damagePoints;

		var hp = (HealthPoint)GetNode("HealthPoint");
		hp.SetValue(Convert.ToSingle(CurrentHP));

		//Display the damage points which reduce HP.
		ShowDamagePoints(damagePoints);

		if (CurrentHP <= 0)
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
		var damagePointsDisplayScene = (PackedScene)GD.Load(Constants.DamagePointsDisplayFilename);
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
