using Godot;
using System;

public partial class Main : Node2D
{
	public double morphTimer = 0.0;
	public bool canMorph = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Morphing state
		if (canMorph) { // ! Mainly here for conditions where player is not allowed to morph (idk what that is yet) - Josh
			morphTimer += delta;

			if (Input.IsActionPressed("morph_witch") && morphTimer > 0.5) {
				GD.Print("morphing into witch"); // 1 + num1
				morphTimer = 0;
			}
			if (Input.IsActionPressed("morph_fox") && morphTimer > 0.5) {
				GD.Print("morphing into fox"); // 2 + num2
				morphTimer = 0;
			}
			if (Input.IsActionPressed("morph_salamander") && morphTimer > 0.5) {
				GD.Print("morphing into salamander"); // 3 + num3
				morphTimer = 0;
			}
			if (Input.IsActionPressed("morph_gargoyle") && morphTimer > 0.5) {
				GD.Print("morphing into gargoyle"); // 4 + num4
				morphTimer = 0;
			}
		}
	}

	public override void _Draw() {
		int width = 1152;
		int height = 640;
		int tileSize = 32;
		for (int x = 0; x < width; x += tileSize) {
			DrawLine(new Vector2(x,0), new Vector2(x, height), Color.Color8(0,0,0), (float)1.5);
		}
		for (int y = 0; y < height; y += tileSize) {
			DrawLine(new Vector2(0,y), new Vector2(width, y), Color.Color8(0,0,0), 2);
		}
	}
}
