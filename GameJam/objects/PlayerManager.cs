using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

// This class is used for morphing between players and handling whatever.
public partial class PlayerManager : Node2D {
    [Signal]
	public delegate void OnPlayerChangedEventHandler(GameObject player);

    [Export]
    public PackedScene WitchScene { get; set; }

    [Export]
    public PackedScene FoxScene { get; set; }

    [Export]
    public PackedScene SalamanderScene { get; set; }

    [Export]
    public PackedScene GargoyleScene { get; set; }

    Player player;
    private enum MorphState { witch, fox, salamander, gargoyle };
    private MorphState currentMorph;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        player = WitchScene.Instantiate<Player>();
        currentMorph = MorphState.witch;
        AddChild(player);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
    }

    public override void _Input(InputEvent @event) {
        // Using try-finally block to trigger a code block after a return
        try {

            if (@event.IsActionPressed("ui_left")) {
                player.MoveLeft();
            } else if (@event.IsActionPressed("ui_right")) {
                player.MoveRight();
            } else if (@event.IsActionPressed("ui_up")) {
                player.MoveUp();
            } else if (@event.IsActionPressed("ui_down")) {
                player.MoveDown();
            }
            Func<MorphState, bool> Morph = (MorphState state) => {
                if (state != currentMorph) {
                    Vector2I oldTileCoords = player.tileCoords;

                    player.QueueFree();
                    GD.Print("Morphing to: ", state);
                    switch (state) {
                        case MorphState.witch:
                            player = WitchScene.Instantiate<Player>();
                            break;
                        case MorphState.fox:
                            player = FoxScene.Instantiate<Player>();
                            break;
                        case MorphState.salamander:
                            player = SalamanderScene.Instantiate<Player>();
                            break;
                        case MorphState.gargoyle:
                            player = GargoyleScene.Instantiate<Player>();
                            break;
                    }
                    currentMorph = state;
                    AddChild(player);
                    player.OverrideTileCoords(oldTileCoords);
                    return true;
                }
                return false;
            };

            if (@event.IsActionPressed("morph_witch")) {
                Morph(MorphState.witch);
            } else if (@event.IsActionPressed("morph_fox")) {
                Morph(MorphState.fox);
            } else if (@event.IsActionPressed("morph_salamander")) {
                Morph(MorphState.salamander);
            } else if (@event.IsActionPressed("morph_gargoyle")) {
                Morph(MorphState.gargoyle);
            }
        } finally {
            EmitSignal(SignalName.OnPlayerChanged, player);
        }
    }

    public void MovePlayerTo(Vector2I coords) {
        try {
            player.MoveTo(coords, "none");
        } finally {
            EmitSignal(SignalName.OnPlayerChanged, player);
        }
    }
}
