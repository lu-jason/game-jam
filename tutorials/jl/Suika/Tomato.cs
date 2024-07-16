using Godot;
using System;

public partial class Tomato : RigidBody2D
{
	public bool Released;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Released = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!Released)
		{
			var mousePosition = GetGlobalMousePosition();
			// GD.Print(mousePosition);
			Position = new Vector2(mousePosition.X, 100);
		}
	}

	public void Start(Vector2 position)
	{
		Freeze = true;
		Show();
		GD.Print(Position);
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventMouseButton)
		{
			if (eventMouseButton.ButtonIndex == MouseButton.Left && !eventMouseButton.Pressed)
			{
				Freeze = false;
				Released = true;
				GD.Print("Left button released");
			}
		}
	}

	private void OnTomatoCollisonBodyEntered(Node2D body)
	{
		if (body is Tomato apple)
		{
			GD.Print("Profit", apple.Name);
		}
	}

}
