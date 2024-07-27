using Godot;
using System;
using System.ComponentModel;

public partial class Player : GameObject
{
    [Export]
    public int Strength;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override bool CanMove(Vector2I coords, string direction)
    {
        // Later we could hold a reference to these so we don't need to look them up with this "hardcodey" way

        LevelViewer levelViewer = GetNode<LevelViewer>("/root/Main/LevelViewer");
        TileData floorData = levelViewer.GetTileData("floors", coords);
        TileData wallData = levelViewer.GetTileData("walls", coords);

        // For now adding shadows check here as well
        LightingManager lightingManager = GetNode<LightingManager>("/root/Main/LevelViewer/LightingManager");
        TileMap shadowTileMap = lightingManager.GetNode<TileMap>("ShadowTileMap");
        TileData shadowData = shadowTileMap.GetCellTileData(0, coords);

        if (levelViewer.IsObject(coords))
        {
            var obj = levelViewer.GetObject(coords);
            GD.Print("Object is here");
            if (obj != null && Strength >= obj.ObjectStrength)
            {
                var moved = levelViewer.MoveObject(coords, direction);
                if (moved)
                {
                    GD.Print("Object moved");
                    return true;
                }
                else
                {
                    GD.Print("Object not moved");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        // If the walls and shadows are empty we can move there
        if ((wallData == null) && (shadowData == null))
        {
            return true;
            // TODO - readd hole logic
        }

        // Do whatever logic here for now return true;
        return false;
    }

}