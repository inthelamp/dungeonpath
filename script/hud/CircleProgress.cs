/*************************************************************************/
/*  CircleProgress.cs                                                    */
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

public class CircleProgress : TextureProgress
{
	[Export] public float LatencyTime;
	[Export] public string FeatureName;
	[Signal] public delegate void EndCircleProgress();

	private float _timer;

	public override void _Ready()
	{
		SetProcess(false);
	}

	public void Start()
	{
		SetProcess(true);
		MaxValue = LatencyTime;
		_timer = 0;
	}

	public override void _Process(float delta)
	{
		_timer += delta;
		if (Value == LatencyTime)
		{
			Reset();
		}
		else
		{
			Value =_timer/LatencyTime;
		}
	}

	private void Reset()
	{
		Value = 0;
		SetProcess(false);
		EmitSignal("EndCircleProgress", FeatureName);
	}
}
