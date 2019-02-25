using Godot;
using System;

public abstract class Attack : KinematicBody2D
{
    [Export]
    public int Speed; // How fast the attack will move (pixels/sec).
 
    public string WeaponPath { get; set; }
    public bool IsInAttack { get; set; }	
    
	public abstract void Start(Vector2 pos, float dir);
}


