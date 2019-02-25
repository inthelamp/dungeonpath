using Godot;
using System;	

public class AttackAsset
{
    public int Speed { get; set; }	//Speed of attack
	public string Name { get; set; }	//Name of attack
	public string FileName { get; set; }   //File name of attack
    public bool IsEnabled { get; set; }    //Is it ready to use?
}	

public abstract class Weapon : Node
{	
    public string PlayerPath { get; set; } //node path of the one who possesses this weapon.
	public int Level { get; set; } //level
	public int MaximumAp { get; set; } //Maximum attack points
	public int MinimumAp { get; set; } //Minimum attack points    
    public System.Collections.Generic.Dictionary<string, AttackAsset> AttackAssets { get; set; } 
                        = new System.Collections.Generic.Dictionary<string, AttackAsset>();

    //Set MaximumAp
    public abstract void SetMaximumAp(int level);

    //Set MinimumAp
    public abstract void SetMinimumAp(int level);

    //Random number between MinimumAp and MaximumAp
	public abstract int GetAttackPoints();
}