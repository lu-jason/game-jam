using System.Xml.Schema;
using Godot;

public partial class Fruit : RigidBody2D
{
	[Signal]
	public delegate void HitEventHandler(Fruit fruit1, Fruit fruit2);

	public bool Spawned = false;

	public enum FruitEnum
	{
		cherry, tomato, grape, lemon, orange, apple, pear, peach, pineapple, melon, watermelon
	}

	[Export]
	public FruitEnum FruitType;

	public bool Released;

	public bool Combining = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Released = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!Released && !Spawned)
		{
			var mousePosition = GetGlobalMousePosition();
			// GD.Print(mousePosition);
			Position = new Vector2(mousePosition.X, 100);
		}
	}

	public void Start()
	{
		Freeze = true;
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

	private void OnBodyEntered(Node body)
	{
		GD.Print("Profit???", body.Name);
		if (body is Fruit fruit && fruit.FruitType == FruitType)
		{

			if (!Combining || !fruit.Combining)
			{
				EmitSignal(SignalName.Hit, this, fruit);

				Combining = true;
				fruit.Combining = true;
			}

		}
	}

}
