using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// InteractionManager is a class for managing interactions with the player.
public partial class InteractionManager : Node2D {
	private LevelViewer levelViewer;
	private Vector2I tileBounds;

	private Label interactionText;

	private GameObject playerRef;
	private Dictionary<ulong, uint> interactionDistances;
	private Dictionary<ulong, GameObject> gameObjects;

	// Pretty sure C# passes by reference
  private TileMap levelRef;

	public void Setup(TileMap level) {
		tileBounds = level.GetUsedRect().End;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		levelViewer = GetNode<LevelViewer>("/root/Main/LevelViewer");
		interactionText = GetNode<Label>("InteractionText");
		gameObjects = new Dictionary<ulong, GameObject>();
		interactionDistances = new Dictionary<ulong, uint>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	public void OnObjectChanged(GameObject go, Vector2I position) {
		if (go != null) {
			var tileData = levelViewer.GetTileData("objects", position);
			var canInteract = tileData != null ? tileData.GetCustomData("CanInteract").AsBool() : false;
			var interactionDistance = tileData != null ? tileData.GetCustomData("InteractionDistance").AsUInt32() : 0;
			GD.Print("The following object has changed: ", go.GetInstanceId(), "(CanInteract=", canInteract, ", interactionDistance=", interactionDistance, ")");

			// Remove object from interactable list if not interactable anymore
			if (!canInteract && gameObjects.ContainsKey(go.GetInstanceId())) {
				GD.Print("Removing object from interable list: ", go.GetInstanceId());
				gameObjects.Remove(go.GetInstanceId());
				interactionDistances.Remove(go.GetInstanceId());
			}
			// Add new interactable object
			else if (canInteract && !gameObjects.ContainsKey(go.GetInstanceId())) {
				GD.Print("Adding object to interable list: ", go.GetInstanceId());
				gameObjects.Add(go.GetInstanceId(), go);
				interactionDistances.Add(go.GetInstanceId(), interactionDistance);
			}
			HandleInteractions();
		} else {
			GD.Print("Changed object is null");
		}
	}

	public void OnPlayerChanged(GameObject player) {
		GD.Print("Player has changed: ", player.GetInstanceId());
		playerRef = player;
		HandleInteractions();
	}

	public bool CheckPlayerCanInteractWithObject(GameObject go) {
		if (playerRef != null) {
			var interactionDistance = interactionDistances[go.GetInstanceId()];
			var numTilesBetween = GetNumTilesBetween(playerRef.tileCoords, go.tileCoords);
			return numTilesBetween <= interactionDistance;
		}
		return false;
	}

	public uint GetNumTilesBetween(Vector2I posA, Vector2I posB) {
		// Using simple maths for now
		return (uint)Math.Ceiling(
			Math.Sqrt(Math.Pow(posA.X - posB.X, 2) + Math.Pow(posA.Y - posB.Y, 2))
		);
	}

	public void HandleInteractions() {
		GD.Print("gameObjects list = ", gameObjects != null ? gameObjects.Count() : "Is Null");
		if (gameObjects != null && gameObjects.Count() > 0) {
			var filteredGameObjects = gameObjects.Where(x => CheckPlayerCanInteractWithObject(x.Value));
			var sortedGameObjects = filteredGameObjects.OrderBy(x => interactionDistances[x.Value.GetInstanceId()]);
			var sortedGameObjectsList = sortedGameObjects.ToList();
			GD.Print("The number of objects that can be interacted by the player is: ", sortedGameObjects.Count());
			if (sortedGameObjectsList.Count() > 0) {
				string textToDisplay = "Press [Space] to interact";
				if (sortedGameObjectsList[0].Value.interactionText != "") {
					textToDisplay = sortedGameObjectsList[0].Value.interactionText;
				}
				interactionText.Text = textToDisplay;
				interactionText.GlobalPosition = new Vector2(
					sortedGameObjectsList[0].Value.GlobalPosition.X - interactionText.Size.X / 2,
					sortedGameObjectsList[0].Value.GlobalPosition.Y - 36
				);
				GD.Print("Showing interaction text");
				interactionText.Show();
			} else {
				GD.Print("Hiding interaction text");
				interactionText.Hide();
			}
		} else {
			GD.Print("Hiding interaction text");
			interactionText.Hide();
		}
	}
}
