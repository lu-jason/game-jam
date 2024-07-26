using Godot;
using System;

// This scene represents anything that can move around on the tile map
// And requires that logic
// i.e player, rocks etc.
public partial class GameObject : Node2D {
    // Fuck it hard code the pixel size herePixel Size
    const int cPixelSize = 32;

    public Vector2I tileCoords = new Vector2I(0, 0);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
    }

    /// <summary>
    /// Moves the character to the provided coords
    /// </summary>
    /// <param name="coords">To coordinates the character is trying to move to</param>
    /// <param name="direction">The direction the character is trying to move. Up, down, left, right. Can be empty</param>
    /// <returns>If the character can move to coords</returns>
    public bool MoveTo(Vector2I coords, string direction) {
        // First test the new coordinates
        // We can override this in different objects etc.
        if (CanMove(coords, direction)) {
            // TO DO hook up animation here for move to new location
            // We could store the location we want to move to
            // And gradually move our sprite to that positon across multiple frames
            // while blocking movement.
            // While triggering "walk" animation.
            Position = new Vector2(coords.X * cPixelSize, coords.Y * cPixelSize);
            tileCoords = coords;
            GD.Print("Moving to ", tileCoords);
            return true;
        }
        return false;
    }

    virtual public bool CanMove(Vector2I coords, string direction) {
        // Do whatever logic here for now return true;
        return true;
    }

    public bool MoveLeft() {
        Vector2I coords = new Vector2I(tileCoords.X, tileCoords.Y);
        coords.X -= 1;
        return MoveTo(coords, "left");
    }
    public bool MoveRight() {
        Vector2I coords = new Vector2I(tileCoords.X, tileCoords.Y);
        coords.X += 1;
        return MoveTo(coords, "right");
    }

    public bool MoveDown() {
        Vector2I coords = new Vector2I(tileCoords.X, tileCoords.Y);
        coords.Y += 1;
        return MoveTo(coords, "down");
    }

    public bool MoveUp() {
        Vector2I coords = new Vector2I(tileCoords.X, tileCoords.Y);
        coords.Y -= 1;
        return MoveTo(coords, "up");
    }

    public void OverrideTileCoords(Vector2I coords) {
        tileCoords = coords;
        Position = new Vector2(coords.X * cPixelSize, coords.Y * cPixelSize);
    }
}
