/*************************************************************************/
/*  Splash.cs                                                            */
/*************************************************************************/
/*                       This file is part of:                           */
/*                           DungeonPath                                 */
/*             https://github.com/inthelamp/dungeonpath                  */
/*************************************************************************/
/* Copyright (c) 2018-2021 Dong Won Kim.                                 */
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

public class Splash : Node
{
	protected const string WorldScene = "res://scene/World.tscn";
	protected const string PlayerScene = "res://scene/player/Player.tscn";	
	protected const string StageConfig = "res://stage/stagesSerialized.json";    
	protected const int FirstStage = 1;

	[Signal]
	public delegate void FinishedLoading();  
	public World GameWorld { get; set; }

	public void Start()
	{
		var parent = GetParent();
		if (parent == null)
		{
			GD.Print("No Parent");			
			return; //Error handling for no parent
		}	

		Connect("FinishedLoading", parent, "OnSplashFinishedLoading");

		//Create world
		if (GameWorld == null)
		{	
			var worldScene = (PackedScene)GD.Load(WorldScene);
			GameWorld = (World)worldScene.Instance(); 		//Instance World		

			var saveGame = new File();
			if (saveGame.FileExists("user://savegame.save"))
			{
				//Load game saved
				LoadGame(saveGame);				
			}
			else
			{
				//Load the first stage
				LoadStage(FirstStage);
			}			
		}		
		else
		{
			//Remove old TileMap and Enemy nodes of World			
			Remove();

			var player = (Player)GameWorld.GetNode("Player");
			//Build new game stage
			LoadStage(player.Stage);
		}

		//Emit signal
		EmitSignal("FinishedLoading", this, GameWorld);	
	}

	protected void Remove()
	{
		//Remove replaceable nodes
		var replaceableNodes = GetTree().GetNodesInGroup("Replaceable");
		foreach (Node replaceableNode in replaceableNodes)
		{
			GameWorld.RemoveChild(replaceableNode);
			replaceableNode.QueueFree();			
		}

		//Remove enemy nodes
		var enemyNodes = GetTree().GetNodesInGroup("Enemies");
		var enemeiesNode = GameWorld.GetNode("Enemies");	
		foreach (Node enemyNode in enemyNodes)
		{
			enemeiesNode.RemoveChild(enemyNode);
			enemyNode.QueueFree();			
		}
	}

