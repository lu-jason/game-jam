using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

// This class is used for morphing between players and handling whatever.
public partial class PlayerManager : Node2D {
    [Export]
    public PackedScene WitchScene { get; set; }

    [Export]
    public PackedScene FoxScene { get; set; }

    [Export]
    public PackedScene SalamanderScene { get; set; }

    [Export]
    public PackedScene GargoyleScene { get; set; }

    [Signal]
    public delegate void OnPlayerChangeEventHandler(GameObject player);

    Player player;
    private enum MorphState { witch, fox, salamander, gargoyle };
    private MorphState currentMorph;

    static public bool lockInput = false;

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
        // Changing this to not use try-finally as it spams these signals every frame
        // try-finally is used to execute code after a return

        bool performedAction = false;
        // Instead of just fully exiting here we could maybe buffer inputs, idk
        if (lockInput)
        {
            return;
        }

        if (@event.IsActionPressed("ui_left")) {
            player.MoveLeft();
            performedAction = true;
        } else if (@event.IsActionPressed("ui_right")) {
            player.MoveRight();
            performedAction = true;
        } else if (@event.IsActionPressed("ui_up")) {
            player.MoveUp();
            performedAction = true;
        } else if (@event.IsActionPressed("ui_down")) {
            player.MoveDown();
            performedAction = true;
        }
        Func<MorphState, bool> Morph = (MorphState state) => {
            if (state != currentMorph) {
                Vector2I oldTileCoords = player.tileCoords;

                string currentDirection = player.facingDirection;
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
                player.facingDirection = currentDirection;
                player.SetAnimationState("idle", currentDirection);
                AddChild(player);
                player.OverrideTileCoords(oldTileCoords);
                performedAction = true;
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

        if (@event.IsActionPressed("use_ability"))
        {
            // Affect the tile in front of the player.
            string direction = player.facingDirection;
            Vector2I affectedTile = player.tileCoords;
            float rotation = 0;
            if (direction == "up")
            {
                affectedTile.Y -= 1;

            }
            else if (direction == "right")
            {
                rotation = 90;
                affectedTile.X += 1;
            }
            else if (direction == "down")
            {
                rotation = 180;
                affectedTile.Y += 1;
            }
            else if (direction == "left")
            {
                rotation = 270;
                affectedTile.X -= 1;
            }
            else
            {
                GD.Print("Using action with no direction properly set.");
            }

            player.UseAbility(affectedTile);
            var Animation = player.GetAbilityAnimation();
            if (Animation != null)
            {
                // Add 16 for offset
                // More hardcoded rubbish
                Animation.Position = new Vector2(affectedTile.X*32+16, affectedTile.Y * 32+16);
                Animation.ZIndex = 10;
                Animation.RotationDegrees = rotation;
                Animation.Play();
                AddChild(Animation);
            }
            performedAction = true;

        }

        if (performedAction)
        {
            EmitSignal(SignalName.OnPlayerChange, player);
        }
    }

    public void MovePlayerTo(Vector2I coords) {
        player.OverrideTileCoords(coords);//, "none");
    }

    public static void SetLockInput(bool value)
    {
        lockInput = value;
    }
}