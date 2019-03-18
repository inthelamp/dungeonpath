/*************************************************************************/
/*  HUD.cs                                                               */
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

public class HUD : CanvasLayer
{
	[Signal] public delegate void QuitGame();
	[Signal] public delegate void GameOver();	
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
		hp.SetMax(player.MaxHP);
		player.CurrentHP = player.MaxHP;
		hp.SetValue(player.CurrentHP);

		var mp = (MagicPoint)GetNode("Status/MagicPoint");
		mp.SetMax(player.MaxMP);
		player.CurrentMP = player.MaxMP;
		mp.SetValue(player.CurrentMP);

		var exp = (ExperiencePoint)GetNode("Status/ExperiencePoint");
		exp.SetMax(player.MaxEXP);
		exp.SetValue(player.CurrentEXP);

		//If it has no experience points, i.e. 0, the value is the same as its old value, and
		//doesn't call OnValueChanged(), leaving Experience Points bar uninitialized.
		//So call OnValueChanged() intentionally here.
		if (player.CurrentEXP == 0)
		{
			exp.OnValueChanged(player.CurrentEXP);
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

	public async void ShowGameOver()
	{
		var messageTimer = (Timer)GetNode("MessageTimer");

		ShowMessage("Game Over");
		await ToSignal(messageTimer, "timeout");

		//Save game at this point
		SaveGame();

		//Signal to World
		EmitSignal("GameOver");		
	}

	public async void ShowLevelUp()
	{
		var messageTimer = (Timer)GetNode("MessageTimer");

		ShowMessage("Level UP");
		
		//Take enough time to show this level up
		await ToSignal(messageTimer, "timeout");
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

	//Save the game before the end of the world
	private void SaveGame()
	{
		var main = (Main)GetNode("/root/Main");
		if (main != null)
		{
			main.SaveGame();
		}		
	}

	private void OnQuitButtonPressed()
	{
		//Save game at this point
		SaveGame();

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

