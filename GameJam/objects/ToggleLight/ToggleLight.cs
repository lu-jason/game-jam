using System;
using Godot;

public partial class ToggleLight : GameObject
{
    public bool ToggleState = true;
    public int ToggleChannel = 0;
    private int Weight = 0;

    private AnimatedSprite2D Sprite;

    private Area2D Trigger;

    private LevelViewer Level;

    // the tilemap atlas position that refers to on.
    private static Vector2I OnState = new(1, 1);

    // the tilemap atlas position that refers to off.
    private static Vector2I OffState = new(0, 1);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        // setup the things.
        Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        Trigger = GetNode<Area2D>("Area2D");
        Level = GetParent<LevelViewer>();

        // set the tile at the current position.
        UpdateTile();
    }

    public void SetItemData(string data) {
        GD.Print("SetItemData: ", data);
        string[] parts = data.Split(",");
        ToggleState = (parts.Length >= 1) && (parts[0] == "On");
        ToggleChannel = (parts.Length >= 2) ? int.Parse(parts[1]) : -1;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void OnTriggerEnter(Area2D area)
    {
        Weight ++;
        ToggleOn();
    }

    public void OnTriggerExit(Area2D area)
    {
        Weight --;
        if (Weight <= 0) ToggleOff();
    }

    private void UpdateTile() {
        // set the tile in the tilemap.
        Level
            .SetTileData("lights",
            tileCoords,
            10,
            ToggleState ? OnState : OffState);
    }

    public void TriggerChannel(int channel, int state) {
        if (channel != ToggleChannel) return;
        HandleToggle(state != 0);
    }

    /// <summary>
    /// handleToggle will manage the light toggle, changing the sprite status and enabling or disabling the light source.
    /// </summary>
    /// <param name="state"></param>
    private void HandleToggle(bool state)
    {   
        if (state == ToggleState) return;

        // update the internal state.
        ToggleState = state;
        Sprite.SetFrameAndProgress(ToggleState ? 1 : 0, 0);
        UpdateTile();
    }

    /// <summary>
    /// ToggleOn will enable this light source.
    /// </summary>
    public void ToggleOn()
    {
        HandleToggle(true);
    }

    /// <summary>
    /// ToggleOff will disable this light source.
    /// </summary>
    public void ToggleOff()
    {
        HandleToggle(false);
    }
}
