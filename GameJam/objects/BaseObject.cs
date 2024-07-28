using Godot;
using System;

// A base object for other objects to inherit from.
public partial class BaseObject : Node2D {
    // Fuck it hard code the pixel size herePixel Size
	protected int cPixelSize { get { return 32; }}

	public Vector2I tileCoords = new Vector2I(0, 0);

	// Called when the node enters the scene tree for the first time.
    public override void _Ready() {
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
    }
}