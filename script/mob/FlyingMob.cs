/*************************************************************************/
/*  FlyingMob.cs                                                         */
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
