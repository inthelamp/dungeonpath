using Godot;
using System;

public class Intro : Node
{
    //Member variables here, example:
    //private int a = 2;
    //private string b = "textvar";
	
	private String _currentAnimationName="";
	
    public override void _Ready()
    {
        //Called every time the node is added to the scene.
        //Initialization here
        var introAnim = (AnimationPlayer)GetNode("IntroAnimation");
		introAnim.Play("IntroOne");
    }

//public override void _Process(float delta)
//{
////Called every frame. Delta is time since last frame.
////Update game logic here.
//
//}
	
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






