using Godot;
using System;

public partial class LevelViewer : Node2D
{
	[Export]
	public PackedScene Map { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TileMap Level = Map.Instantiate<TileMap>();
		Level.ZIndex = -1;

		AddChild(Level);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
