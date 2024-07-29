using System;
using System.Numerics;
using System.Xml.Linq;
using Godot;
using Godot.Collections;

public partial class LevelViewer : Node2D
{
    [Export]
    public PackedScene Map { get; set; }

    [Export]
    public PackedScene RockScene { get; set; }

	[Export]
    public PackedScene DoorScene { get; set; }

    [Export]
    public PackedScene ToggleLightScene { get; set; }

    [Export]
    public PackedScene PressureSwitchScene { get; set; }

    [Signal]
    public delegate void OnLoadLevelEventHandler(TileMap LoadedLevel);

    [Signal]
    public delegate void OnLightsChangedEventHandler(TileMap loadedLevel);

    [Signal]
    public delegate void OnObjectChangedEventHandler(GameObject go, Vector2I position);

    private CanvasLayer gameOverScene;

    private TileMap Level;
    private int currentLevelID;
    const int cMAX_LEVEL_NUMBER = 3;

    private LightingManager lightingManager;

    private TileMap shadowTileMap;

    private PlayerManager playerManager;

    private Vector2I tileBounds;

    private GameObject[,] gameObjects;

    private Dictionary<string, int> LayerMap = new Dictionary<string, int>();

    private Vector2I witchSpawn = Vector2I.Zero;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        currentLevelID = 0;
        LoadCurrentLevel();
        lightingManager = GetNode<LightingManager>("LightingManager");
        shadowTileMap = lightingManager.GetNode<TileMap>("ShadowTileMap");
        gameOverScene = GetNode<CanvasLayer>("GameOver");
        gameOverScene.Hide();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        if (CheckIfGameOver()) {
            DisplayGameOver();
        }
    }

    public void LoadCurrentLevel() {
        LoadLevel(currentLevelID);
    }

    public void LoadLevel(int level)
    {
        GD.Print("Loading Level: ", level);
        // Clean up old first if it exists
        if (Level != null)
        {
            Level.QueueFree();
            LayerMap.Clear();
            Level = null;
        }

        string levelPath = "res://levels/Level" + level.ToString() + ".tscn";
        GD.Print("levelPath: ", levelPath);
        PackedScene levelScene = GD.Load<PackedScene>(levelPath);
        Level = levelScene.Instantiate<TileMap>();
        Level.ZIndex = -1;

        AddChild(Level);

        SetupLevelInfo();

        GetNode<InteractionManager>("InteractionManager").Setup(Level);

        CreateObjects();

        playerManager = GetNode<PlayerManager>("PlayerManager");
        playerManager.MovePlayerTo(witchSpawn);

        // Emit Signals
        EmitSignal(SignalName.OnLoadLevel, Level);
        EmitSignal(SignalName.OnLightsChanged, Level);
    }

    private void SetupLevelInfo()
    {
        for (int i = 0; i < Level.GetLayersCount(); i++)
        {
            LayerMap.Add(Level.GetLayerName(i), i);
        }

        // Initiate lightLevels to match loadedLevel size
        tileBounds = Level.GetUsedRect().End;

        gameObjects = new GameObject[tileBounds.X, tileBounds.Y];
    }

    public static (string, string) GetItemData(string itemString) {
        if (itemString.Length <= 0) return ("", "");
        string[] parts = itemString.Split(":");
        return (parts[0], (parts.Length >= 2) ? parts[1] : "");
    }

    private void CreateObjects()
    {
        // Iterate through all the objects in the "objects" layer in the tilemap
        // and create the corresponding object and delete the cell from the map
        for (int x = 0; x < tileBounds.X; x++)
        {
            for (int y = 0; y < tileBounds.Y; y++)
            {
                var tileData = GetTileData("objects", new Vector2I(x, y));

                // skip missing tiledata.
                if (tileData == null) continue;

                var position = new Vector2I(x, y);

                string itemString = tileData.GetCustomData("ItemId").AsString();
                (string itemId, string itemData) = GetItemData(itemString);
                switch (itemId)
                {
                    case "Rock":
                        GD.Print("Found rock at ", x, y);
                        var rock = RockScene.Instantiate<Rock>();
                        AddChild (rock);
                        rock.OverrideTileCoords(position);
                        AddGameObject (rock, position);
                        break;
					case "Door":
                        GD.Print("Found door at ", x, y);
                        var door = DoorScene.Instantiate<Door>();
                        AddChild(door);
                        door.OverrideTileCoords(position);
                        AddGameObject (door, position);
                        break;
                    case "Light":
                        GD.Print("Found Light at ", x, y);
                        break;
                    case "ToggleLight":
                        GD.Print("Found ToggleLight at ", x, y);
                        ToggleLight obj =
                            ToggleLightScene.Instantiate<ToggleLight>();
                        AddChild (obj);
                        obj.SetItemData(itemData);
                        obj.OverrideTileCoords(position);
                        AddGameObject (obj, position);
                        break;
                    case "Switch":
                        GD.Print("Found PressureSwitch at ", x, y);
                        PressureSwitch pSwitch = PressureSwitchScene.Instantiate<PressureSwitch>();
                        AddChild(pSwitch);
                        pSwitch.SetItemData(itemData);
                        pSwitch.OverrideTileCoords(position);
                        AddGameObject (pSwitch, position);
                        break;
                    case "WitchSpawn":
                        GD.Print("Found WitchSpawn at ", x, y);
                        witchSpawn.X = x;
                        witchSpawn.Y = y;
                        break;
                    default:
                        continue;
                }

                Level.SetCell(GetLayerNumber("objects"), position, -1);
            }
        }
    }

    private bool AddGameObject(GameObject obj, Vector2I position) {
		// Using a try-finally to execute code block after a return
		try {
			if (gameObjects[position.X, position.Y] == null)
			{
				gameObjects[position.X, position.Y] = obj;
				return true;
			}
			return false;
		} finally {
            EmitSignal(SignalName.OnObjectChanged, gameObjects[position.X, position.Y], position);
		}
    }

    // IsObject returns whether an object is at the provided position.
    public bool IsObject(Vector2I position)
    {
        if (gameObjects[position.X, position.Y] != null)
        {
            return true;
        }
        return false;
    }

    // ObjectAtPosition returns an object that is at the provided position.
    public GameObject ObjectAtPosition(Vector2I position)
    {
        return IsObject(position) ? gameObjects[position.X, position.Y] : null;
    }

    public bool MoveObject(Vector2I position, string direction) {
		// Using a try-finally to execute code after a return
		try {
			if (IsObject(position)) {
				var gameObject = gameObjects[position.X, position.Y];
				switch (direction) {
				case "left":
					if (gameObject.MoveLeft())
					{
						gameObjects[position.X - 1, position.Y] = gameObject;
						gameObjects[position.X, position.Y] = null;
						return true;
					}
					break;
				case "right":
					if (gameObject.MoveRight())
					{
						gameObjects[position.X + 1, position.Y] = gameObject;
						gameObjects[position.X, position.Y] = null;
						return true;
					}
					break;
				case "up":
					if (gameObject.MoveUp())
					{
						gameObjects[position.X, position.Y - 1] = gameObject;
						gameObjects[position.X, position.Y] = null;
						return true;
					}
					break;
				case "down":
					if (gameObject.MoveDown())
					{
						gameObjects[position.X, position.Y + 1] = gameObject;
						gameObjects[position.X, position.Y] = null;
						return true;
					}
					break;
				}
			}

			return false;

		} finally {
            EmitSignal(SignalName.OnObjectChanged, gameObjects[position.X, position.Y], position);
		}
    }

    public int GetLayerNumber(string layerName)
    {
        return LayerMap[layerName];
    }

    public override void _Input(InputEvent @event)
    {
        // We can move this later if we want just adding for testing sake`
        if (@event.IsActionPressed("place_light"))
        {
            CreateLightSource();
        }
    }

    public TileData GetTileData(string layerName, Vector2I coords)
    {
        int layer = GetLayerNumber(layerName);
        return Level.GetCellTileData(layer, coords);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="layerName"></param>
    /// <param name="coords"></param>
    /// <param name="data"></param>
    ///
    public void SetTileData(
        string layerName,
        Vector2I coords,
        int data,
        Vector2I atlasPos
    )
    {
        int layer = GetLayerNumber(layerName);
        Level.SetCell (layer, coords, data, atlasPos);
    }

    public void OnToggleLightingPressed()
    {
        LightingManager lightingManager =
            GetNode<LightingManager>("LightingManager");

        // Weirdly lightManager.Visible didn't like it when I tried to use it in a ternary operator.
        bool isVisible = lightingManager.Visible;
        if (isVisible)
        {
            lightingManager.Hide();
            lightingManager.ClearTiles();
        }
        else
        {
            lightingManager.Show();

            // Repopulate tiles
            EmitSignal(SignalName.OnLightsChanged, Level);
        }
    }

    public void UpdateLighting()
    {
        LightingManager lightingManager =
            GetNode<LightingManager>("LightingManager");

        lightingManager.Show();

        // Repopulate tiles
        EmitSignal(SignalName.OnLightsChanged, Level);
    }

    private void CreateLightSource()
    {
        GD.Print("WARNING: CreateLightSource has been deactivated while I rework a bunch of shit");
        // TODO place in front of the user instead of on the player.
        // perhaps storing the direction the player is facing could be good
        //var playerPos = Level.LocalToMap(player.Position);
        //// Layer 2 is lights
        //Level.SetCell(2, new Vector2I(playerPos.X, playerPos.Y), 2, new Vector2I(0, 0));
        //EmitSignal(SignalName.OnLightsChanged, Level);
    }

    private void OnIncrementLevelPressed()
    {
        GD.Print("Increment Level");
        currentLevelID += 1;
        if (currentLevelID >= cMAX_LEVEL_NUMBER)
        {
            DisplayGameOver();
            return;
        }
        LoadLevel(currentLevelID);
    }
    public void ApplyFlameToTile(Vector2I affectedTile, string layerName)
    {
        // Change affected Tile to linked tile
        TileData tileData = GetTileData(layerName, affectedTile);
        Vector2I LinkedTile = tileData.GetCustomData("LinkedFlameTile").AsVector2I();
        // For now just assume that the linked tile is on the same texture
        int sourceID = Level.GetCellSourceId(GetLayerNumber(layerName), affectedTile);
        Level.SetCell(GetLayerNumber(layerName), affectedTile, sourceID, LinkedTile);

        EmitSignal(SignalName.OnLightsChanged, Level);
    }

    public void ApplyIceToTile(Vector2I affectedTile, string layerName)
    {
        // Change affected Tile to linked tile
        TileData tileData = GetTileData(layerName, affectedTile);
        Vector2I LinkedTile = tileData.GetCustomData("LinkedIceTile").AsVector2I();
        // For now just assume that the linked tile is on the same texture
        int sourceID = Level.GetCellSourceId(GetLayerNumber(layerName), affectedTile);
        Level.SetCell(GetLayerNumber(layerName), affectedTile, sourceID, LinkedTile);

        EmitSignal(SignalName.OnLightsChanged, Level);
    }

    public bool CheckIfGameOver() {
        GD.Print("player=", playerManager.GetPlayerRef(), " | shadowTileMap=", shadowTileMap);
        if (playerManager.GetPlayerRef() != null && shadowTileMap != null) {
            Vector2I playerCoords = playerManager.GetPlayerRef().tileCoords;
            TileData shadowData = shadowTileMap.GetCellTileData(0, playerCoords);

            // Detected player is in the shadow
            if (shadowData != null) {
                GD.Print("game over detected");
                return true;
            }
        }
        GD.Print("game over not detected");
        return false;
    }

    public void DisplayGameOver() {
        // Clean up current level if it exists
        if (Level != null) {
            Level.ZIndex = 100;
            Level.QueueFree();
            LayerMap.Clear();
            Level = null;
        }
        GD.Print("Displaying gameover view");
        gameOverScene.Show();
    }

    public void OnPlayerChanged(GameObject player) {
        if (CheckIfGameOver()) {
            DisplayGameOver();
        }
    }
}
