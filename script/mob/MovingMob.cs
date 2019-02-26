/*************************************************************************/
/*  MovingMob.cs                                                         */
/*************************************************************************/
/*                       This file is part of:                           */
/*                           DungeonPath                                 */
/*             https://github.com/dwkim263/DungeonPath/wiki              */
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

public class MovingMob : Mob
{
	private readonly String[] DEFAULT_MOVING_TYPES = { "idle", "walk" };

	private int _direction = -1;

	private RayCast2D _rcRight;
	private RayCast2D _rcLeft;

	public override void _Ready()
	{
		_rcRight = (RayCast2D)GetNode("RayCastRight");
		_rcLeft = (RayCast2D)GetNode("RayCastRight");

		MobSprite = (Sprite)GetNode("Sprite");

		//Because of _direction = -1
		MobSprite.Scale = new Vector2(MobSprite.Scale.x * _direction, MobSprite.Scale.y);

		if (MovingTypes == null)
		{
			MovingTypes = DEFAULT_MOVING_TYPES;
		}
		AnimatPlay = (AnimationPlayer)GetNode("Anim");
		Animation = MovingTypes[new Random().Next(0, MovingTypes.Length)];
	}

	public override void _IntegrateForces(Physics2DDirectBodyState state)
	{
		var lv = state.GetLinearVelocity();
		var newAnim = Animation;

		if (IsDead)
		{
			newAnim = "dead";
		}
		else
		{
			newAnim = "walk";

			var wallSide = 0.0;

			for (int i=0; i < state.GetContactCount(); ++i)
			{
				var contactObj = state.GetContactColliderObject(i);
				var contactObjId = state.GetContactColliderId(i);
				var contactLocalNormal = state.GetContactLocalNormal(i);

				if (contactObj != null)
				{
					if (contactObj is Player) //Player is the object to get a collision with this mob.
					{
						Player player = (Player)contactObj;
						Hit(player);
					}
				}

				if (contactLocalNormal.x > 0.9)
				{
					wallSide = 1;
				}
				else if (contactLocalNormal.x < -0.9)
				{
					wallSide = -1;
				}
			}

			if (wallSide != 0 && wallSide != _direction)
			{
				_direction = -_direction;
				MobSprite.Scale = new Vector2(MobSprite.Scale.x * -_direction, MobSprite.Scale.y);
				newAnim = "idle";
			}

			if (_direction < 0 &&  !_rcLeft.IsColliding() && _rcRight.IsColliding())
			{
				_direction = -_direction;
				MobSprite.Scale = new Vector2(MobSprite.Scale.x * -_direction, MobSprite.Scale.y);
				newAnim = "idle";
			}
			else if (_direction > 0 && !_rcRight.IsColliding() && _rcLeft.IsColliding())
			{
				_direction = -_direction;
				MobSprite.Scale = new Vector2(MobSprite.Scale.x * -_direction, MobSprite.Scale.y);
				newAnim = "idle";
			}

			var speed = (int)Constants.RandRand(MinSpeed, MaxSpeed);
			lv.x = _direction * speed;
		}

		if (IsInAttack)
		{
			newAnim = "hit";
		}

		if (Animation != newAnim)
		{
			Animation = newAnim;
			if (_direction > 0)
			{
				AnimatPlay.Play(Animation);
			}
			else
			{
				AnimatPlay.PlayBackwards(Animation);
			}
		}

		state.SetLinearVelocity(lv);
	}
}
