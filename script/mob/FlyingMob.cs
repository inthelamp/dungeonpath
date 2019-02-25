using Godot;
using System;

public class FlyingMob : Mob
{
	private readonly String[] DEFAULT_MOVING_TYPES = { "fly" };

	public override void _Ready()
	{	 
		MobSprite = (Sprite)GetNode("Sprite");
		
		if (MovingTypes == null)
		{
			MovingTypes = DEFAULT_MOVING_TYPES;
		}

		AnimatPlay = (AnimationPlayer)GetNode("Anim");
		Animation = MovingTypes[new Random().Next(0, MovingTypes.Length)];
	}

	public override void _PhysicsProcess(float delta)
	{	
		var newAnim = Animation;
		
		if (IsDead)
		{
			newAnim = "dead";
			var posY = Position.y + Constants.FALLING_DEAD_SPEED*delta;
			if (posY > GetViewport().GetSize().y) //If it is greater than height, then free it.
			{
				QueueFree();
			}			
		}
		else
		{ 
			newAnim = "fly";				
			SpawnLocation.SetOffset(SpawnLocation.GetOffset() + Constants.RandRand(MinSpeed, MaxSpeed) * delta);
			var direction = SpawnLocation.Rotation; 
			Rotation = direction;
			Position = SpawnLocation.Position;					
		}

		if (IsInAttack)
		{
			newAnim = "hit";
		}

		if (Animation != newAnim)
		{
			Animation = newAnim;
			AnimatPlay.Play(Animation);	
		}				
	}

	//Contact Report is 4 and Contact Monitor is on.
	public void OnBodyEntered(Playable body)
	{
		if (body is Player) //Player is the object to get a collision with this mob.
		{   
			Player player = (Player)body;
			Hit(player);				
		}
	}	
	
	private void OnAnimationFinished(String animName)
	{
		if (animName == "fly")
		{
			AnimatPlay.Play(animName);	
		}
	}	
}


