using System;
using Godot;

public partial class ToggleLight : GameObject
{
    public bool ToggleState = false;

    private AnimatedSprite2D Sprite;

    private Area2D Trigger;

    private LevelViewer Level;

    // the tilemap atlas position that refers to on.
    private static Vector2I OnState = new Vector2I(3, 1);

    // the tilemap atlas position that refers to off.
    private static Vector2I OffState = new Vector2I(1, 1);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        // setup the things.
        Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        Trigger = GetNode<Area2D>("Area2D");
        Level = GetParent<LevelViewer>();

        // set the tile at the current position.
        handleToggle (ToggleState);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void _Input(InputEvent @event)
    {
    }

    public void OnTriggerEnter(Area2D area)
    {
        ToggleOn();
    }

    public void OnTriggerExit(Area2D area)
    {
        ToggleOff();
    }

    /// <summary>
    /// /// handleToggle will manage the light toggle, changing the sprite status and enabling or disabling the light source.
    /// </summary>
    /// <param name="state"></param>
    private void handleToggle(bool state)
    {
        // update the internal state.
        ToggleState = state;
        Sprite.SetFrameAndProgress(ToggleState ? 1 : 0, 0);

        // set the tile in the tilemap.
        Level
            .SetTileData("lights",
            tileCoords,
            10,
            ToggleState ? OnState : OffState);
        Level.OnToggleLightingPressed();
    }

    /// <summary>
    /// ToggleOn will enable this light source.
    /// </summary>
    public void ToggleOn()
    {
        handleToggle(true);
    }

    /// <summary>
    /// ToggleOff will disable this light source.
    /// </summary>
    public void ToggleOff()
    {
        handleToggle(false);
    }
}
