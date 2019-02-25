using System;

static class Constants
{
	//Shoot
	//The value should be between 0 and 0.9.
	//0 is 90 degree facing straight upward and 0.99 is close to 0 degree facing straight forward.
	public const float MINIMUM_FACE_TARGET_ANGLE = 0.5f;
	
	//Mob dies
	//For a flying object, the speed when it dies, falling down from the sky  
	public const int FALLING_DEAD_SPEED = 200;

	//Status bars
	/*
		Calculate the max health point for level
		MaxHp = BASE_HP + (MAXIMUM_POSSIBLE_HP - BASE_HP) * Level / MAXIMUM_LEVEL
	*/
	public const int MAXIMUM_LEVEL = 100;
	public const int MAXIMUM_POSSIBLE_HP = 10000;
	public const int BASE_HP = 500;
	public const int NUMBER_OF_MOBS_FOR_LEVEL_UP = 10;

	/*
		Calculate the max magic point for level
		MaxMp = MaxHp * A
		A is between 0.01 and 0.99 and its default value is 0.7.
	*/
	public const float MP_FORMULAR_LINEAR_A = 0.7f;

	/*
	   Formular for required experience to level up
	   (A * level + level ^ B) * 10
	   A is between 1 and 10 and its default value is 4.
	   B is between 1.1 and 2 and its default value is 1.85.
	*/
	public const int EXP_FORMULAR_LINEAR_A = 4;
	public const float EXP_FORMULAR_EXPONENT_B = 1.85f;

	/*
	   Formular for required skill points to level up
	   (A * level + level ^ B) * 10
	   A is between 1 and 10 and its default value is 4.
	   B is between 1.1 and 2 and its default value is 1.85.
	*/
	public const int SP_FORMULAR_LINEAR_A = 4;
	public const float SP_FORMULAR_EXPONENT_B = 1.95f;

	//HUD
	//For Damage Points Display Label
	public const string DAMAGE_POINTS_DISPLAY_FILENAME = "res://scene/hud/DamagePointsDisplay.tscn";
	//For Experience Points Display Label
	public const string EXP_POINTS_DISPLAY_FILENAME = "res://scene/hud/ExperiencePointsDisplay.tscn";	
	//For Short Message Display Label
	public const string SHORT_MESSAGE_DISPLAY_FILENAME = "res://scene/hud/ShortMessageDisplay.tscn";		
	
	//C# doesn't support GDScript's randi().
	public static float RandRand(float min, float max)
	{
		return (float) (new Random().NextDouble() * (max - min) + min);
	}
}