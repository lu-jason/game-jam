using Godot;
using Godot.Collections;
using System;

public partial class LightingManager : Node2D
{
    // If we want different lights
    // figure out dafuq these do.
    enum LightType 
    {
        Point = 1,
        Directional = 2,
        Left = 3,
    }

    private int[,] lightLevels;
    Dictionary<Vector2I, LightType> lights = new Dictionary<Vector2I,LightType>();
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
        // NOTE:
        // This code isn't super efficient if we find we need better performance I can added some caching behaviour
        // So the tiles only updated when needed.

        // TODO: Do this better cause it sucks
        for (int x = 0; x < lightLevels.GetLength(0); x++) {
            for (int y = 0; y < lightLevels.GetLength(1); y++) {
                lightLevels[x, y] = 2;
            }
        }

        foreach (var light in lights) 
        {
            AdjustLightMapAt(light.Key);
        }

        // Update the tileMap 
        TileMap LightingTileMap = GetNode<TileMap>("LightingTileMap");
        // Update light tilemap based on light levels
        for (int x = 0; x < lightLevels.GetLength(0); x++) 
        {
            for (int y = 0; y < lightLevels.GetLength(1); y++) 
            {
                LightingTileMap.SetCell(0, new Vector2I(x, y), 0, new Vector2I(lightLevels[x,y], 0));
            }
        }
    }

    public void UpdateBaseLevelLighting(TileMap loadedLevel) 
    {
        TileMap LightingTileMap = GetNode<TileMap>("LightingTileMap");
        if (LightingTileMap != null) {
            LightingTileMap.Position = loadedLevel.Position;
        }
        // Initiate lightLevels to match loadedLevel size
        Vector2I tileBounds = loadedLevel.GetUsedRect().End;
        lightLevels = new int[tileBounds.X, tileBounds.Y];
    }

    // NOTE update lights based on the whole Tile map kinda sucks could be way more optimal
    public void OnLightsChanged(TileMap loadedLevel) 
    {
        GD.Print("lights changed!");
        // Update the list of lights based on layer 2 for now
        lights.Clear();
        Vector2I tileBounds = loadedLevel.GetUsedRect().End;
        for (int x = 0; x < tileBounds.X; x++) {
            for (int y = 0; y < tileBounds.Y; y++) {
                var cellData = loadedLevel.GetCellTileData(2, new Vector2I(x, y));
                if (cellData != null) 
                {
                    // Default to Point light for now
                    lights.Add(new Vector2I(x, y), LightType.Point);
                    GD.Print("Found light at Poistion:", x, y);
                    // TODO read custom data for light type here
                    //var Type = cellData.GetCustomData("LightType");
                    //Type.AsInt16();  
                }
            }
        }
    }

    private void AdjustLightMapAt(Vector2I lightPosition) 
    {
        lightLevels[lightPosition.X, lightPosition.Y] = 4;

        // Shitty hardcoded testing code
        if (lightPosition.X - 1 > 0) 
        {
            lightLevels[lightPosition.X - 1, lightPosition.Y] = 3;
        }
        if (lightPosition.Y - 1 > 0) 
            {
            lightLevels[lightPosition.X, lightPosition.Y-1] = 3;
        }
        if (lightPosition.X + 1 < lightLevels.GetLength(0)) {
            lightLevels[lightPosition.X + 1, lightPosition.Y] = 3;
        }
        if (lightPosition.Y + 1 < lightLevels.GetLength(1)) {
            lightLevels[lightPosition.X , lightPosition.Y+1] = 3;
        }

    }
}
