using Godot;
using System;

public partial class Rock : GameObject {

	public override bool CanMove(Vector2I coords, string direction) {
		// Later we could hold a reference to these so we don't need to look them up with this "hardcodey" way

		TileData floorData = levelViewer.GetTileData("floors", coords);
		TileData wallData = levelViewer.GetTileData("walls", coords);

		// For now adding shadows check here as well
		TileMap shadowTileMap = lightingManager.GetNode<TileMap>("ShadowTileMap");
		TileData shadowData = shadowTileMap.GetCellTileData(0, coords);

		// if (levelViewer.IsObject(coords)) {
		// 	GD.Print("Object is here");
		// 	var moved = levelViewer.MoveObject(coords, direction);
		// 	if (moved) {
		// 		GD.Print("Object moved");
		// 	} else {
		// 		GD.Print("Object not moved");
		// 	}
		// }

		// If the walls and shadows are empty we can move there
		if ((wallData == null) && (shadowData == null) && !levelViewer.IsObject(coords)) {
			return true;
			// TODO - readd hole logic
		}

		// Do whatever logic here for now return true;
		return false;
	}
}
