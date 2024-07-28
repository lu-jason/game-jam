using Godot;
using System;

public partial class AbilityAnimation : AnimatedSprite2D
{
    // Called when the node enters the scene tree for the first time.
    // TODO get this programmatically
    int NumberOfFrames = 0;
    public override void _Ready()
    {
        string[] names = SpriteFrames.GetAnimationNames();
        NumberOfFrames = SpriteFrames.GetFrameCount(names[0]);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Frame == NumberOfFrames-1)
        {
            QueueFree();
        }
    }
}
