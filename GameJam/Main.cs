using Godot;
using System;

public partial class Main : Node2D {

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {

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

    void OnDebugButtonPressed()
    {
        GD.Print("hit");
        var levelViewer = GetNode<LevelViewer>("LevelViewer");
        if (levelViewer != null)
        {
            // TODO hook this up (Mabye a debug signal would be better)
            //levelViewer.DebugPollTileMapData();
        }
    }
}
