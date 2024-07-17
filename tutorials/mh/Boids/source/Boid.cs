using Godot;
using System;

public partial class Boid : Area2D
{
    public bool mRunTest = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
        // Physics state could be thread locked so only safe to call with here

        if (mRunTest)
        { 
            // This all seems overkill and a pretty mid way to go about detecting the nearby boids
            var spaceRid = GetWorld2D().Space;
            var spaceState = PhysicsServer2D.SpaceGetDirectState(spaceRid);

            // Create a circle in the physics engine
            Rid circleRid = PhysicsServer2D.CircleShapeCreate();
            int radius = 128;
            PhysicsServer2D.ShapeSetData(circleRid, radius);

            // Setup a query with the circle and position of this boid
            var query = new PhysicsShapeQueryParameters2D();
            query.ShapeRid = circleRid;
            query.Transform = GlobalTransform;

            GD.Print(GlobalPosition);

            var results = spaceState.IntersectShape(query);

            PhysicsServer2D.FreeRid(circleRid);

            GD.Print(results);
            mRunTest = false; 
        }

    }

    public void OnDebugButtonPressed()
    {
        mRunTest = true;
        //CheckNearbyBoids();
    }

    private void CheckNearbyBoids()
    {
        //GD.Print("Checking Nearby Boids");
        //var range = GetNode<CollisionShape2D>("DetectionRange");


        //Rid circleRid = PhysicsServer2D.CircleShapeCreate();
        //int radius = 64;
        //PhysicsServer2D.ShapeSetData(circleRid, radius);

        //var query = new PhysicsShapeQueryParameters2D();
        //query.ShapeRid = circleRid;
        //query.Transform = GlobalTransform;
        //query.CollisionLayer = 0;

        //var spaceState = GetWorld2d().DirectSpaceState;
        //PhysicsDirectSpaceState2D.
        //var results = IntersectShape(query);
        //// Do the query here somehow

        //PhysicsServer2D.FreeRid(circleRid);


    }
}
