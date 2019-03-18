/*************************************************************************/
/*  Player.cs                                                            */
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

public class Player : Playable, IPersist
{
  	[Signal]
	public delegate void ShootFireBall();

	private const float Gravity = 500.0f; //pixels/second/second
	private const int WalkForce = 500;  //500 800
	private const int AttackForce = 1000;  //1000
	private const int WalkMinSpeed = 10; //10
	private const int WALK_MAX_SPEED = 200; //200
	private const int StopForce = 1300; //1300
	private const int JumpSpeed = 200; //400
	private const float JumpMaxAirborneTime = 0.2f;

	private bool _isInputJumping;
	private bool _isFacingRight;
	private float _onAirTime;
	private Position2D _muzzle;
	private AnimationPlayer _anim;
	private AnimatedSprite _animatedSprite;
	private TextureRect _attackEffect;
	private CollisionShape2D _circleFormCollision;
	private LongRangeMagicWeapon _wand;

	private Vector2 _velocity = new Vector2();

	public override void _Ready()
	{
		_muzzle = (Position2D)GetNode("Muzzle");
		_anim = (AnimationPlayer)GetNode("Animation");
		_animatedSprite = (AnimatedSprite)GetNode("AnimatedSprite");
		_attackEffect = (TextureRect)GetNode("AttackEffect");
		_wand = (LongRangeMagicWeapon)GetNode("Weapon/Wand");
		_isFacingRight = true;

		//Disable these collisions
		_circleFormCollision = (CollisionShape2D)GetNode("CircleFormCollision");
		_circleFormCollision.Disabled = true;
	}

	public void Start()
	{
		IsEntering = false;

		//Save game at this point
		var main = (Main)GetNode("/root/Main");
		if (main != null)
		{
			main.SaveGame();
		}		
	}

	//float restExp = (CurrentEXP + gainedExp) - MaxEXP;
	//if restExp >= 0, then level-up.
	public override void LevelUp(float restExp)
	{
		++Level;
		MaxHP = GetMaxHPForLevel(Level);
		MaxMP = GetMaxMPForLevel(Level);
		MaxEXP = GetRequiredEXPForLevelUp(Level);
		CurrentHP = MaxHP;
		CurrentMP = MaxMP;
		CurrentEXP = restExp;

		var hud = (HUD)GetParent().GetNode("HUD");

		if (Level != 1)
		{
			hud.Initialize(this);					
			hud.ShowLevelUp();
		}
	}

	/*
	Calculate required experience points to the level
	Formular for required experience to level up
	(A * level + level ^ B) * 10
	A is between 1 and 10 and its default value is 4.
	B is between 1.1 and 2 and its default value is 1.85.
	*/
	public override int GetRequiredEXPForLevelUp(int level)
	{
		++level;
		double results = (Constants.EXPFormularLinearA * level)
		+ Math.Pow(level, Constants.EXPFormularExponentB);
		return (int)Math.Floor(results);
	}

	/*
	Calculate the max health points for the level
	MaxHP = BaseHP + (MaximumPossibleHP - BaseHP) * Level /
	MaximumLevel
	*/
	public override int GetMaxHPForLevel(int level)
	{
		double results = Constants.BaseHP +
		(Constants.MaximumPossibleHP - Constants.BaseHP) *
		level / Constants.MaximumLevel;
		return (int)Math.Floor(results);
	}

	/*
	Calculate the max magic points for the level
	MaxMP = MaxHP * A
	A is between 0.01 and 0.99 and its default value is 0.7.
	*/
	public override int GetMaxMPForLevel(int level)
	{
		return (int)Math.Floor(GetMaxHPForLevel(level) *
		Constants.MPFormularLinearA);
	}

	public override int GetAttackPoints()
	{
        //The constant number can be considerd later to adjust the game balance		
		return (int)(MaxHP/10);
	}

	public override void GetAttacked(int damagePoints)
	{
		//Display the damage points which reduce HP.
		ShowDamagePoints(damagePoints);

		//Update current HP
		CurrentHP -= damagePoints;
		if (CurrentHP < 0)
		{
			CurrentHP = 0;
		}			

		//update HUD
		var hud = GetParent().GetNode("HUD");
		var hp = (HealthPoint)hud.GetNode("Status/HealthPoint");
		hp.SetValue(Convert.ToSingle(CurrentHP));

		if (CurrentHP == 0)
		{
			Die();
		}
	}

	//Update magic points after using it
	public override void UseMagicPoints(int magicPoints)
	{
		//Update current MP
		CurrentMP -= magicPoints;
		if (CurrentMP < 0)
		{
			CurrentMP = 0;
		}		

		//update HUD
		var hud = GetParent().GetNode("HUD");
		var mp = (MagicPoint)hud.GetNode("Status/MagicPoint");
		
		mp.SetValue(Convert.ToSingle(CurrentMP));
	}	

