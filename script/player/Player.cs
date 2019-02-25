using Godot;
using System;

public class Player : Playable, IPersist
{
  	[Signal]
	public delegate void ShootFireBall();

	private const float GRAVITY = 500.0f; //pixels/second/second
	private const int WALK_FORCE = 500;  //500 800
	private const int ATTACK_FORCE = 1000;  //1000	
	private const int WALK_MIN_SPEED = 10; //10
	private const int WALK_MAX_SPEED = 200; //200
	private const int STOP_FORCE = 1300; //1300
	private const int JUMP_SPEED = 200; //400
	private const float JUMP_MAX_AIRBORNE_TIME = 0.2f;

	private bool _isInputJumping;
	private bool _isFacingRight;
	private float _onAirTime;
	private Position2D _muzzle;
	private AnimationPlayer _anim;
	private AnimatedSprite _animatedSprite;
	private TextureRect _attackEffect;
	private CollisionShape2D _circleFormCollision;
	private Weapon _wand;

	private Vector2 _velocity = new Vector2();

	public override void _Ready()
	{	
		_muzzle = (Position2D)GetNode("Muzzle");
		_anim = (AnimationPlayer)GetNode("Animation"); 
		_animatedSprite = (AnimatedSprite)GetNode("AnimatedSprite");
		_attackEffect = (TextureRect)GetNode("AttackEffect");
		_wand = (Weapon)GetNode("Weapon/Wand");		
		_isFacingRight = true;		

		//Disable these collisions
		_circleFormCollision = (CollisionShape2D)GetNode("CircleFormCollision");
		_circleFormCollision.Disabled = true;	
	}

	public void Enter(Vector2 pos)
	{
		Position = pos;
		Show();
		IsEntered = true;
	}

	/*
	Calculate required experience points to the level
	Formular for required experience to level up
	(A * level + level ^ B) * 10
	A is between 1 and 10 and its default value is 4.
	B is between 1.1 and 2 and its default value is 1.85.
	*/
	public override int GetRequiredExpForLevelUp(int level)
	{
		++level;
		double results = (Constants.EXP_FORMULAR_LINEAR_A * level)
		+ Math.Pow(level, Constants.EXP_FORMULAR_EXPONENT_B);
		return (int)Math.Floor(results);
	}

	/*
	Calculate the max health points for the level
	MaxHp = BASE_HP + (MAXIMUM_POSSIBLE_HP - BASE_HP) * Level /
	MAXIMUM_LEVEL
	*/
	public override int GetMaxHpForLevel(int level)
	{
		double results = Constants.BASE_HP +
		(Constants.MAXIMUM_POSSIBLE_HP - Constants.BASE_HP) *
		level / Constants.MAXIMUM_LEVEL;
		return (int)Math.Floor(results);
	}

	/*
	Calculate the max magic points for the level
	MaxMp = MaxHp * A
	A is between 0.01 and 0.99 and its default value is 0.7.
	*/
	public override int GetMaxMpForLevel(int level)
	{
		return (int)Math.Floor(GetMaxHpForLevel(level) *
		Constants.MP_FORMULAR_LINEAR_A);
	}

	public override int GetAttackPoints()
	{
		return (int)(MaxHp/10);
	}

	public override void GetAttacked(int damagePoints)
	{
		//Display the damage points which reduce HP.
		ShowDamagePoints(damagePoints);

		//Update current HP
		CurrentHp -= damagePoints;		

		//update HUD
		var hud = GetParent().GetNode("HUD");
		var hp = (HealthPoint)hud.GetNode("Status/HealthPoint");
		hp.SetValue(Convert.ToSingle(CurrentHp));	

		if (CurrentHp <= 0)
		{
			Die();
		}
	}

	//Display damage points given by attack from mob
	private void ShowDamagePoints(int damagePoints)
	{
		//load damage-point label	
		var damagePointsDisplayScene = (PackedScene)GD.Load(Constants.DAMAGE_POINTS_DISPLAY_FILENAME);
		if (damagePointsDisplayScene == null) 
		{
			return;  //Error handling
		}
		var damagePointsDisplay = (ShortMessage)damagePointsDisplayScene.Instance();
		
		//update label
		damagePointsDisplay.SetText("-" + damagePoints.ToString());
		AddChild(damagePointsDisplay);
		damagePointsDisplay.Start();
	}

	//Display experience points gained from mob
	private void ShowExperiencePoints(int exp)
	{
		//load experience-point label	
		var expDisplayScene = (PackedScene)GD.Load(Constants.EXP_POINTS_DISPLAY_FILENAME);
		if (expDisplayScene == null) 
		{
			return;  //Error handling
		}
		var expDisplay = (ShortMessage)expDisplayScene.Instance();
		
		//update the label
		expDisplay.SetText("+" + exp.ToString());
		AddChild(expDisplay);
		expDisplay.Start();
	}

