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



