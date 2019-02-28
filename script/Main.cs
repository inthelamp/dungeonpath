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

public class Enemy
{
	public string name { get; set; }
	public int level { get; set; }
	public string scene { get; set; }
	public string mobPath { get; set; }
	public int instances { get; set; }
	public int posX { get; set; }
	public int posY { get; set; }
	public int minSpeed { get; set; }
	public int maxSpeed { get; set; }
	public String[] movingTypes { get; set; }
}

public class Stage
{
	public int stageNumber { get; set; }
	public int startPosX { get; set; }
	public int startPosY { get; set; }
	public string mapName { get; set; }
	public string mapScene { get; set; }
	public string backgroundImg { get; set; }
	public List<Enemy> enemy { get; set; }
}

public class StageObject
{
	public Stage stage { get; set; }
}

public class Main : Node
{
	private const string GAMEWORLD_SCENE = "res://scene/GameWorld.tscn";
	private const string PLAYER_SCENE = "res://scene/player/Player.tscn";
	private const string STAGE_CONFIG = "res://stage/stagesSerialized.json";
	private const string ARROW_CURSOR_PATH = "res://art/hud/arrow-cursor.png";

	private const int FIRST_STAGE = 1;

	public override void _Ready()
	{
		//Load the custom images for the mouse cursor
		var arrowCursor = ResourceLoader.Load(ARROW_CURSOR_PATH);

		//Input.SetMouseMode(Input.MouseMode.Confined);
		Input.SetCustomMouseCursor(arrowCursor);

		//Make a signal connection
		var splash = (MainSplash)GetNode("MainSplash");
		splash.Connect("StartGame", this, "WelcomeToTheGameWorld");
	}

