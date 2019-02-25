using Godot;
using System;

public class HealthPoint : TextureProgress
{

	public override void _Ready()
	{

	}

	public void SetFigureText(string figure)
	{
		var label = (Label)GetNode("Figure");
		label.SetText(figure);
	}

	private void OnValueChanged(float value)
	{
		SetFigureText(((int)value).ToString()+" / "+GetMax().ToString());
	}

}
