/*************************************************************************/
/*  Constants.cs                                                         */
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
using System;

static class Constants
{
	//Shoot
	//The value should be between 0 and 0.9.
	//0 is 90 degree facing straight upward and 0.99 is close to 0 degree facing straight forward.
	public const float MinimumFaceTargetAngle = 0.5f;

	//Mob dies
	//For a flying object, the speed when it dies, falling down from the sky
	public const int FallingDeadSpeed = 200;

	//Status bars
	/*
		Calculate the max health point for level
		MaxHP = BaseHP + (MaximumPossibleHP - BaseHP) * Level / MaximumLevel
	*/
	public const int MaximumLevel = 100;
	public const int MaximumPossibleHP = 10000;
	public const int BaseHP = 500;
	public const int NumberOfMobsForLevelUp = 10;

	/*
		Calculate the max magic point for level
		MaxMP = MaxHP * A
		A is between 0.01 and 0.99 and its default value is 0.7.
	*/
	public const float MPFormularLinearA = 0.7f;

	/*
	   Formular for required experience to level up
	   (A * level + level ^ B) * 10
	   A is between 1 and 10 and its default value is 4.
	   B is between 1.1 and 2 and its default value is 1.85.
	*/
	public const int EXPFormularLinearA = 4;
	public const float EXPFormularExponentB = 1.85f;

	/*
	   Formular for required skill points to level up
	   (A * level + level ^ B) * 10
	   A is between 1 and 10 and its default value is 4.
	   B is between 1.1 and 2 and its default value is 1.85.
	*/
	public const int SPFormularLinearA = 4;
	public const float SPFormularExponentB = 1.95f;

	//HUD
	//For Damage Points Display Label
	public const string DamagePointsDisplayFilename = "res://scene/hud/DamagePointsDisplay.tscn";
	//For Experience Points Display Label
	public const string EXPPointsDisplayFilename = "res://scene/hud/ExperiencePointsDisplay.tscn";
	//For Short Message Display Label
	public const string ShortMessageDisplayFilename = "res://scene/hud/ShortMessageDisplay.tscn";

	//C# doesn't support GDScript's randi().
	public static float RandRand(float min, float max)
	{
		return (float) (new Random().NextDouble() * (max - min) + min);
	}
}
