using Godot;
using System;
using System.Numerics;

public partial class LevelViewer : Node2D {
    [Export]
    public PackedScene Map { get; set; }

    [Export]
    public PackedScene Rock { get; set; }

	[Export]
	public PackedScene ArcticFoxScene { get; set; }
	[Export]
	public PackedScene PlayerScene { get; set; }

    [Signal]
    public delegate void OnLoadLevelEventHandler(TileMap LoadedLevel);

    [Signal]
    public delegate void OnLightsChangedEventHandler(TileMap loadedLevel);

	private TileMap Level;
	private Player player;
	private ArcticFox fox;

	private enum MorphState { witch, fox, salamander, gargoyle }
	private MorphState currentMorph;
	private Godot.Vector2 currentPos;
	private double morphTimer = 0.0;
	private double morphTimeout = 0.3;
	private bool canMorph = true;
    public Object[] objects;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        Level = Map.Instantiate<TileMap>();
        Level.ZIndex = -1;

        AddChild(Level);
        EmitSignal(SignalName.OnLoadLevel, Level);

		//var rock = Rock.Instantiate<Rock>();
		//rock.Position = Level.MapToLocal(new Vector2I(2, 2));
        // Later change this to be called whenever we move a light or something can change it maybe
        EmitSignal(SignalName.OnLightsChanged, Level);

        player = GetNode<Player>("Player");
		currentMorph = MorphState.witch;

        //var rock = Rock.Instantiate<Rock>();
        //rock.Position = Level.MapToLocal(new Vector2I(2, 2));
	}

    public override void _Input(InputEvent @event) {

		if (currentMorph == MorphState.witch && !player.moving) {
			if (@event.IsActionPressed("ui_left")) {
				PlayerMovement("ui_left", TileSet.CellNeighbor.LeftSide);
			} else if (@event.IsActionPressed("ui_right")) {
				PlayerMovement("ui_right", TileSet.CellNeighbor.RightSide);
			} else if (@event.IsActionPressed("ui_up")) {
				PlayerMovement("ui_up", TileSet.CellNeighbor.TopSide);
			} else if (@event.IsActionPressed("ui_down")) {
				PlayerMovement("ui_down", TileSet.CellNeighbor.BottomSide);
			}
		}
		if (currentMorph == MorphState.fox) {
			if (@event.IsActionPressed("ui_left")) {
				FoxMovement("ui_left", TileSet.CellNeighbor.LeftSide);
			} else if (@event.IsActionPressed("ui_right")) {
				FoxMovement("ui_right", TileSet.CellNeighbor.RightSide);
			} else if (@event.IsActionPressed("ui_up")) {
				FoxMovement("ui_up", TileSet.CellNeighbor.TopSide);
			} else if (@event.IsActionPressed("ui_down")) {
				FoxMovement("ui_down", TileSet.CellNeighbor.BottomSide);
			}
		}

		// Morphing state
		if (canMorph) { // ! Mainly here for conditions where player is not allowed to morph (idk what that is yet) - Josh
			if (@event.IsActionPressed("morph_witch") && currentMorph != MorphState.witch  && morphTimer > morphTimeout) {
				GD.Print("morphing into witch"); // 1 + num1
				
				MorphPlayer(MorphState.witch);
				morphTimer = 0;
			}
			if (@event.IsActionPressed("morph_fox") && currentMorph != MorphState.fox && morphTimer > morphTimeout) {
				GD.Print("morphing into fox");  // 2 + num2

				MorphPlayer(MorphState.fox);
				morphTimer = 0;
			}	
			if (@event.IsActionPressed("morph_salamander") && morphTimer > morphTimeout) {
				GD.Print("morphing into salamander"); // 3 + num3
				morphTimer = 0;
			}
			if (@event.IsActionPressed("morph_gargoyle") && morphTimer > morphTimeout) {
				GD.Print("morphing into gargoyle"); // 4 + num4
				morphTimer = 0;
			}
		}

        // We can move this later if we want just adding for testing sake
        if (@event.IsActionPressed("place_light")) 
        {
            CreateLightSource();
        }
    }
	
    private void PlayerMovement(string direction, TileSet.CellNeighbor neighbour) {
        var playerCurrentXY = Level.LocalToMap(player.Position);
        var nextCell = Level.GetNeighborCell(playerCurrentXY, neighbour);
        var nextCellCoordinates = Level.MapToLocal(nextCell);
        var nextCellLayerZero = Level.GetCellTileData(0, nextCell);
        // For now adding shadows check here as well
        LightingManager LightingManager = GetNode<LightingManager>("LightingManager");
        TileMap ShadowTileMap = LightingManager.GetNode<TileMap>("ShadowTileMap");

        if ((Level.GetCellTileData(1, nextCell) == null) && (ShadowTileMap.GetCellTileData(0, nextCell) == null)) {
			player.MoveToPosition(nextCellCoordinates, direction);
			if (nextCellLayerZero.GetCustomData("Hole").AsBool()) {
				GD.Print("Is Hole");
			}
		} else {
			player.Face(direction);
		}
	}
	private void FoxMovement(string direction, TileSet.CellNeighbor neighbour) {
		GD.Print("Move ", direction);
	}

	private void MorphPlayer(MorphState morphInto) {
		switch (currentMorph) {
			case MorphState.witch:
				currentPos = player.Position;
				player.SetPhysicsProcess(false);
				player.QueueFree();
				break;
			case MorphState.fox:
				currentPos = fox.Position;
				fox.SetPhysicsProcess(false);
				fox.QueueFree();		
				break;
			case MorphState.salamander:
				break;
			case MorphState.gargoyle:
				break;
		}


		switch (morphInto) {
			case MorphState.witch:
				player = PlayerScene.Instantiate<Player>();

                var cameraPathingNode = new RemoteTransform2D {
                    RemotePath = GetNode<Camera2D>("CharacterCamera").GetPath()
                };

                player.AddChild(cameraPathingNode);
				AddChild(player);

				player.Position = currentPos;
				currentMorph = MorphState.witch;
				break;
			case MorphState.fox:
				fox = ArcticFoxScene.Instantiate<ArcticFox>();		
				AddChild(fox);
				fox.Position = currentPos;
				currentMorph = MorphState.fox;
				break;
			case MorphState.salamander:
				currentMorph = MorphState.salamander;
				break;
			case MorphState.gargoyle:
				currentMorph = MorphState.gargoyle;
				break;
		}
	}

	// private  GetActiveCharacterFromMorphState() {
	// 	switch (currentMorph){
	// 			case MorphState.witch:
	// 			return player;
	// 		case MorphState.fox:
	// 			AddChild(fox);
	// 			fox.Position = currentPos;
	// 			currentMorph = MorphState.fox;
	// 			break;
	// 		case MorphState.salamander:
	// 			currentMorph = MorphState.salamander;
	// 			break;
	// 		case MorphState.gargoyle:
	// 			currentMorph = MorphState.gargoyle;
	// 			break;
	// 	}
	// }


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		morphTimer += delta;
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
        // TODO place in front of the user instead of on the player.
        // perhaps storing the direction the player is facing could be good
        var playerPos = Level.LocalToMap(player.Position);
        // Layer 2 is lights
        Level.SetCell(2, new Vector2I(playerPos.X, playerPos.Y), 0, new Vector2I(9, 5));
        EmitSignal(SignalName.OnLightsChanged, Level);

    }
}

