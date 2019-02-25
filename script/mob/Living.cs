using Godot;
using System;

public abstract class Living : RigidBody2D
{
	[Export]
    public int Level; // How fast the attack will move (pixels/sec).
	
	public Sprite MobSprite { get; set; } //Mob Sprite
	public AnimationPlayer AnimatPlay { get; set; } //AnimationPlayer
	public string Animation { get; set; } //Animation like "idle", "walk", "hit"
	public PathFollow2D SpawnLocation { get; set; }

	public int MaxHp { get; set; } //Maximum Health Point
	public int MaxMp { get; set; } //Maximum Magic Point
	public float CurrentHp { get; set; } //Current Health Point
	public float CurrentMp { get; set; } //Current Magic Point
	public int MinSpeed { get; set; } //Minimum speed
	public int MaxSpeed { get; set; } //Maximum speed

	public Playable TargetPlayer { get; set; }  //The player to attack

	public String[] MovingTypes; //Define moving animations

	public bool IsInAttack { get; set; }	//check if it is in attacking player
	public bool IsLockedOn { get; set; } 	//check if mob is locked on by player
	public bool IsDead { get; set; }        //check if mob is dead

	//Start moving
	public abstract void Start();

	//Delete mob
	public abstract void Die();

	//Calculate the max health point for the level
	public abstract int GetMaxHpForLevel(int level);

	//Calculate the max magic point for the level
	public abstract int GetMaxMpForLevel(int level);

	//It's about how much damage this mob can give player by the attack and 
	//the damage decrease the player's HP in the end.
	public abstract int GetAttackPoints();

	//Dealing with player's attack on this mob
	public abstract void GetAttacked(int damagePoints);

	//This mob becomes a target of player.
	public abstract void SetMobLockedOn(Player player);

	//This mob is released from lock-on state. 
	public abstract void SetMobReleased();	

	//Give player some experience points when this mob is removed.
	public abstract float GetExp();
}
