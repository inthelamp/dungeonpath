using Godot;
using System;

public class Wand : LongRangeWeapon
{
    public override void _Ready()
    {
        if (Level == 0)
        {
            Level = 1;
        }
        SetMaximumAp(Level);
        SetMinimumAp(Level);

        var attackAsset = new AttackAsset();
        attackAsset.Name = "FireBall";
        attackAsset.FileName = "res://scene/player/weapon/FireBall.tscn";
        attackAsset.Speed = 500;
        attackAsset.IsEnabled = true;
        AttackAssets.Add(attackAsset.Name, attackAsset);
    }
}
