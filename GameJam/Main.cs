using Godot;
using System;

public partial class Main : Node2D {
	public double morphTimer = 0.0;
	public bool canMorph = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		// Morphing state
		// TODO - may need throttling/debounce to ensure not super spam of morphing
		if (Input.IsActionPressed("morph_witch")) {
			GD.Print("morphing into witch"); // 1 + num1
		}
		if (Input.IsActionPressed("morph_fox")) {
			GD.Print("morphing into fox"); // 2 + num2
		}
		if (Input.IsActionPressed("morph_salamander")) {
			GD.Print("morphing into salamander"); // 3 + num3
		}
		if (Input.IsActionPressed("morph_gargoyle")) {
			GD.Print("morphing into gargoyle"); // 4 + num4
		}


	}

	public override void _Draw() {
		// int width = 1152;
		// int height = 640;
		// int tileSize = 64;
		// for (int x = 0; x < width; x += tileSize) {
		// 	DrawLine(new Vector2(x, 0), new Vector2(x, height), Color.Color8(0, 0, 0), (float)1.5);
		// }
		// for (int y = 0; y < height; y += tileSize) {
		// 	DrawLine(new Vector2(0, y), new Vector2(width, y), Color.Color8(0, 0, 0), 2);
		// }
	}
}