	//Display short message
	private void ShowShortMessage(string message)
	{
		//load experience-point label	
		var shortMessageScene = (PackedScene)GD.Load(Constants.SHORT_MESSAGE_DISPLAY_FILENAME);
		if (shortMessageScene == null) 
		{
			return;  //Error handling
		}
		var shortMessage = (ShortMessage)shortMessageScene.Instance();
		
		//update the label
		shortMessage.SetText(message);
		AddChild(shortMessage);
		shortMessage.Start();
	}

	private void ShowShortMessage(string message, Vector2 pos)
	{
		//load experience-point label	
		var shortMessageScene = (PackedScene)GD.Load(Constants.SHORT_MESSAGE_DISPLAY_FILENAME);
		if (shortMessageScene == null) 
		{
			return;  //Error handling
		}
		var shortMessage = (ShortMessage)shortMessageScene.Instance();
		
		//update the label
		shortMessage.SetText(message);
		AddChild(shortMessage);
		shortMessage.Start(pos);
	}

	public override void AddExp(float exp)
	{
		ShowExperiencePoints((int)exp);
		CurrentExp += exp;
		if (CurrentExp >= MaxExp)
		{
			var restExp = CurrentExp - MaxExp;
			LevelUp(restExp);
		}
		else
		{
			var hud = GetParent().GetNode("HUD");
			var expNode = (ExperiencePoint)hud.GetNode("Status/ExperiencePoint");
			expNode.SetValue(CurrentExp);			
		}
	}

	//float restExp = (CurrentExp + gainedExp) - MaxExp;
	//if restExp >= 0, then level-up.
	public override void LevelUp(float restExp)
	{
		++Level;
		MaxHp = GetMaxHpForLevel(Level);
		MaxMp = GetMaxMpForLevel(Level);
		MaxExp = GetRequiredExpForLevelUp(Level);
		CurrentHp = MaxHp;
		CurrentMp = MaxMp;
		CurrentExp = restExp;
		
		var hud = (HUD)GetParent().GetNode("HUD");
		hud.Initialize(this);
	}

	public void SetIsJumping(bool isInputJumping)
	{
		_isInputJumping = isInputJumping;
	}

	//Release the target locked on
	public override void ReleaseTarget()
	{
		if (TargetMob.IsLockedOn)	
		{
			TargetMob.SetMobReleased();
		}	
		TargetMob = null;		
	}

	//Lock on the target
	public override void LockOnTarget(Mob mob)
	{
		TargetMob = mob;
		if (!mob.IsLockedOn)	
		{
			mob.SetMobLockedOn(this);
		}	
	}

	private void GetInput(float delta)
	{
		bool isInputWalkRight = Input.IsActionPressed("ui_right");
		bool isInputWalkLeft = Input.IsActionPressed("ui_left");
		bool isInputJump = Input.IsActionPressed("ui_select");
		bool isInputReadyToFight = Input.IsActionJustPressed("ui_up");

		//Changing form to Circle being ready to fight.
		bool isInputFormToCircle = Input.IsActionJustPressed("circle_form");
		bool isInputAttackLeft = Input.IsActionJustPressed("left_attack");	
		bool isInputAttackRight = Input.IsActionJustPressed("right_attack");	

		bool isStop = true;
		var force = new Vector2(0, GRAVITY);

		if (isInputWalkLeft) 
		{
			if (_velocity.x <= WALK_MIN_SPEED && _velocity.x > -WALK_MAX_SPEED)
			{
				force.x -= WALK_FORCE;
				isStop = false;
			}
		} 
		else if (isInputWalkRight) 
		{
			if (_velocity.x >= -WALK_MIN_SPEED && _velocity.x < WALK_MAX_SPEED)
			{
				force.x += WALK_FORCE;
				isStop = false;
			}
		} 
		else if (isInputFormToCircle) //Changing form to Circle being ready to fight
		{	
			if (!IsCircleForm)
			{
				IsReadyToFight = true;
				IsCircleForm = true;

				//Enable this collision	
				_circleFormCollision.Show();
				_circleFormCollision.Disabled = false;
			}
			else
			{
				ResetCombatFlags();
			}
		}
		else if (isInputReadyToFight)
		{	if (!IsReadyToFight) //Watch out enemies
			{
				IsReadyToFight = true;
			}
			else //toggle the key
			{
				ResetCombatFlags();					
			}
		} 
		else if ((isInputAttackLeft || isInputAttackRight) && !IsInAttack) //Just started the battle
		{
			IsReadyToFight = true;
			IsInAttack = true;

			if (isInputAttackLeft) 
			{
				if (_velocity.x <= WALK_MIN_SPEED && _velocity.x > -WALK_MAX_SPEED)
				{
					force.x -= ATTACK_FORCE;
					isStop = false;
				}
			} 
			else if (isInputAttackRight) 
			{
				if (_velocity.x >= -WALK_MIN_SPEED && _velocity.x < WALK_MAX_SPEED)
				{
					force.x += ATTACK_FORCE;
					isStop = false;
				}
			} 				
		} 

		if (isStop) 
		{
			var vSign = Math.Sign(_velocity.x);
			var vLength = Math.Abs(_velocity.x);

			vLength -= STOP_FORCE * delta;
			if (vLength < 0) 
			{
				vLength = 0;
			}
			_velocity.x = vLength * vSign;
		}

		//Player can jump only from the floor. 
		if (isInputJump && IsOnFloor()) 
		{
			_isInputJumping = true;
			_onAirTime = JUMP_MAX_AIRBORNE_TIME;
		}

		_velocity += force * delta;
	}

