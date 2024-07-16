using Godot;
using System;
using System.Threading.Tasks;

public partial class Main : Node2D
{
	[Export]
	public PackedScene AppleScene { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		Apple apple = AppleScene.Instantiate<Apple>();

		var startPosition = GetNode<Marker2D>("StartPosition");
		apple.Start(startPosition.Position);

		AddChild(apple);

		GD.Print("Start");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public override async void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventMouseButton)
		{
			if (eventMouseButton.ButtonIndex == MouseButton.Left && !eventMouseButton.Pressed)
			{
				Apple apple = AppleScene.Instantiate<Apple>();

				var startPosition = GetNode<Marker2D>("StartPosition");
				apple.Start(startPosition.Position);

				await Task.Delay(1000);

				AddChild(apple);

				GD.Print("Left button released");
			}
		}
	}
}
