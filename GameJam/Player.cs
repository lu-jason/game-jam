using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public partial class Player : CharacterClass {
	// Called when the node enters the scene tree for the first time.

    public override void Move(string direction, TileSet.CellNeighbor neighbour){
		var playerCurrentXY = currentLevel.LocalToMap(Position);
        var nextCell = currentLevel.GetNeighborCell(playerCurrentXY, neighbour);
        var nextCellCoordinates = currentLevel.MapToLocal(nextCell);
        var nextCellLayerZero = currentLevel.GetCellTileData(0, nextCell);

        // For now adding shadows check here as well
        LightingManager LightingManager = GetNode<LightingManager>("/root/Main/LevelViewer/LightingManager");
        TileMap ShadowTileMap = LightingManager.GetNode<TileMap>("ShadowTileMap");

        if ((currentLevel.GetCellTileData(1, nextCell) == null) && (ShadowTileMap.GetCellTileData(0, nextCell) == null)) {
			MoveToPosition(nextCellCoordinates, direction);
			if (nextCellLayerZero.GetCustomData("Hole").AsBool()) {
				GD.Print("Is Hole");
			}
		} else {
			Face(direction);
		}
	}
}