	public override void _PhysicsProcess(float delta)
	{
		GetInput(delta);
		_velocity = MoveAndSlide(_velocity, new Vector2(0, -1)); //MoveAndSlide(motion, floor_normal)

		if (IsInAttack) //Attacking
		{
			Attack(_velocity);
		}
		else //Normal movements
		{
			_animatedSprite.Play();

			if (_isInputJumping) //Jumping
			{
				_velocity.y = Jump(delta, _velocity.y);
			} 
			else if (_velocity.Length() > 0 && IsOnFloor()) //Walking
			{
				Walk(_velocity.x);	
			} 
			else  //Staying at the same spot
			{
				Idle();
			}
		}

		if (TargetMob != null && !TargetMob.IsLockedOn)
		{
			TargetMob = null;
		}
	}

	private void ResetCombatFlags()
	{
		IsReadyToFight = false;
		IsCircleForm = false;

		//Disable these collisions
		_circleFormCollision.Hide();		
		_circleFormCollision.Disabled = true;	
	}

	private void OnHudSpotTarget()
	{
		//Release the object first if player has already one,
		//and then consider the spot the mouse has just clicked at. 
		if (TargetMob != null)
		{
			ReleaseTarget();	
		}	

		var targetPosition = GetGlobalMousePosition();
		var spaceState = GetWorld2d().DirectSpaceState;
		var resultArray = spaceState.IntersectPoint(targetPosition);
		if (resultArray != null && resultArray.Count > 0)
		{
			var result = (Dictionary)resultArray[0];
			if (result != null && result.ContainsKey("collider"))		
			{	
				if (result["collider"] is Mob)
				{
					var mob = (Mob)result["collider"];
					LockOnTarget(mob);	
				}						
			}
		}		
	}

	private void Attack(Vector2 velocity)
	{
		if (IsCircleForm)
		{
			if (velocity.x < 0)
			{   
				//Attack effect at the left side of the screen
				var pos = _attackEffect.GetPosition();					
				_attackEffect.SetPosition(new Vector2(Math.Abs(pos.x) * -1, pos.y));
				_attackEffect.SetScale(new Vector2(-1, 1));

				_anim.Play("attackLeft");
			} else {
				//Attack effect at the right side of the screen
				var pos = _attackEffect.GetPosition();					
				_attackEffect.SetPosition(new Vector2(Math.Abs(pos.x), pos.y));
				_attackEffect.SetScale(new Vector2(1, 1));

				_anim.Play("attackRight");					
			}				

			KinematicCollision2D collision = MoveAndCollide(velocity);	
			if (collision != null && collision.Collider is Mob)
			{
				Hit((Mob)collision.Collider);
			}					
		} 
		else 
		{	
			if (TargetMob != null && TargetMob.IsLockedOn)
			{	
				var posX = velocity.x;
				if (posX == 0)
				{
					if (_isFacingRight)
					{
						posX = 16; //Any number greater than 0
					}
					else
					{
						posX = -16; //Any number less than 0
					}
				}

				Walk(posX);
				Shoot(TargetMob.Position);
			}
		}
		IsInAttack = false; //Because the above block should process only one time per an attack 
	}

	private void Idle()
	{
		if (IsReadyToFight)
		{
			if (IsCircleForm)
			{
				_animatedSprite.Animation = "circle";
			}
			else
			{
				_animatedSprite.Animation = "idle-wand";
			}
		}
		else
		{
			_animatedSprite.Animation = "idle";
		}
	}

