/*************************************************************************/
/*  HUD.cs                                                               */
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

public class HUD : CanvasLayer
{
	[Signal] public delegate void QuitGame();
	[Signal] public delegate void EnableFeature();
	[Signal] public delegate void CircleButtonPressed();
	[Signal] public delegate void SpotTarget();

	public override void _Ready()
	{
		//Menu
		var menu = (MenuButton)GetNode("MenuButton");
		var popupMenu = menu.GetPopup();
		popupMenu.Connect("id_pressed", this, "OnMenuIdPressed");
	}

	public void Initialize(Player player)
	{
		var hp = (HealthPoint)GetNode("Status/HealthPoint");
		hp.SetMax(player.MaxHp);
		hp.SetValue(player.CurrentHp);
		var mp = (MagicPoint)GetNode("Status/MagicPoint");
		mp.SetMax(player.MaxMp);
		mp.SetValue(player.CurrentMp);
		var exp = (ExperiencePoint)GetNode("Status/ExperiencePoint");
		exp.SetMax(player.MaxExp);
		exp.SetValue(player.CurrentExp);

		//If it has no experience points, i.e. 0, the value is the same as its old value, and
		//doesn't call OnValueChanged(), leaving Experience Points bar uninitialized.
		//So call OnValueChanged() intentionally here.
		if (player.CurrentExp == 0)
		{
			exp.OnValueChanged(player.CurrentExp);
		}

		var level = (Label)GetNode("Status/Level");
		level.SetText("Level " + player.Level.ToString());
	}

	public void ShowMessage(string text)
	{
		var messageTimer = (Timer)GetNode("MessageTimer");
		var messageLabel = (Label)GetNode("MessageLabel");

		messageLabel.Text = text;
		messageLabel.Show();
		messageTimer.Start();
	}

	async public void ShowGameOver()
	{
		var startButton = (Button)GetNode("StartButton");
		var messageTimer = (Timer)GetNode("MessageTimer");
		var messageLabel = (Label)GetNode("MessageLabel");

		ShowMessage("Game Over");
		await ToSignal(messageTimer, "Timeout");
		messageLabel.Text = "Dodge the\nCreeps!";
		messageLabel.Show();
		startButton.Show();
	}

	public void OnMessageTimerTimeout()
	{
		var messageLabel = (Label)GetNode("MessageLabel");
		messageLabel.Hide();
	}

	private void OnBackToPlayButtonPressed()
	{
		var backToPlayButton = (Button)GetNode("BackToPlayButton");
		var quitButton = (Button)GetNode("QuitButton");
		backToPlayButton.Hide();
		quitButton.Hide();
		GetTree().Paused = false;
	}

	private void OnQuitButtonPressed()
	{
		var main = (Main)GetNode("/root/Main");
		if (main != null)
		{
			main.SaveGame();
		}

		EmitSignal("QuitGame");
	}

	private void OnPlayerShootFireBall()
	{
	    var circleButton = (CircleButton)GetNode("CircleButton/FireBall");
		circleButton.Hide();
	    var circleProgress =(CircleProgress)GetNode("CircleProgress/FireBall");
		circleProgress.Show();
		circleProgress.Start();
	}

	public override void _Process(float delta)
	{
		bool isInputCancel = Input.IsActionPressed("ui__cancel");
		bool isInputMouseLeftClick = Input.IsActionJustPressed("mouse_left_click");
		var circleButton = (HBoxContainer)GetNode("CircleButton");

		//Clicked on a mob to lock on
		if (isInputMouseLeftClick && GetViewport().GetMousePosition().y <= circleButton.GetPosition().y)
		{
			EmitSignal("SpotTarget");
		}
		else if (isInputCancel)
		{
			Stop();
		}
	}

	private void Stop()
	{
		GetTree().Paused = true;

		var backToPlayButton = (Button)GetNode("BackToPlayButton");
		var quitButton = (Button)GetNode("QuitButton");
		backToPlayButton.Show();
		quitButton.Show();
	}

	private void OnEndCircleProgress(string featureName)
	{
	    var circleButton = (CircleButton)GetNode("CircleButton/" + featureName);
		circleButton.Show();
	    var circleProgress = (CircleProgress)GetNode("CircleProgress/" + featureName);
		circleProgress.Hide();
		EmitSignal("EnableFeature", featureName);
	}

	private void OnCircleButtonPressed(string featureName)
	{
		EmitSignal("CircleButtonPressed", featureName);
	}

	//Find which menu was clicked in the list of menus.
	private void OnMenuIdPressed(int id)
	{
		switch (id)
		{
			case 0:	//Setting sound

				break;
			case 1:		//Show key mapping
				var keyMapping = (WindowDialog)GetNode("Menu/KeyMapping");
				keyMapping.Visible = true;
				keyMapping.Show();
				break;
			case 3:    //Exit
				Stop();
				break;
			default:
			break;
		}
	}
}
