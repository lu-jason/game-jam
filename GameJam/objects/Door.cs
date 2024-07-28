using Godot;
using System;

public partial class Door : GameObject {
	// Called when the node enters the scene tree for the first time.
	
	private LevelViewer levelViewer;
	private LightingManager lightingManager;
	public override void _Ready() {
		levelViewer = GetNode<LevelViewer>("/root/Main/LevelViewer");
		lightingManager = GetNode<LightingManager>("/root/Main/LevelViewer/LightingManager");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	public override bool CanMove(Vector2I coords, string direction) {
		return false;
	}
}