	//Load game saved
	private void LoadGame(File saveGame)
	{
		//Load the file line by line and process that dictionary to restore the object it represents
		saveGame.Open("user://savegame.save", File.ModeFlags.Read);

		//Retrieve nodes specified with "Persist", implementing methods of the interface IPersist
		var saveNodes = GetTree().GetNodesInGroup("Persist");
		foreach (Node saveNode in saveNodes)
		{
			saveNode.QueueFree();
		}

		while (!saveGame.EofReached())
		{
			var currentLine = JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<object, object>>(saveGame.GetLine());
			if (currentLine == null)
				continue;

			if (currentLine["Stage"] != null)
			{
				var stage = Convert.ToInt32(currentLine["Stage"]);
				LoadStage(stage);
			}

			Playable newObject = null;
			if (currentLine["Name"].ToString() == "Player")
			{
				newObject = (Playable)GameWorld.GetNode("Player");
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

			if (currentLine["IsDead"] != null)
			{
				newObject.IsDead = (bool)currentLine["IsDead"];
			}

			if (newObject.IsDead)
			{
				//Locate the player at the safe place.
				var startPosition = (Position2D) GameWorld.GetNode("StartPosition");
				newObject.Position = startPosition.Position;			
				newObject.IsDead = false;					
			}
			else
			{
				var posX = Convert.ToSingle(currentLine["PosX"]);
				var posY = Convert.ToSingle(currentLine["PosY"]);
				newObject.Position = new Vector2(posX, posY);				
			}

			if (currentLine["Level"] != null)
			{
				newObject.Level = Convert.ToInt32(currentLine["Level"]);
			}

			if (currentLine["MaxHP"] != null)
			{
				newObject.MaxHP = Convert.ToInt32(currentLine["MaxHP"]);
			}

			if (currentLine["MaxMP"] != null)
			{
				newObject.MaxMP = Convert.ToInt32(currentLine["MaxMP"]);
			}

			if (currentLine["MaxEXP"] != null)
			{
				newObject.MaxEXP = Convert.ToInt32(currentLine["MaxEXP"]);
			}

			if (currentLine["CurrentHP"] != null)
			{
				newObject.CurrentHP = Convert.ToSingle(currentLine["CurrentHP"]);
			}

			if (currentLine["CurrentMP"] != null)
			{
				newObject.CurrentMP = Convert.ToSingle(currentLine["CurrentMP"]);
			}

			if (currentLine["CurrentEXP"] != null)
			{
				newObject.CurrentEXP = Convert.ToSingle(currentLine["CurrentEXP"]);
			}
		}
		saveGame.Close();
	}

	//Load stage with given stage number
	private void LoadStage(int stageNumber)
	{
		//Access stage configuration
		var aFile = new File();
		if (!aFile.FileExists(StageConfig)) 
		{
			return; //Handling error
		}

		aFile.Open(StageConfig, File.ModeFlags.Read);

		while (!aFile.EofReached())
		{
			var currentLine = aFile.GetLine();
			var stages = JsonConvert.DeserializeObject<System.Collections.Generic.List<StageObject>>(currentLine);
			if (stages == null)	
			{
				continue;
			}

			if (stageNumber > stages.Count)
			{
				GD.Print("Put the portal at different position from the position of its previous stage portal.");
				return;
			}
			var stage = stages[stageNumber-1].stage;

			//Set the start position of the player in this stage
			Position2D startPosition = (Position2D)GameWorld.GetNode("StartPosition");
			startPosition.Position = new Vector2(stage.startPosX, stage.startPosY);

			//Set Brackground
			Sprite background = (Sprite)GameWorld.GetNode("ParallaxBackground/ParallaxLayer/Background");
			background.Texture = ResourceLoader.Load(stage.backgroundImg) as Texture;

			//Create TileMap
			var tileMapScene = (PackedScene)GD.Load(stage.mapScene);
			if (tileMapScene == null) 
			{
				return; 		//Error handling
			}	
			var tileMap = tileMapScene.Instance(); 	//Instance TileMap

			//Access HUD node
			CanvasLayer hud = (CanvasLayer)GameWorld.GetNode("HUD");

			//Attach TileMap below HUD node in World
			GameWorld.AddChildBelowNode(hud, tileMap);		
			//Add tileMap to Replaceable group
			tileMap.AddToGroup("Replaceable");				

			//Create objects of the TileMap here
			//Stage portal through which player can go to the next stage
			if (tileMap.HasNode("StagePortal"))
			{
				var portal = (Portal)tileMap.GetNode("StagePortal");
				portal.Connect("Entered", GameWorld, "OnStagePortalEntered");
			}

			//Dealing with enemeies
			//Access Enemies node
			Node enemiesNode = (Node)GameWorld.GetNode("Enemies");
			//Create and add an enemy
			var enemies = (List<Enemy>)stage.enemy;

			//ForEach loop: taking care of each enemy
			enemies.ForEach(delegate(Enemy enemy)
			{
				var enemyScene = (PackedScene)GD.Load(enemy.scene);
				if (enemyScene == null) 
				{
					return; 					//Error handling
				}

				if (enemy.mobPath != null) 							//Is it FlyingMob mobs
				{
					var mobPathScene = (PackedScene)GD.Load(enemy.mobPath);
					if (mobPathScene == null) 
					{
						return; 				//Error handling
					}
					var mobPath = mobPathScene.Instance(); 			//Instance ModPath
					GameWorld.AddChild(mobPath);

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
						enemyNode.Connect("MobDie", GameWorld, "OnMobDie");

						//Add it to Enemies group
						enemyNode.AddToGroup("Enemies");						
					}
				}
				else
				{															//WalkingMob mobs
					var enemyNode = (WalkingMob)enemyScene.Instance(); 		//Instance a mob
					enemyNode.Level = enemy.level;
					enemyNode.MinSpeed= enemy.minSpeed;
					enemyNode.MaxSpeed= enemy.maxSpeed;
					enemyNode.Position = new Vector2(enemy.posX, enemy.posY);
					enemyNode.MovingTypes = enemy.movingTypes;
					enemiesNode.AddChild(enemyNode);
					enemyNode.Connect("MobDie", GameWorld, "OnMobDie");

					//Add it to Enemies group
					enemyNode.AddToGroup("Enemies");						
				}
			}); //ForEach loop: taking care of each enemy

			//Create and add Player
			Player player = null;
			if (!GameWorld.HasNode("Player"))
			{
				var playerScene = (PackedScene)GD.Load(PlayerScene);
				if (playerScene == null) 
				{
					return; 								//Error handling
				}
				player = (Player)playerScene.Instance();
				GameWorld.AddChild(player);

				//Establish Connects between player and hud
				player.Connect("ShootFireBall", hud, "OnPlayerShootFireBall");
				hud.Connect("EnableFeature", player, "OnHudEnableFeature");
				hud.Connect("CircleButtonPressed", player, "OnHudCircleButtonPressed");
				hud.Connect("SpotTarget", player, "OnHudSpotTarget");
			} else {
				player = (Player)GameWorld.GetNode("Player");
			}

			//Set stage
			player.Stage = stageNumber;

			//For player who never played before
			if (player.Level == 0)
			{
				player.LevelUp(0);
			}
	
  			player.Position = startPosition.Position;
		}
		aFile.Close();
	}	

	class Enemy
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

	class Stage
	{
		public int stageNumber { get; set; }
		public int startPosX { get; set; }
		public int startPosY { get; set; }
		public string mapName { get; set; }
		public string mapScene { get; set; }
		public string backgroundImg { get; set; }
		public List<Enemy> enemy { get; set; }
	}

	class StageObject
	{
		public Stage stage { get; set; }
	}

}
