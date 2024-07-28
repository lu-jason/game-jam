using Godot;
using System;

public partial class Door : GameObject {
	// Called when the node enters the scene tree for the first time.
	public override bool CanMove(Vector2I coords, string direction) {
		return false;
	}
}