	private void LoadGame(GameWorld gameWorld)
	{
		var saveGame = new File();
		if (!saveGame.FileExists("user://savegame.save"))
		{
			LoadStage(gameWorld, FIRST_STAGE);
			return;
		}

		//Load the file line by line and process that dictionary to restore the object it represents
		saveGame.Open("user://savegame.save", (int)File.ModeFlags.Read);

		//Retrieve nodes specified with "Persist" and implementing methods of the interface IPersist
		var saveNodes = GetTree().GetNodesInGroup("Persist");
		foreach (Node saveNode in saveNodes)	saveNode.QueueFree();

		while (!saveGame.EofReached())
		{
			var currentLine = JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<object, object>>(saveGame.GetLine());
			if (currentLine == null)
				continue;

			if (currentLine["Stage"] != null)
			{
				var stage = Convert.ToInt32(currentLine["Stage"]);
				LoadStage(gameWorld, stage);
			}

			Playable newObject = null;
			if (currentLine["Name"].ToString() == "Player")
			{
				newObject = (Playable)gameWorld.GetNode("Player");
			}
			else
			{
				var newObjectScene = (PackedScene)GD.Load(currentLine["Filename"].ToString());
				if (newObjectScene == null)
				{
					return; //Error handling
				}
				newObject = (Playable)newObjectScene.Instance();
				GetNode(currentLine["Parent"].ToString()).AddChild(newObject);
			}

			var posX = Convert.ToSingle(currentLine["PosX"]);
			var posY = Convert.ToSingle(currentLine["PosY"]);
			newObject.Position = new Vector2(posX, posY);

			if (currentLine["Level"] != null)
			{
				newObject.Level = Convert.ToInt32(currentLine["Level"]);
			}

			if (currentLine["MaxHp"] != null)
			{
				newObject.MaxHp = Convert.ToInt32(currentLine["MaxHp"]);
			}

			if (currentLine["MaxMp"] != null)
			{
				newObject.MaxMp = Convert.ToInt32(currentLine["MaxMp"]);
			}

			if (currentLine["MaxExp"] != null)
			{
				newObject.MaxExp = Convert.ToInt32(currentLine["MaxExp"]);
			}

			if (currentLine["CurrentHp"] != null)
			{
				newObject.CurrentHp = Convert.ToSingle(currentLine["CurrentHp"]);
			}

			if (currentLine["CurrentMp"] != null)
			{
				newObject.CurrentMp = Convert.ToSingle(currentLine["CurrentMp"]);
			}

			if (currentLine["CurrentExp"] != null)
			{
				newObject.CurrentExp = Convert.ToSingle(currentLine["CurrentExp"]);
			}

			if (currentLine["IsReadyToFight"] != null)
			{
				newObject.IsReadyToFight = (bool)currentLine["IsReadyToFight"];
			}
		}
		saveGame.Close();
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

	private void LoadStage(GameWorld gameWorld, int stageNo)
	{
		var aFile = new File();
		if (!aFile.FileExists(STAGE_CONFIG)) return; //Handling error

		aFile.Open(STAGE_CONFIG, (int)File.ModeFlags.Read);

		while (!aFile.EofReached())
		{
			var currentLine = aFile.GetLine();
			var stages = JsonConvert.DeserializeObject<System.Collections.Generic.List<StageObject>>(currentLine);
			if (stages == null)	continue;

			var stage = stages[stageNo-1].stage;

			//Set the start position of the player in this stage
			Position2D startPosition = (Position2D)gameWorld.GetNode("StartPosition");
			startPosition.SetPosition(new Vector2(stage.startPosX, stage.startPosY));

			//Set Brackground
			Sprite background = (Sprite)gameWorld.GetNode("ParallaxBackground/ParallaxLayer/Background");
			var img = new Image();
			var imgTexture = new ImageTexture();
			img.Load(stage.backgroundImg);
			imgTexture.CreateFromImage(img);
			background.SetTexture(imgTexture);

			//Create and add TileMap
			var tileMapScene = (PackedScene)GD.Load(stage.mapScene);
			if (tileMapScene == null) return; //Error handling
			var tileMap = tileMapScene.Instance(); 		//Instance TileMap
			//Access HUD node
			CanvasLayer hud = (CanvasLayer)gameWorld.GetNode("HUD");
			//Attach TileMap below HUD node in GameWorld
			gameWorld.AddChildBelowNode(hud, tileMap);

			//Dealing with enemeies
			//Access Enemies node
			Node enemiesNode = (Node)gameWorld.GetNode("Enemies");
			//Create and add an enemy
			var enemies = (List<Enemy>)stage.enemy;

			//ForEach loop: taking care of each enemy
			enemies.ForEach(delegate(Enemy enemy)
			{
				var enemyScene = (PackedScene)GD.Load(enemy.scene);
				if (enemyScene == null) return; 					//Error handling

				if (enemy.mobPath != null) 							//Is it FlyingMob mobs
				{
					var mobPathScene = (PackedScene)GD.Load(enemy.mobPath);
					if (mobPathScene == null) return; 				//Error handling
					var mobPath = mobPathScene.Instance(); 			//Instance ModPath
					gameWorld.AddChild(mobPath);

					for (int i = 0; i < enemy.instances; ++i)
					{
						var enemyNode = (FlyingMob)enemyScene.Instance(); 	//Instance a mob
						var spawnLocation = (PathFollow2D)mobPath.GetNode("MobSpawnLocation"+" "+ i.ToString());
						enemyNode.SpawnLocation = spawnLocation;
						enemyNode.Level= enemy.level;
						enemyNode.MinSpeed= enemy.minSpeed;
						enemyNode.MaxSpeed= enemy.maxSpeed;
						enemyNode.MovingTypes = enemy.movingTypes;
						enemiesNode.AddChild(enemyNode);
						enemyNode.Connect("MobDie", gameWorld, "OnMobDie");
					}
				}
				else
				{															//MovingMob mobs
					var enemyNode = (MovingMob)enemyScene.Instance(); 		//Instance a mob
					enemyNode.Level = enemy.level;
					enemyNode.MinSpeed= enemy.minSpeed;
					enemyNode.MaxSpeed= enemy.maxSpeed;
					enemyNode.SetPosition(new Vector2(enemy.posX, enemy.posY));
					enemyNode.MovingTypes = enemy.movingTypes;
					enemiesNode.AddChild(enemyNode);
					enemyNode.Connect("MobDie", gameWorld, "OnMobDie");
				}
			}); //ForEach loop: taking care of each enemy

			//Create and add Player
			var playerScene = (PackedScene)GD.Load(PLAYER_SCENE);
			if (playerScene == null) return; 								//Error handling
			var player = (Player)playerScene.Instance();
			gameWorld.AddChild(player);

			//Establish Connects between player and hud
			player.Connect("ShootFireBall", hud, "OnPlayerShootFireBall");
			hud.Connect("EnableFeature", player, "OnHudEnableFeature");
			hud.Connect("CircleButtonPressed", player, "OnHudCircleButtonPressed");
			hud.Connect("SpotTarget", player, "OnHudSpotTarget");

			//For player who never played before
			if (player.Level == 0)
			{
				player.Stage = FIRST_STAGE;
				player.LevelUp(0);
				player.Enter(startPosition.GetPosition());
			} else {
				player.Stage = stageNo;
			}
		}
		aFile.Close();
	}

	public void WelcomeToTheGameWorld(Node triggerNode)
	{
		//Create GameWorld
		var gameWorldScene = (PackedScene)GD.Load(GAMEWORLD_SCENE);
		if (gameWorldScene == null)
		{
			return; //Error handling
		}

		var gameWorld = (GameWorld)gameWorldScene.Instance(); 		//Instance GameWorld
		AddChild(gameWorld);

		LoadGame(gameWorld);
		gameWorld.GameStart();

		RemoveChild(triggerNode);
		triggerNode.QueueFree();
	}

	public void OnQuitGame()
	{
		GetTree().Quit();
	}
}



