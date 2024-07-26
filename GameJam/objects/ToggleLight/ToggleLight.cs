using System;
using Godot;

public partial class ToggleLight : GameObject
{
    public bool ToggleState = false;

    private AnimatedSprite2D Sprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Sprite = instance.GetChild<AnimatedSprite2D>();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void ToggleOn()
    {
        ToggleState = true;
    }

    public void ToggleOff()
    {
        ToggleState = false;
    }
}
