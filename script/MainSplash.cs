using Godot;
using System;

public class MainSplash : Node
{
	[Signal]
	public delegate void StartGame();

    public override void _Ready()
    {

    }

	private void OnStartButtonPressed()
	{
		EmitSignal("StartGame", this);
	}

}
