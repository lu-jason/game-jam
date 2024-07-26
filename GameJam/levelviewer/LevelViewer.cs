using Godot;
using Godot.Collections;
using System;
using System.Numerics;

public partial class LevelViewer : Node2D
{
	[Export]
	public PackedScene Map { get; set; }

	[Export]
	public PackedScene RockScene { get; set; }

	[Signal]
	public delegate void OnLoadLevelEventHandler(TileMap LoadedLevel);

	[Signal]
	public delegate void OnLightsChangedEventHandler(TileMap loadedLevel);

	private TileMap Level;

	private Vector2I tileBounds;

	private GameObject[,] gameObjects;

	private Dictionary<string, int> LayerMap = new Dictionary<string, int>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Level = Map.Instantiate<TileMap>();
		Level.ZIndex = -1;

		AddChild(Level);
		EmitSignal(SignalName.OnLoadLevel, Level);

		SetupLevelInfo();

		// Later change this to be called whenever we move a light or something can change it maybe
		EmitSignal(SignalName.OnLightsChanged, Level);

		// TODO Find spawn point
		// For now hardcoded
		PlayerManager playerManager = GetNode<PlayerManager>("PlayerManager");
		playerManager.MovePlayerTo(new Vector2I(10, 10));

		CreateObjects();
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

	private void CreateObjects()
	{
		// Iterate through all the objects in the "objects" layer in the tilemap
		// and create the corresponding object and delete the cell from the map
		for (int x = 0; x < tileBounds.X; x++)
		{
			for (int y = 0; y < tileBounds.Y; y++)
			{
				var tileData = GetTileData("objects", new Vector2I(x, y));

				if (tileData != null && tileData.GetCustomData("Rock").AsBool())
				{
					var position = new Vector2I(x, y);
					Level.SetCell(GetLayerNumber("objects"), position, -1);
					GD.Print("Found rock at ", x, y);
					var rock = RockScene.Instantiate<Rock>();
					AddChild(rock);
					rock.MoveTo(position, "");
					AddGameObject(rock, position);
				}
			}
		}
	}

	private bool AddGameObject(GameObject obj, Vector2I position)
	{
		if (gameObjects[position.X, position.Y] == null)
		{
			gameObjects[position.X, position.Y] = obj;
			return true;
		}
		return false;
	}

	public bool IsObject(Vector2I position)
	{
		if (gameObjects[position.X, position.Y] != null)
		{
			return true;
		}
		return false;
	}

	public bool MoveObject(Vector2I position, string direction)
	{
		if (IsObject(position))
		{
			var gameObject = gameObjects[position.X, position.Y];
			switch (direction)
			{
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
	}

	private int GetLayerNumber(string layerName)
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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public TileData GetTileData(string layerName, Vector2I coords)
	{
		int layer = GetLayerNumber(layerName);
		return Level.GetCellTileData(layer, coords);
	}

	public void OnToggleLightingPressed()
	{
		LightingManager lightingManager = GetNode<LightingManager>("LightingManager");
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
}

