/*************************************************************************/
/*  Intro.cs                                                             */
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

public class Intro : Node
{
	private String _currentAnimationName="";

	public override void _Ready()
	{
		var introAnim = (AnimationPlayer)GetNode("IntroAnimation");
		introAnim.Play("IntroOne");
	}

	private void OnNextButtonPressed()
	{
		//Replace with function body
		var introAnim = (AnimationPlayer)GetNode("IntroAnimation");
		switch (_currentAnimationName)
		{
			case "IntroOne":
			introAnim.Play("IntroTwo");
			break;
			case "IntroTwo":
			introAnim.Play("IntroThree");
			break;
			case "IntroThree":
			introAnim.Play("IntroFour");
			break;
			case "IntroFour":
			LoadLevelOne();
			break;
			default:
			break;
		}
	}

	private void OnIntroAnimationFinished(String anim_name)
	{
		_currentAnimationName = anim_name;
	}

	private void LoadLevelOne()
	{
		PackedScene gameWorldScene = (PackedScene)GD.Load("res://Scene/GameWorld.tscn");
		Node gameWorld = gameWorldScene.Instance();
		GetParent().AddChild(gameWorld);

		GetParent().RemoveChild(this);
		QueueFree();
	}
}
