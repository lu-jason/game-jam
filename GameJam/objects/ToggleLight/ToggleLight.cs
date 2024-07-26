using System;
using Godot;

public partial class ToggleLight : GameObject
{
    public bool ToggleState = false;

    private AnimatedSprite2D Sprite;

    private CollisionShape2D Trigger;

    private LevelViewer Level;

    [Signal]
    public delegate void OnLightsChangedEventHandler(TileMap loadedLevel);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // setup the things.
        Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        Trigger = GetNode<CollisionShape2D>("CollisionShape2D");
        Level = GetParent<LevelViewer>();

        // set the tile at the current position.
        Level
            .SetTileData("lights",
            tileCoords,
            10,
            ToggleState ? new Vector2I(1, 1) : new Vector2I(0, 1));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_left"))
        {
            ToggleOn();
        }
        else if (@event.IsActionPressed("ui_right"))
        {
            ToggleOff();
        }
    }

    /// <summary>
    ///
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
            ToggleState ? new Vector2I(1, 1) : new Vector2I(0, 1));
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
