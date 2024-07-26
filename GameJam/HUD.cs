using Godot;
using System;

public partial class HUD : CanvasLayer {

	private int counter = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	public override void _Input(InputEvent @event) {
		var label = GetNode<Label>("Label");
		if (@event.IsActionPressed("ui_left")) {
			if (label.Text.Contains("left")) {
				counter++;
				label.Text = "left " + "x" + counter;
			} else {
				counter = 1;
				label.Text = "left";
			}
		} else if (@event.IsActionPressed("ui_right")) {
			if (label.Text.Contains("right")) {
				counter++;
				label.Text = "right " + "x" + counter;
			} else {
				counter = 1;
				label.Text = "right";
			}
		} else if (@event.IsActionPressed("ui_up")) {
			if (label.Text.Contains("up")) {
				counter++;
				label.Text = "up " + "x" + counter;
			} else {
				counter = 1;
				label.Text = "up";
			}
		} else if (@event.IsActionPressed("ui_down")) {
			if (label.Text.Contains("down")) {
				counter++;
				label.Text = "down " + "x" + counter;
			} else {
				counter = 1;
				label.Text = "down";
			}
		}
	}
}