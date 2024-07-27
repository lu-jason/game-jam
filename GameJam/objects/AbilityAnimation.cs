using Godot;
using System;

public partial class AbilityAnimation : AnimatedSprite2D
{
    // Called when the node enters the scene tree for the first time.
    // TODO get this programmatically
    const int NumberOfFrames = 6;
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Frame == 6)
        {
            QueueFree();
        }
    }
}
