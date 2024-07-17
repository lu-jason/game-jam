using Godot;
using System;

public partial class Main : Node
{
    [Export]
    public PackedScene BoidScene { get; set; }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SpawnBoids();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void SpawnBoids()
    {
        int NumberOfBoids = 10;
        for (int i=0; i<NumberOfBoids; i++)
        {
            Boid boid = BoidScene.Instantiate<Boid>();
            boid.Position = new Vector2(GD.RandRange(50,750), GD.RandRange(50, 750));
            AddChild(boid);

            GD.Print("Created boid at: ", boid.Position);
        }
    }
}
