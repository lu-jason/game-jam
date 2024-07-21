using Godot;
using Godot.Collections;
using System;
using System.Linq;
using System.Security.Cryptography;

public partial class LevelViewer : Node2D
{
    [Export]
    public PackedScene Map { get; set; }

    public TileMap loadedLevel;

    //private TileMap LightingTileMap;


    Dictionary<Vector2I, int> lightLevels;

    Color BlackColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);

    // TODO load the shaders in code rather than setting them via export
    //[Export]
    //public ShaderMaterial lightShader;

    //[Export]
    //public TileSet lightTileSet;

    //private ImageTexture lightMask;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        loadedLevel = Map.Instantiate<TileMap>();
        loadedLevel.ZIndex = -1;

        GD.Print("Size of level: ", loadedLevel.GetUsedRect().End);
        GD.Print("Tile Size: ", loadedLevel.TileSet.TileSize);
        //lightShader.SetShaderParameter("TileSize", tileSize);
        //lightShader.SetShaderParameter("TileBounds", tileBounds);

        TileMap LightingTileMap = GetNode<TileMap>("LightingTileMap");

        //LightingTileMap = loadedLevel;
        //LightingTileMap.Clear();
        TileSet lighting = LightingTileMap.TileSet;
        LightingTileMap.Position = loadedLevel.Position;
        LightingTileMap.SetCell(0, new Vector2I(1, 1), 0, new Vector2I(0, 0));
        LightingTileMap.SetCell(0, new Vector2I(1, 2), 0, new Vector2I(0, 0));
        LightingTileMap.SetCell(0, new Vector2I(1, 3), 0, new Vector2I(0, 0));
        LightingTileMap.SetCell(0, new Vector2I(1, 4), 1, new Vector2I(1, 0));
        LightingTileMap.SetCell(0, new Vector2I(1, 5), 1, new Vector2I(2, 0));

        LightingTileMap.SetCell(0, new Vector2I(2, 1), 1, new Vector2I(0, 0));
        LightingTileMap.SetCell(0, new Vector2I(2, 2), 1, new Vector2I(0, 0));
        LightingTileMap.SetCell(0, new Vector2I(3, 1), 1, new Vector2I(0, 0));


        LightingTileMap.SetCell(0, new Vector2I(4, 1), 1, new Vector2I(1, 0));
        LightingTileMap.SetCell(0, new Vector2I(3, 2), 1, new Vector2I(1, 0));

        LightingTileMap.SetCell(0, new Vector2I(5, 1), 1, new Vector2I(2, 0));

        LightingTileMap.SetCell(0, new Vector2I(4, 2), 1, new Vector2I(2, 0));
        LightingTileMap.SetCell(0, new Vector2I(3, 1), 1, new Vector2I(2, 0));


        GD.Print("Size of level: ", LightingTileMap.GetUsedRect().End);
        GD.Print("Tile Size: ", LightingTileMap.TileSet.TileSize);

        // Test 

        //Vector2I pixelSize = tileSize * tileBounds; 

        // 4 for r g b a
        //int NumOfBytes= tileBounds.X * tileBounds.Y*4;
        //GD.Print("Number of bytes: ",NumOfBytes);

        //byte[] imageData = new byte[NumOfBytes];
        //for (int i = 0; i < tileBounds.X*tileBounds.Y*4; i=i+4)
        //{
        //    imageData[i] = 0;
        //    imageData[i + 1] = 0;
        //    imageData[i + 2] = 255;
        //    imageData[i + 3] = 255;
        //}
        //var DynamicImage = Image.Create(tileBounds.X, tileBounds.Y, false, Image.Format.Rgba8);
        ////DynamicImage.Resize(pixelSize.X, pixelSize.Y);
        //DynamicImage.Fill(BlackColor);
        //DynamicImage.SetPixel(0, 0, BlackColor);

        //lightMask = ImageTexture.CreateFromImage(DynamicImage);
        //lightMask.

        //lightShader.SetShaderParameter("LightMask", lightMask);
        //loadedLevel.Material = lightShader;
        // Setup lightLevels dictionary (this feels kinda yikes but whatever), maybe I should've 
        //for (int x = 0; x < tileSize.X; x++)
        //{
        //    for (int y = 0; y < tileSize.Y; y++)
        //    {
        //        lightLevels.Add(new Vector2I(x, y), 0);
        //    }
        //}
        AddChild(loadedLevel);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }

    public void DebugPollTileMapData()
    {
        //loadedLevel.SetCell()
        var tileData = loadedLevel.GetCellTileData(0, new Vector2I(0, 0));
        //tileData.SetCustomData("lightLevel", 1);
        var lightLevel = tileData.GetCustomData("lightLevel");
        GD.Print("Node at 0 0 has light level: ", lightLevel.AsInt16());


        var tileData2 = loadedLevel.GetCellTileData(0, new Vector2I(2, 2));
        lightLevel = tileData2.GetCustomData("lightLevel");
        GD.Print("Node at 2 2 has light level: ",lightLevel.AsInt16());
        lightLevel.AsBool();
    }
}
