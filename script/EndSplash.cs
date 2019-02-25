using Godot;
using System;

public class EndSplash : Node
{
	[Signal]
	public delegate void StartGame();

    public override void _Ready()
    {

    }

	private void OnPlayButtonPressed()
	{
		var parent = GetParent();
		if (parent != null)
		{
			this.Connect("StartGame", parent, "WelcomeToTheGameWorld");
		} else {
			GD.Print("No Parent");
		}

		GetTree().Paused = false;
		EmitSignal("StartGame", this);
	}

	private void OnEndButtonPressed()
	{
	    GetTree().Quit();
	}
}
