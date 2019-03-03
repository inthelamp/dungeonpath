/*************************************************************************/
/*  Playable.cs                                                          */
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

public abstract class Playable : KinematicBody2D
{
	public int Level { get; set; } //level
	public int Stage { get; set; } = 1; //stage number
	public int MaxHP { get; set; } //Maximum Health Point
	public int MaxMP { get; set; } //Maximum Magic Point
	public int MaxEXP { get; set; } //Required Experience Point to level up
	public float CurrentHP { get; set; } //Current Health Point
	public float CurrentMP { get; set; } //Current Magic Point
	public float CurrentEXP { get; set; } //Current Experience Point
	public bool IsReadyToFight { get; set; }
	public bool IsCircleForm { get; set; }
	public bool IsInAttack { get; set; }
	public bool IsEntering { get; set; }

	public Mob TargetMob { get; set; }  //The mob to attack

	//Calculate the required experience point to level up
	public abstract int GetRequiredEXPForLevelUp(int level);

	//Calculate the max health point for the level
	public abstract int GetMaxHPForLevel(int level);

	//Calculate the max magic point for the level
	public abstract int GetMaxMPForLevel(int level);

	//float restExp = (CurrentEXP + gainedExp) - MaxEXP;
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
	public abstract void AddEXP(float exp);
}
