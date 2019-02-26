/*************************************************************************/
/*  Living.cs                                                            */
/*************************************************************************/
/*                       This file is part of:                           */
/*                           DungeonPath                                 */
/*             https://github.com/dwkim263/DungeonPath/wiki              */
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
