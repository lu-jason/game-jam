using Godot;
using Godot.Collections;
using System;
using System.Numerics;

public partial class LevelViewer : Node2D {
	[Export]
	public PackedScene Map { get; set; }

	[Signal]
	public delegate void OnLoadLevelEventHandler(TileMap LoadedLevel);

	[Signal]
	public delegate void OnLightsChangedEventHandler(TileMap loadedLevel);

	private TileMap Level;

	private Dictionary<string, int> LayerMap = new Dictionary<string, int>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Level = Map.Instantiate<TileMap>();
		Level.ZIndex = -1;

		AddChild(Level);
		EmitSignal(SignalName.OnLoadLevel, Level);

		for (int i = 0; i < Level.GetLayersCount(); i++) {
			LayerMap.Add(Level.GetLayerName(i), i);
		}

		// Later change this to be called whenever we move a light or something can change it maybe
		EmitSignal(SignalName.OnLightsChanged, Level);

        // TODO Find spawn point
        // For now hardcoded
        PlayerManager playerManager = GetNode<PlayerManager>("PlayerManager");
        playerManager.MovePlayerTo(new Vector2I(10, 10));


        //player = GetNode<Player>("Player");
		//player.currentLevel = Level;
	}

	private int GetLayerNumber(string layerName) {
		return LayerMap[layerName];
	}
	public override void _Input(InputEvent @event) 
    {
		

        // We can move this later if we want just adding for testing sake
        if (@event.IsActionPressed("place_light")) 
        {
            CreateLightSource();
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

    public TileData GetTileData(String layerName, Vector2I coords) 
    {
        int layer = GetLayerNumber(layerName);
        return Level.GetCellTileData(layer, coords);
    }

	public void OnToggleLightingPressed() {
		LightingManager lightingManager = GetNode<LightingManager>("LightingManager");
		// Weirdly lightManager.Visible didn't like it when I tried to use it in a ternary operator.
		bool isVisible = lightingManager.Visible;
		if (isVisible) {
			lightingManager.Hide();
			lightingManager.ClearTiles();
		} else {
			lightingManager.Show();
			// Repopulate tiles
			EmitSignal(SignalName.OnLightsChanged, Level);
		}
	}


	private void CreateLightSource() {
        GD.Print("WARNING: CreateLightSource has been deactivated while I rework a bunch of shit");
		// TODO place in front of the user instead of on the player.
		// perhaps storing the direction the player is facing could be good
		//var playerPos = Level.LocalToMap(player.Position);
		//// Layer 2 is lights
		//Level.SetCell(2, new Vector2I(playerPos.X, playerPos.Y), 2, new Vector2I(0, 0));
		//EmitSignal(SignalName.OnLightsChanged, Level);

	}
}

