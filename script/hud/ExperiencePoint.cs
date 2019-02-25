using Godot;
using System;

public class ExperiencePoint : TextureProgress
{

	public override void _Ready()
	{

	}

	public void SetFigureText(string figure)
	{
		var label = (Label)GetNode("Figure");
		label.SetText(figure);
	}

	public void OnValueChanged(float value)
	{
		SetFigureText(((int)value).ToString()+" / "+GetMax().ToString());
	}

}
