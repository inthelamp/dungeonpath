using Godot;
using System;

public class ShortMessage : Label
{
	[Export]
	public float DisplayTime; //Second


	private float _timer;

    public override void _Ready()
    {

    }

	public void Start()
	{
		SetPosition(new Vector2(
			Constants.RandRand(-90, -70),
			Constants.RandRand(-50, -30)			
		));
	}

	public void Start(Vector2 pos)
	{
		SetPosition(pos);
	}
	
	public override void _Process(float delta)
	{
		_timer += delta;
		if (_timer >= DisplayTime)
		{
			QueueFree();
		}
	}
}



