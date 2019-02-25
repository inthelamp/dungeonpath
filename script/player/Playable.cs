using Godot;
using System;

public abstract class Playable : KinematicBody2D
{
	public int Level { get; set; } //level
	public int Stage { get; set; } = 1; //stage number
	public int MaxHp { get; set; } //Maximum Health Point
	public int MaxMp { get; set; } //Maximum Magic Point
	public int MaxExp { get; set; } //Required Experience Point to level up
	public float CurrentHp { get; set; } //Current Health Point
	public float CurrentMp { get; set; } //Current Magic Point
	public float CurrentExp { get; set; } //Current Experience Point
	public bool IsReadyToFight { get; set; } 
	public bool IsCircleForm { get; set; }
	public bool IsInAttack { get; set; }	
	protected bool IsEntered { get; set; }

	public Mob TargetMob { get; set; }  //The mob to attack

	//Calculate the required experience point to level up
	public abstract int GetRequiredExpForLevelUp(int level);

	//Calculate the max health point for the level
	public abstract int GetMaxHpForLevel(int level);

	//Calculate the max magic point for the level
	public abstract int GetMaxMpForLevel(int level);

	//float restExp = (CurrentExp + gainedExp) - MaxExp;
	//if restExp >= 0, then level-up.
	public abstract void LevelUp(float restExp);

	//It's about how much damage player can give a mob by the attack and 
	//the damage decrease the mob's HP in the end.
	public abstract int GetAttackPoints();

	//Get it attacked
	public abstract void GetAttacked(int damagePoints);

	//Release the target locked on or aimed
	public abstract void ReleaseTarget();

	//Lock on the target
	public abstract void LockOnTarget(Mob mob);	

	//Add experience points
	public abstract void AddExp(float exp);
}
