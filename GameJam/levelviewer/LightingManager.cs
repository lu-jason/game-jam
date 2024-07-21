using Godot;
using System;

public partial class LightingManager : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        TileMap LightingTileMap = GetNode<TileMap>("LightingTileMap");

        TileSet lighting = LightingTileMap.TileSet;
        //LightingTileMap.Position = loadedLevel.Position;
        //LightingTileMap.SetCell(0, new Vector2I(1, 1), 0, new Vector2I(0, 0));
        //LightingTileMap.SetCell(0, new Vector2I(1, 2), 0, new Vector2I(0, 0));
        //LightingTileMap.SetCell(0, new Vector2I(1, 3), 0, new Vector2I(0, 0));
        //LightingTileMap.SetCell(0, new Vector2I(1, 4), 1, new Vector2I(1, 0));
        //LightingTileMap.SetCell(0, new Vector2I(1, 5), 1, new Vector2I(2, 0));

        //LightingTileMap.SetCell(0, new Vector2I(2, 1), 1, new Vector2I(0, 0));
        //LightingTileMap.SetCell(0, new Vector2I(2, 2), 1, new Vector2I(0, 0));
        //LightingTileMap.SetCell(0, new Vector2I(3, 1), 1, new Vector2I(0, 0));


        //LightingTileMap.SetCell(0, new Vector2I(4, 1), 1, new Vector2I(1, 0));
        //LightingTileMap.SetCell(0, new Vector2I(3, 2), 1, new Vector2I(1, 0));

        //LightingTileMap.SetCell(0, new Vector2I(5, 1), 1, new Vector2I(2, 0));

        //LightingTileMap.SetCell(0, new Vector2I(4, 2), 1, new Vector2I(2, 0));
        //LightingTileMap.SetCell(0, new Vector2I(3, 1), 1, new Vector2I(2, 0));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

    public void SetLightingTileMapPosition(Vector2 position)
    {
        TileMap LightingTileMap = GetNode<TileMap>("LightingTileMap");
        if (LightingTileMap != null)
        {
            LightingTileMap.Position = position;
        }
    }
}
