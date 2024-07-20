using Godot;
using System;

public partial class Main : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public override void _Draw() {
		for (int x = 0; x < 2048; x += 64) {
			DrawLine(new Vector2(x,0), new Vector2(x, 768), Color.Color8(0,0,0), (float)1.5);
		}
		for (int y = 0; y < 768; y += 64) {
			DrawLine(new Vector2(0,y), new Vector2(2048, y), Color.Color8(0,0,0), 2);
		}
	}
}
