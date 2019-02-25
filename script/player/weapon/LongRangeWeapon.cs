using Godot;
using System;

public class LongRangeWeapon : Weapon
{
	//Set MaximumAp
    public override void SetMaximumAp(int level)
    {
		double results = (Constants.BASE_HP +
		(Constants.MAXIMUM_POSSIBLE_HP - Constants.BASE_HP) *
		level / Constants.MAXIMUM_LEVEL) / 10;
		MaximumAp = (int)Math.Floor(results);
    }

    //Set MinimumAp
    public override void SetMinimumAp(int level)
    {
		double results = (Constants.BASE_HP +
		(Constants.MAXIMUM_POSSIBLE_HP - Constants.BASE_HP) *
		level / Constants.MAXIMUM_LEVEL) / 15;
        MinimumAp = (int)Math.Floor(results);
    }

    //Random number between MinimumAp and MaximumAp
	public override int GetAttackPoints()
    {
        return (int)Constants.RandRand(MinimumAp, MaximumAp);
    }
}