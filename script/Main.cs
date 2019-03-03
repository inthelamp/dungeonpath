/*************************************************************************/
/*  Main.cs                                                              */
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
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Main : Node
{
	private const string ArrowCursorPath = "res://art/hud/arrow-cursor.png";
	private const int FirstStage = 1;

	public override void _Ready()
	{
		//Load the custom images for the mouse cursor
		var arrowCursor = ResourceLoader.Load(ArrowCursorPath);

		Input.SetMouseMode(Input.MouseMode.Confined);
		Input.SetCustomMouseCursor(arrowCursor);
	}

	public void SaveGame()
	{
		var saveGame = new File();
		saveGame.Open("user://savegame.save", (int)File.ModeFlags.Write);

		var saveNodes = GetTree().GetNodesInGroup("Persist");
		foreach (IPersist saveNode in saveNodes)
		{
			var nodeData = saveNode.Save();
			saveGame.StoreLine(JsonConvert.SerializeObject(nodeData));
		}
		saveGame.Close();
	}
	
	public void OnSplashFinishedLoading(Splash splash, World gameWorld)
	{
		if (!this.HasNode("World"))
		{
			AddChild(gameWorld);
		}
		gameWorld.GameStart();	
			
		RemoveChild(splash);
		splash.QueueFree();		
	}
}



