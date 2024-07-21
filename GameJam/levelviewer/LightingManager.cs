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

    const int MAX_LIGHT_LEVEL = 4; // We can adjust as needed. 

    private int[,] lightLevels;
    // This array stores the running light adjustments for a point light
    // see comment below about desired adjustment (assuming no walls)
    private int[,] lightAdjusted;

    //private int[,] maxLightFromPointSource = new int[9, 9]
    //{
    //    { 0,0,0,0,1,0,0,0,0 },
    //    { 0,0,0,1,2,1,0,0,0 },
    //    { 0,0,1,2,3,2,1,0,0 },
    //    { 0,1,2,3,4,3,2,1,0 },
    //    { 1,2,3,4,4,4,3,2,1 },
    //    { 0,1,2,3,4,3,2,1,0 },
    //    { 0,0,1,2,3,2,1,0,0 },
    //    { 0,0,0,1,2,1,0,0,0 },
    //    { 0,0,0,0,1,0,0,0,0 },
    //};

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


        // NOTE:
        // This code isn't super efficient if we find we need better performance I can added some caching behaviour
        // So the tiles only updated when needed.

        // TODO: Do this better cause it sucks
        for (int x = 0; x < lightLevels.GetLength(0); x++) {
            for (int y = 0; y < lightLevels.GetLength(1); y++) {
                lightLevels[x, y] = 0;
            }
        }

        foreach (var light in lights) {
            ApplyPointLight(light.Key);
        }

        // Update the tileMap 
        TileMap LightingTileMap = GetNode<TileMap>("LightingTileMap");
        // Update light tilemap based on light levels
        for (int x = 0; x < lightLevels.GetLength(0); x++) {
            for (int y = 0; y < lightLevels.GetLength(1); y++) {
                LightingTileMap.SetCell(0, new Vector2I(x, y), 0, new Vector2I(lightLevels[x, y], 0));
            }
        }

    }

    /* Still figuring this shit out
     * 
     * Max pattern for current setting of 5 shades.
     * 
     * 0 0 0 0 1 0 0 0 0
     * 0 0 0 1 2 1 0 0 0
     * 0 0 1 2 3 2 1 0 0
     * 0 1 2 3 4 3 2 1 0
     * 1 2 3 4 4 4 3 2 1
     * 0 1 2 3 4 3 2 1 0
     * 0 0 1 2 3 2 1 0 0
     * 0 0 0 1 2 1 0 0 0
     * 0 0 0 0 1 0 0 0 0
     * 
     * 
     * 
     */

    private void ApplyPointLight(Vector2I lightPosition) 
    {
        // reset hasLightBeenAdjusted for this new point light should default to false
        lightAdjusted = new int[9, 9];
        Vector2I AdjustedMapLocation = new Vector2I(4, 4);

        // Always apply strongest light light to tile the light is on
        AddLightLevel(lightPosition.X, lightPosition.Y,MAX_LIGHT_LEVEL);
        lightAdjusted[AdjustedMapLocation.X, AdjustedMapLocation.Y] = 4;

        // TODO - Don't do this if we are on a wall

        LightUpNeighbours(lightPosition, AdjustedMapLocation, MAX_LIGHT_LEVEL);

    }

    private void LightUpNeighbours(Vector2I lightPos, Vector2I adjustedPos, int lightValue) 
    {
        // This code could be more efficient by adding some early outs, but can't be bothered right now.

        // Setup lambda function to reduce duplicate code
        // Also why the hell can't I set the return type to void
        Func<Vector2I, Vector2I, bool> LightUpPosition = (Vector2I tilemapPos, Vector2I adjustedPos) => 
        { 
            if (lightValue > lightAdjusted[adjustedPos.X, adjustedPos.Y]) 
            {
                // Figure out how much higher we need to set the light if it has already been set for that position
                int adjustLevel = lightValue - lightAdjusted[adjustedPos.X, adjustedPos.Y];

                AddLightLevel(tilemapPos.X, tilemapPos.Y, adjustLevel);
                lightAdjusted[adjustedPos.X, adjustedPos.Y] = lightValue;
                return true;
            }
            return false;
        };

        // Up, Right, Down, Left
        Vector2I UPos = new Vector2I(lightPos.X, lightPos.Y - 1);
        Vector2I RPos = new Vector2I(lightPos.X + 1, lightPos.Y);
        Vector2I DPos = new Vector2I(lightPos.X, lightPos.Y + 1);
        Vector2I LPos = new Vector2I(lightPos.X - 1, lightPos.Y);

        Vector2I UAdj = new Vector2I(adjustedPos.X, adjustedPos.Y - 1);
        Vector2I RAdj = new Vector2I(adjustedPos.X + 1, adjustedPos.Y);
        Vector2I DAdj = new Vector2I(adjustedPos.X, adjustedPos.Y + 1);
        Vector2I LAdj = new Vector2I(adjustedPos.X - 1, adjustedPos.Y);

        LightUpPosition(UPos, UAdj);    // UP
        LightUpPosition(RPos, RAdj);    // RIGHT
        LightUpPosition(DPos, DAdj);      // DOWN
        LightUpPosition(LPos, LAdj);    // LEFT

        if (lightValue > 1) {
            // TODO Add wall check
            LightUpNeighbours(UPos, UAdj, lightValue - 1);
            LightUpNeighbours(RPos, RAdj, lightValue - 1);
            LightUpNeighbours(DPos, DAdj, lightValue - 1);
            LightUpNeighbours(LPos, LAdj, lightValue - 1);
        }
    }

    // return true if valid tile.
    private bool AddLightLevel(int x, int y, int value) 
    {
        // exit out early if out of bounds
        if (x<0 || y<0 || x>=lightLevels.GetLength(0) || y>=lightLevels.GetLength(1)) 
        {
            return false;
        }

        int newLight = lightLevels[x, y] + value;

        // Clamp value
        if (newLight < 0) 
        {
            newLight = 0;
        }
        else if (newLight > MAX_LIGHT_LEVEL) 
        {
            newLight = MAX_LIGHT_LEVEL;
        }

        lightLevels[x, y] = newLight;
        return true;
    }
}
