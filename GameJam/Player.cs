using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public int animationSpeed = 5;
	public bool moving = false;
	public int tileSize = 32;
	public Dictionary<string, Vector2> inputs = new Dictionary<string, Vector2>();
	public RayCast2D ray;

	public override void _Ready()
	{
		// position setting
		inputs.Add("ui_right", Vector2.Right);
		inputs.Add("ui_left", Vector2.Left);
		inputs.Add("ui_up", Vector2.Up);
		inputs.Add("ui_down", Vector2.Down);
		Position = Position.Snapped(Vector2.One * tileSize);
		Position += Vector2.One * tileSize / 2;

		ray = GetNode<RayCast2D>("RayCast2D");
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (moving)
		{
			return;
		}
		foreach (var direction in inputs.Keys)
		{
			if (@event.IsActionPressed(direction))
			{
				// MoveBasic(direction);
				Move(direction);
			}
		}
	}

	// public void MoveBasic(string direction){
	// 	Position += inputs[direction] * tileSize;
	// }
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("ui_right") && !moving)
		{
			Move("ui_right");
		}

		if (Input.IsActionPressed("ui_left") && !moving)
		{
			Move("ui_left");
		}

		if (Input.IsActionPressed("ui_down") && !moving)
		{
			Move("ui_down");
		}

		if (Input.IsActionPressed("ui_up") && !moving)
		{
			Move("ui_up");
		}
	}

	private async void Move(string dir)
	{
		var offset = inputs[dir] * tileSize;
		ray.TargetPosition = offset;
		ray.ForceRaycastUpdate();
		if (!ray.IsColliding())
		{
			var tween = GetTree().CreateTween();
			tween.TweenProperty(this, "position", Position + offset, 1.0 / animationSpeed).SetTrans(Tween.TransitionType.Sine);
			moving = true;
			GetNode<AnimationPlayer>("AnimationPlayer").Play(dir);
			await ToSignal(tween, Tween.SignalName.Finished);
			moving = false;
		}
	}
}
