using Godot;
using System;

public class LongRangeAttack : Attack
{
    private Vector2 _velocity = new Vector2();
	private Particles2D _explosion; 
	private float _explosionTime;
	private bool _isExplosionStarted;

    public override void _Ready()
    {	
		_explosion = (Particles2D)GetNode("Explosion");
		_explosion.SetVisible(false);
    }

	public override void Start(Vector2 pos, float dir)
	{
		Rotation = dir;
		Position = pos;
		_velocity = new Vector2(Speed, 0).Rotated(Rotation);	

		//Play sound effect
		var sound = (AudioStreamPlayer)GetNode("Sounds/Launch");
		sound.Play();
	}
	
	private void OnVisibilityViewportExited(Godot.Object viewport)
	{
	    QueueFree();
	}	
	
	public override void _PhysicsProcess(float delta)
	{
		KinematicCollision2D collision = MoveAndCollide(_velocity * delta);
		
		if (collision != null) {
			if (!_explosion.Visible)
			{
				_isExplosionStarted = true;
				_explosion.SetVisible(true);
			}		

			if (!IsInAttack && collision.Collider is Mob)
			{
				IsInAttack = true;
				Mob mob = (Mob)collision.Collider;
				Hit(mob);
			}	
		}

		if (_isExplosionStarted)
		{
			_explosionTime += delta;
		}

		if (_explosionTime >= _explosion.Lifetime)
		{			
			QueueFree();
		}
	}

	private void Hit(Mob mob)
	{
		
		var attackSound = (AudioStreamPlayer)GetNode("Sounds/Hit");
		attackSound.Play();	

		var weapon = (Weapon)GetNode(WeaponPath);
		if (weapon == null) 
		{
			return;
		}
			
		var player = (Player)GetNode(weapon.PlayerPath);
		if (player.TargetMob != null && player.TargetMob != mob)
		{
			player.ReleaseTarget();
		}

		if (!mob.IsLockedOn)
		{
			player.LockOnTarget(mob);
		}
	
		var attackPoints = weapon.GetAttackPoints();
		mob.GetAttacked(attackPoints);
	}
}
