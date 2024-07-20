using Godot;
using System;

public partial class LevelViewer : Node2D
{
	[Export]
	public PackedScene Map { get; set; }

	public TileMap loadedLevel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		loadedLevel = Map.Instantiate<TileMap>();
		loadedLevel.ZIndex = -1;

		AddChild(loadedLevel);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
	}
}
