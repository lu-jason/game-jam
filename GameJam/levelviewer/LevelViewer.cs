using Godot;
using System;
using System.Numerics;

public partial class LevelViewer : Node2D {
    [Export]
    public PackedScene Map { get; set; }

    [Export]
    public PackedScene Rock { get; set; }

    [Signal]
    public delegate void OnLoadLevelEventHandler(TileMap LoadedLevel);

    [Signal]
    public delegate void OnLightsChangedEventHandler(TileMap loadedLevel);

    public Object[] objects;

    private TileMap Level;
    private Player player;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        Level = Map.Instantiate<TileMap>();
        Level.ZIndex = -1;

        AddChild(Level);
        EmitSignal(SignalName.OnLoadLevel, Level);

        // Later change this to be called whenever we move a light or something can change it maybe
        EmitSignal(SignalName.OnLightsChanged, Level);

        player = GetNode<Player>("Player");

        //var rock = Rock.Instantiate<Rock>();
        //rock.Position = Level.MapToLocal(new Vector2I(2, 2));

        // AddChild(rock);
    }

    public override void _Input(InputEvent @event) {

        if (!player.moving) {
            if (@event.IsActionPressed("ui_left")) {
                PlayerMovement("ui_left", TileSet.CellNeighbor.LeftSide);
            } else if (@event.IsActionPressed("ui_right")) {
                PlayerMovement("ui_right", TileSet.CellNeighbor.RightSide);
            } else if (@event.IsActionPressed("ui_up")) {
                PlayerMovement("ui_up", TileSet.CellNeighbor.TopSide);
            } else if (@event.IsActionPressed("ui_down")) {
                PlayerMovement("ui_down", TileSet.CellNeighbor.BottomSide);
            }
        }

        // We can move this later if we want just adding for testing sake
        if (@event.IsActionPressed("place_light")) 
        {
            CreateLightSource();
        }
    }
    private void PlayerMovement(string direction, TileSet.CellNeighbor neighbour) {
        var playerCurrentXY = Level.LocalToMap(player.Position);
        var nextCell = Level.GetNeighborCell(playerCurrentXY, neighbour);
        var nextCellCoordinates = Level.MapToLocal(nextCell);
        var nextCellLayerZero = Level.GetCellTileData(0, nextCell);
        // For now adding shadows check here as well
        LightingManager LightingManager = GetNode<LightingManager>("LightingManager");
        TileMap ShadowTileMap = LightingManager.GetNode<TileMap>("ShadowTileMap");

        if ((Level.GetCellTileData(1, nextCell) == null) && (ShadowTileMap.GetCellTileData(0, nextCell) == null)) {
			player.MoveToPosition(nextCellCoordinates, direction);
			if (nextCellLayerZero.GetCustomData("Hole").AsBool()) {
				GD.Print("Is Hole");
			}
		} else {
			player.Face(direction);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

    public void OnToggleLightingPressed() 
    {
        LightingManager lightingManager = GetNode<LightingManager>("LightingManager");
        // Weirdly lightManager.Visible didn't like it when I tried to use it in a ternary operator.
        bool isVisible = lightingManager.Visible;
        if (isVisible) 
        {
            lightingManager.Hide();
            lightingManager.ClearTiles();
        }
        else 
        { 
            lightingManager.Show();
            // Repopulate tiles
            EmitSignal(SignalName.OnLightsChanged, Level);
        }
    }


    private void CreateLightSource() 
    {
        // TODO place in front of the user instead of on the player.
        // perhaps storing the direction the player is facing could be good
        var playerPos = Level.LocalToMap(player.Position);
        // Layer 2 is lights
        Level.SetCell(2, new Vector2I(playerPos.X, playerPos.Y), 0, new Vector2I(9, 5));
        EmitSignal(SignalName.OnLightsChanged, Level);

    }
}

