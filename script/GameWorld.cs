/*************************************************************************/
/*  GameWorld.cs                                                         */
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

public class GameWorld : Node
{
  private HUD _hud;
  private Player _player;
  private Node _enemies;

  public override void _Ready()
  {

  }

  public void GameStart()
  {
    _player = (Player)GetNode("Player");
    _hud = (HUD)GetNode("HUD");
    _hud.Initialize(_player);

    _enemies = GetNode("Enemies");
    var children = _enemies.GetChildren();
    foreach (Mob enemy in children)
    {
	    var mob = (Mob) enemy;
      mob.Start();
    }
  }

  public void GameOver()
  {
    _hud.ShowGameOver();
  }

  private void OnMobDie(Mob mob)
  {
    //Earn experience points from mob
    _player.AddExp(mob.GetExp());

    //Release the mob
    if (_player.TargetMob != null && _player.TargetMob == mob)
    {
      _player.TargetMob = null;
    }

    //Remove the mob
    _enemies.RemoveChild(mob);
    mob.Die();
  }

  private void OnHUDQuitGame()
  {
    var global = (Global)GetNode("/root/Global");
		global.GotoScene(this, "res://scene/EndSplash.tscn");
  }

  //Keep playing
	private void OnBackgroundSoundFinished()
	{
	    var backgroundSound = (AudioStreamPlayer)GetNode("BackgroundSound");
	    backgroundSound.Play();
	}
}
