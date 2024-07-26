using Godot;
using System;

public partial class Player : GameObject {


    public override bool CanMove(Vector2I coords, string direction) {
        // Later we could hold a reference to these so we don't need to look them up with this "hardcodey" way

        LevelViewer levelViewer = GetNode<LevelViewer>("/root/Main/LevelViewer");
        TileData floorData = levelViewer.GetTileData("floors", coords);
        TileData wallData = levelViewer.GetTileData("walls", coords);

        // For now adding shadows check here as well
        LightingManager lightingManager = GetNode<LightingManager>("/root/Main/LevelViewer/LightingManager");
        TileMap shadowTileMap = lightingManager.GetNode<TileMap>("ShadowTileMap");
        TileData shadowData = shadowTileMap.GetCellTileData(0, coords);

        if (levelViewer.IsObject(coords)) {
            GD.Print("Object is here");
            var moved = levelViewer.MoveObject(coords, direction);
            if (moved) {
                GD.Print("Object moved");
            } else {
                GD.Print("Object not moved");
                return false;
            }
        }

        // If the walls and shadows are empty we can move there
        if ((wallData == null) && (shadowData == null)) {
            return true;
            // TODO - readd hole logic
        }

        // Do whatever logic here for now return true;
        return false;
    }

}
