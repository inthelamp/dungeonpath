using Godot;
using System;

public class CircleProgress : TextureProgress
{
	[Export] public float LatencyTime;
	[Export] public string FeatureName;	
	[Signal] public delegate void EndCircleProgress();

	private float _timer;
	
	public override void _Ready()
	{
		SetProcess(false);
	}
	
	public void Start()
	{
		SetProcess(true);
		SetMax(LatencyTime);
		_timer = 0;	
	}
	
	public override void _Process(float delta)
	{
		_timer += delta;
		if (GetValue() == LatencyTime)
		{
			Reset();
		}
		else
		{
			SetValue(_timer/LatencyTime);
		}
	}

	private void Reset()
	{
		SetValue(0);
		SetProcess(false);		
		EmitSignal("EndCircleProgress", FeatureName);
	}
}