	//Display damage points given by attack from mob
	private void ShowDamagePoints(int damagePoints)
	{
		//load damage-point label
		var damagePointsDisplayScene = (PackedScene)GD.Load(Constants.DamagePointsDisplayFilename);
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
		var expDisplayScene = (PackedScene)GD.Load(Constants.EXPPointsDisplayFilename);
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
		var shortMessageScene = (PackedScene)GD.Load(Constants.ShortMessageDisplayFilename);
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
		var shortMessageScene = (PackedScene)GD.Load(Constants.ShortMessageDisplayFilename);
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

	public override void AddEXP(float exp)
	{
		ShowExperiencePoints((int)exp);
		CurrentEXP += exp;
		if (CurrentEXP >= MaxEXP)
		{
			var restExp = CurrentEXP - MaxEXP;
			LevelUp(restExp);
		}
		else
		{
			var hud = GetParent().GetNode("HUD");
			var expNode = (ExperiencePoint)hud.GetNode("Status/ExperiencePoint");
			expNode.SetValue(CurrentEXP);
		}
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
		var force = new Vector2(0, Gravity);

		if (isInputWalkLeft)
		{
			if (_velocity.x <= WalkMinSpeed && _velocity.x > -WALK_MAX_SPEED)
			{
				force.x -= WalkForce;
				isStop = false;
			}
		}
		else if (isInputWalkRight)
		{
			if (_velocity.x >= -WalkMinSpeed && _velocity.x < WALK_MAX_SPEED)
			{
				force.x += WalkForce;
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
				DisableCombat();
			}
		}
		else if (isInputReadyToFight)
		{	if (!IsReadyToFight) //Watch out enemies
			{
				IsReadyToFight = true;
			}
			else //toggle the key
			{
				DisableCombat();
			}
		}
		else if ((isInputAttackLeft || isInputAttackRight) && !IsInAttack) //Just started the battle
		{
			IsReadyToFight = true;
			IsInAttack = true;

			if (isInputAttackLeft)
			{
				if (_velocity.x <= WalkMinSpeed && _velocity.x > -WALK_MAX_SPEED)
				{
					force.x -= AttackForce;
					isStop = false;
				}
			}
			else if (isInputAttackRight)
			{
				if (_velocity.x >= -WalkMinSpeed && _velocity.x < WALK_MAX_SPEED)
				{
					force.x += AttackForce;
					isStop = false;
				}
			}
		}

		if (isStop)
		{
			var vSign = Math.Sign(_velocity.x);
			var vLength = Math.Abs(_velocity.x);

			vLength -= StopForce * delta;
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
			_onAirTime = JumpMaxAirborneTime;
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

	private void DisableCombat()
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
			velocityY = -JumpSpeed;
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
		if (normalVectToTarget.Dot(normalVectToMuzzle) <= Constants.MinimumFaceTargetAngle)
		{
			ShowShortMessage("Face the target!", new Vector2(-70, 40));
			return;
		}

		//Check if this feature needs magic points and 
		//this player still has magic points.
		if (_wand.IsInGroup("MagicPointsRequired"))
		{
			//Luckily, if it has 1 MP, it can still use the magic by the creator who is creating this world.			
			if (CurrentMP == 0)
			{
				ShowShortMessage("No magic points left!", new Vector2(-70, 40));				
				return;
			}
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

			//The current magic point is reduced by the required magic points.
			UseMagicPoints(_wand.GetMagicPoints());

			EmitSignal("ShootFireBall");

			attackAsset.IsEnabled = false;
		}
	}

	private void Die()
	{
		//Set player's status to Dead
		IsDead = true;

		//Disable collisions
		SetCollisionsEnabled(false);

		//Disable combat ability after loss of its life
		DisableCombat();

		Hide(); //Player disappears after being hit.

		//Display a message the player is dead now.
		var hud = (HUD)GetParent().GetNode("HUD");	
		hud.ShowGameOver();
	}

	//Set collisions with enemies and objects
	private void SetCollisionsEnabled(bool isCollisionEnabled)
	{
		//Collision layer and mask bits start from 0.
		SetCollisionLayerBit(1, isCollisionEnabled);
		SetCollisionMaskBit(2, isCollisionEnabled);
		SetCollisionMaskBit(3, isCollisionEnabled);	
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

	//Properties to save, implementing the method of IPersist
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
			{ "MaxHP", MaxHP },
			{ "MaxMP", MaxMP },
			{ "MaxEXP", MaxEXP },
			{ "CurrentHP", CurrentHP },
			{ "CurrentMP", CurrentMP },
			{ "CurrentEXP", CurrentEXP },
			{ "IsDead", IsDead }
		};
	}
}
