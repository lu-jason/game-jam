using Godot;
using System;

public partial class GameOver : CanvasLayer {
	private LevelViewer levelViewer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		levelViewer = GetNode<LevelViewer>("/root/Main/LevelViewer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	public void OnRetryButtonPressed() {
		GD.Print("Retry button pressed");
		levelViewer.LoadCurrentLevel();
	}

	public void OnQuitButtonPressed() {
		GD.Print("Quit button pressed");
	}
}
