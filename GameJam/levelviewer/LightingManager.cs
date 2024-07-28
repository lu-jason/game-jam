using System;
using System.Reflection.Emit;
using Godot;
using Godot.Collections;

enum Layers : int
{
    FLOORS,
    WALLS,
    LIGHTS,
    OBJECTS
}

public partial class LightingManager : Node2D
{
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
    Dictionary<Vector2I, int> lights = new Dictionary<Vector2I, int>();

    // Pretty sure C# passes by reference
    TileMap levelRef;

    private TileMap LightingTileMap;

    private TileMap ShadowTileMap;

    public override void _Ready()
    {
        LightingTileMap = GetNode<TileMap>("LightingTileMap");
        ShadowTileMap = GetNode<TileMap>("ShadowTileMap");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void UpdateBaseLevelLighting(TileMap loadedLevel)
    {
        TileMap LightingTileMap = GetNode<TileMap>("LightingTileMap");
        if (LightingTileMap != null)
        {
            LightingTileMap.Position = loadedLevel.Position;
        }

        // Initiate lightLevels to match loadedLevel size
        Vector2I tileBounds = loadedLevel.GetUsedRect().End;
        lightLevels = new int[tileBounds.X, tileBounds.Y];
    }

    public void ClearTiles()
    {
        LightingTileMap.Clear();
        ShadowTileMap.Clear();
    }

    // NOTE update lights based on the whole Tile map kinda sucks could be way more optimal
    public void OnLightsChanged(TileMap loadedLevel)
    {
        levelRef = loadedLevel;
        // Update the list of lights based on layer 2 for now
        lights.Clear();
        Vector2I tileBounds = loadedLevel.GetUsedRect().End;
        for (int x = 0; x < tileBounds.X; x++)
        {
            for (int y = 0; y < tileBounds.Y; y++)
            {
                Vector2I position = new(x, y);
                var cellData =
                    loadedLevel
                        .GetCellTileData((int) Layers.LIGHTS,
                        position);
                if (cellData == null) continue;

                // WARNING DO NOT SET THIS TOO HIGH
                // My point source algorithm is slow as kek
                int lightIntensity =
                    cellData.GetCustomData("LightIntensity").AsInt32();

                if (lightIntensity <= 0) continue;

                // Default to Point light for now
                lights.Add(new(x, y), lightIntensity);
            }
        }

        // NOTE:
        // This code isn't super efficient if we find we need better performance I can added some caching behaviour
        // So the tiles only updated when needed.
        // TODO: Do this better cause it sucks
        for (int x = 0; x < lightLevels.GetLength(0); x++)
        {
            for (int y = 0; y < lightLevels.GetLength(1); y++)
            {
                lightLevels[x, y] = 0;
            }
        }

        foreach (var light in lights)
        {
            ApplyPointLight(light.Key, light.Value);
        }

        // Update the tileMap
        ClearTiles();

        // Update light tilemap based on light levels
        for (int x = 0; x < lightLevels.GetLength(0); x++)
        {
            for (int y = 0; y < lightLevels.GetLength(1); y++)
            {
                if (lightLevels[x, y] != 0)
                {
                    int lightIndex = lightLevels[x, y];
                    if (lightIndex > MAX_LIGHT_LEVEL)
                        lightIndex = MAX_LIGHT_LEVEL;
                    LightingTileMap
                        .SetCell(0,
                        new Vector2I(x, y),
                        0,
                        new Vector2I(lightIndex, 0));
                }
                else
                {
                    // This will have to be in sync with the atlas entrys to the shadow tile set
                    uint randVal = GD.Randi() % 9;
                    ShadowTileMap
                        .SetCell(0,
                        new Vector2I(x, y),
                        0,
                        new Vector2I((int) randVal, 0));
                }
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
    private void ApplyPointLight(Vector2I lightPosition, int LightIntensity)
    {
        // add the light level to the map.
        AddLightLevel(lightPosition.X, lightPosition.Y, LightIntensity);

        // return if the light level is 1 or less.
        if (LightIntensity <= 1)
        {
            return;
        }

        // reset hasLightBeenAdjusted for this new point light should default to false
        int arraySize = LightIntensity * 2 - 1;
        lightAdjusted = new int[arraySize, arraySize];

        // Set as midPoint
        Vector2I AdjustedMapLocation =
            new Vector2I(LightIntensity - 1, LightIntensity - 1);

        // Always apply strongest light light to tile the light is on
        lightAdjusted[AdjustedMapLocation.X, AdjustedMapLocation.Y] = 4;

        // Maybe doing the first one is fine
        // Depends if we want to be able to have point lines on a wall
        //if (!IsWall(lightPosition.X, lightPosition.Y))
        //{
        LightUpNeighbours(lightPosition,
        AdjustedMapLocation,
        LightIntensity - 1);
        //}
    }

    private void LightUpNeighbours(
        Vector2I lightPos,
        Vector2I adjustedPos,
        int lightValue
    )
    {
        // This code could be more efficient by adding some early outs, but can't be bothered right now.
        // Setup lambda function to reduce duplicate code
        // Also why the hell can't I set the return type to void
        Func<Vector2I, Vector2I, bool> LightUpPosition =
            (Vector2I tilemapPos, Vector2I adjustedPos) =>
            {
                if (lightValue > lightAdjusted[adjustedPos.X, adjustedPos.Y])
                {
                    // Figure out how much higher we need to set the light if it has already been set for that position
                    int adjustLevel =
                        lightValue -
                        lightAdjusted[adjustedPos.X, adjustedPos.Y];

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

        LightUpPosition (UPos, UAdj); // UP
        LightUpPosition (RPos, RAdj); // RIGHT
        LightUpPosition (DPos, DAdj); // DOWN
        LightUpPosition (LPos, LAdj); // LEFT

        Func<Vector2I, Vector2I, int, bool> LightUpNeighbour =
            (Vector2I UPos, Vector2I UAdj, int lightLevel) =>
            {
                if (!IsWall(UPos.X, UPos.Y))
                {
                    LightUpNeighbours(UPos, UAdj, lightLevel - 1);
                    return true;
                }
                return false;
            };

        if (lightValue > 1)
        {
            // TODO Add wall
            LightUpNeighbour (UPos, UAdj, lightValue);
            LightUpNeighbour (RPos, RAdj, lightValue);
            LightUpNeighbour (DPos, DAdj, lightValue);
            LightUpNeighbour (LPos, LAdj, lightValue);
        }
    }

    // return true if valid tile.
    private bool AddLightLevel(int x, int y, int value)
    {
        // exit out early if out of bounds
        if (
            x < 0 ||
            y < 0 ||
            x >= lightLevels.GetLength(0) ||
            y >= lightLevels.GetLength(1)
        )
        {
            return false;
        }

        int newLight = lightLevels[x, y] + value;

        // Clamp value
        if (newLight < 0)
        {
            newLight = 0;
        }

        lightLevels[x, y] = newLight;
        return true;
    }

    // This should probably live somewhere more global
    // Tests if cell at x,y is a wall
    private bool IsWall(int x, int y)
    {
        return (levelRef != null) && (levelRef.GetCellTileData(1, new Vector2I(x, y)) != null);
    }
}
