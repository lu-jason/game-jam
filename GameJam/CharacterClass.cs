using Godot;
using System;
using System.Collections.Generic;

abstract public partial class CharacterClass : Area2D {
    public int animationSpeed = 4;
    public bool moving = false;
    public int tileSize = 32;

    public TileMap currentLevel;

    public Dictionary<string, Vector2> inputs = new Dictionary<string, Vector2>();

    public override void _Ready() {
		// position setting
		inputs.Add("ui_right", Vector2.Right);
		inputs.Add("ui_left", Vector2.Left);
		inputs.Add("ui_up", Vector2.Up);
		inputs.Add("ui_down", Vector2.Down);
		Position = Position.Snapped(Vector2.One * tileSize);
		Position += Vector2.One * tileSize / 2;
	}
 
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
    
    public abstract void Move(string direction, TileSet.CellNeighbor neighbour);

    public override void _Input(InputEvent @event) {
        if (!moving) {
            if (@event.IsActionPressed("ui_left")) {
                Move("ui_left", TileSet.CellNeighbor.LeftSide);
            } else if (@event.IsActionPressed("ui_right")) {
                Move("ui_right", TileSet.CellNeighbor.RightSide);
            } else if (@event.IsActionPressed("ui_up")) {
                Move("ui_up", TileSet.CellNeighbor.TopSide);
            } else if (@event.IsActionPressed("ui_down")) {
                Move("ui_down", TileSet.CellNeighbor.BottomSide);
            }
        }
    }

    public async void MoveToPosition(Vector2 position, string direction) {
		var tween = GetTree().CreateTween();
		tween.TweenProperty(this, "position", position, 1.0 / animationSpeed).SetTrans(Tween.TransitionType.Sine);
		moving = true;
		GetNode<AnimationPlayer>("AnimationPlayer").Play(direction);
		await ToSignal(tween, Tween.SignalName.Finished);
		moving = false;
	}

    
	public void Face(string direction) {
		GetNode<AnimationPlayer>("AnimationPlayer").Play(direction, fromEnd: true);
		// LookAt(point);
	}
}