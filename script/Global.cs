/*************************************************************************/
/*  Global.cs                                                            */
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

public class Global : Node
{
  public Node MainScene { get; set; }

  public override void _Ready()
  {
	  Viewport root = GetTree().Root;
	  MainScene = root.GetChild(root.GetChildCount() - 1);
  }

  public void GoToScene(Node currentScene, string path)
  {
	CallDeferred(nameof(DeferredGoToScene), currentScene, path);
  }

  public void GoToSceneWithWorld(World world, string path)
  {
	CallDeferred(nameof(DeferredGoToSceneWithWorld), world, path);
  }

  public void DeferredGoToScene(Node currentScene, string path)
  {
	  //Immediately free the current scene, there is no risk here
	  if (currentScene == null) 
	  { 
		return;
	  }

	  currentScene.Free();

	  //Load a new scene.
	  var nextScene = (PackedScene)GD.Load(path);

	  //Instance the new scene
	  currentScene = nextScene.Instance();

	  //Add it to the active scene, as child of root
	  MainScene.AddChild(currentScene);

	  if (currentScene is Splash)
	  { 
		var splashScene = (Splash)currentScene;
		splashScene.Start();
	  }
  }

  public void DeferredGoToSceneWithWorld(World world, string path)
  {
	  if (world == null) 
	  { 
		return; //Error
	  }

	  //Load a new scene.
	  var nextScene = (PackedScene)GD.Load(path);

	  //Instance the new scene.
	  var currentScene = nextScene.Instance();

	  //Add it to the active scene, as child of root
	  MainScene.AddChild(currentScene);

	  if (currentScene is Splash)
	  { 
		var splashScene = (Splash)currentScene;
		splashScene.GameWorld = world;
		splashScene.Start();
	  }
  } 
}