	private float Jump(float delta, float velocityY)
	{
		if (IsReadyToFight)
		{
			if (IsCircleForm)
			{
				_animatedSprite.Animation = "rolling";
			}
			else
			{
				_animatedSprite.Animation = "jump-wand";
			}
		}
		else
		{
			_animatedSprite.Animation = "jump";
		}
		if (_onAirTime > 0)
		{
			velocityY = -JUMP_SPEED;
			_onAirTime -= delta;
		}
		if (IsOnFloor()) 
		{
			_isInputJumping = false;
		}

		return velocityY;
	}

	private void Walk(float velocityX)
	{
		if (IsReadyToFight)
		{
			if (IsCircleForm)
			{
				_animatedSprite.Animation = "rolling";
			}
			else
			{
				_animatedSprite.Animation = "walk-wand";
			}
		}
		else
		{
			_animatedSprite.Animation = "walk";
		}

		if (velocityX < 0)
		{   
			var pos = _muzzle.GetPosition();
			_muzzle.SetPosition(new Vector2(Math.Abs(pos.x) * -1, pos.y));

			_isFacingRight = false;
			_animatedSprite.FlipH = true;
		} else {
			var pos = _muzzle.GetPosition();
			_muzzle.SetPosition(new Vector2(Math.Abs(pos.x), pos.y));

			_isFacingRight = true;
			_animatedSprite.FlipH = false;
		}
		_animatedSprite.FlipV = false;
	}

	//Hand-to-hand combat
	private void Hit(Mob mob)
	{
		_attackEffect.Show(); //The animation hides this effect at the end of the animation.

		var attackSound = (AudioStreamPlayer)GetNode("Sounds/Hit");
		attackSound.Play();

		if (TargetMob != null && TargetMob != mob)
		{
			ReleaseTarget();	
		}
			
		if (!mob.IsLockedOn)
		{
			LockOnTarget(mob);
		}		

		mob.GetAttacked(GetAttackPoints());
	}

	//Long-range attack
	private void Shoot(Vector2 targetPosition)
	{
		//For checking if it faces target.
		var pos = new Vector2(Position.x, _muzzle.GlobalPosition.y);
		var normalVectToTarget = (targetPosition - pos).Normalized();
		var normalVectToMuzzle = (_muzzle.GlobalPosition - pos).Normalized();

		//Player should face target.
		if (normalVectToTarget.Dot(normalVectToMuzzle) <= Constants.MINIMUM_FACE_TARGET_ANGLE)
		{
			ShowShortMessage("Face the target!", new Vector2(-70, 40));
			return;
		}

		//Create a fireball			
		_wand.PlayerPath = GetPath();

		var attackAsset = (AttackAsset)_wand.AttackAssets["FireBall"];
		if (attackAsset.IsEnabled)
		{
			//Load attack
			var attackScene = (PackedScene)GD.Load(attackAsset.FileName);
			if (attackScene == null) 
			{
				return; //Error handling
			}

			var attack = (LongRangeAttack)attackScene.Instance(); 			  //Instance attack	
			attack.WeaponPath = _wand.GetPath();
			attack.Speed = attackAsset.Speed;              					  //Set speed	
			var rotation = (targetPosition - _muzzle.GlobalPosition).Angle(); //Aim at target			
			attack.Start(_muzzle.GlobalPosition, rotation); 				  //Fire
			GetParent().AddChild(attack);

			EmitSignal("ShootFireBall");
			
			attackAsset.IsEnabled = false;
		}
	}

	private void Die()
	{
		Hide(); //Player disappears after being hit.
	}

	private void OnHudEnableFeature(string featureName)
	{
		switch (featureName)
		{
			case "FireBall":
				var attackAsset = (AttackAsset)_wand.AttackAssets[featureName];
				attackAsset.IsEnabled = true;
				break;
			default:
				GD.Print("default");
				break;
		}
	}

	private void OnHudCircleButtonPressed(string featureName)
	{
		IsCircleForm = false;
		IsReadyToFight = true;
		IsInAttack = true;
	}	

	//Properties to save
	public Dictionary<object, object> Save()
	{
		var path = GetParent().GetPath().ToString();
		return new Dictionary<object, object>()
		{
			{ "Stage", Stage },			
			{ "Name", Name },
			{ "Filename", GetFilename() },
			{ "Parent", path },
			{ "PosX", Position.x }, //Vector2 is not supported by JSON
			{ "PosY", Position.y },
			{ "Level", Level },
			{ "MaxHp", MaxHp },
			{ "MaxMp", MaxMp },
			{ "MaxExp", MaxExp },
			{ "CurrentHp", CurrentHp },
			{ "CurrentMp", CurrentMp },
			{ "CurrentExp", CurrentExp },
			{ "IsReadyToFight", IsReadyToFight }
		};
	}
}