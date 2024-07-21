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

	public Object[] objects;

	private TileMap Level;
	private Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Level = Map.Instantiate<TileMap>();
		Level.ZIndex = -1;

		AddChild(Level);
        EmitSignal(SignalName.OnLoadLevel,Level);

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
	}
	private void PlayerMovement(string direction, TileSet.CellNeighbor neighbour) {
		var playerCurrentXY = Level.LocalToMap(player.Position);
		var nextCell = Level.GetNeighborCell(playerCurrentXY, neighbour);
		var nextCellCoordinates = Level.MapToLocal(nextCell);
		var nextCellLayerZero = Level.GetCellTileData(0, nextCell);	

		if (Level.GetCellTileData(1, nextCell) == null) {
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
}

