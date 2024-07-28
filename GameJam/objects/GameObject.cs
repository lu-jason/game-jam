using System;
using Godot;
using static Godot.TextServer;

// This scene represents anything that can move around on the tile map
// And requires that logic
// i.e player, rocks etc.
public partial class GameObject : Node2D
{
    // Fuck it hard code the pixel size herePixel Size
    public const int cPixelSize = 32;

    public Vector2I tileCoords = new Vector2I(0, 0);

    // This is mush
    private Vector2 desiredPosition = new Vector2I(0, 0);
    private Vector2 originalPosition = new Vector2I(0, 0);

    private int currentFrame = 0;

    // idk how many frames we want with this
    const int cAnimationFrames = 20;

    // Technically would be better to rework this to use some enum rather than passing strings around everywhere
    public string facingDirection = "left";

    public LevelViewer levelViewer;
    public LightingManager lightingManager;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        levelViewer = GetNode<LevelViewer>("/root/Main/LevelViewer");
        lightingManager =
            GetNode<LightingManager>("/root/Main/LevelViewer/LightingManager");
        SetAnimationState("idle", facingDirection);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Do some lerp shit here
        if (currentFrame <= cAnimationFrames)
        {
            float ratio = ((float) currentFrame / (float) cAnimationFrames);
            Position =
                originalPosition +
                ((desiredPosition - originalPosition) * ratio);
            currentFrame = currentFrame + 1;
        }

        if (currentFrame == cAnimationFrames)
        {
            PlayerManager.SetLockInput(false);
            SetAnimationState("idle", facingDirection);
        }
    }

    /// <summary>
    /// Moves the character to the provided coords
    /// </summary>
    /// <param name="coords">To coordinates the character is trying to move to</param>
    /// <param name="direction">The direction the character is trying to move. Up, down, left, right. Can be empty</param>
    /// <returns>If the character can move to coords</returns>
    public bool MoveTo(Vector2I coords, string direction)
    {
        // First test the new coordinates
        // We can override this in different objects etc.
        if (CanMove(coords, direction))
        {
            originalPosition = desiredPosition;
            desiredPosition =
                new Vector2(coords.X * cPixelSize, coords.Y * cPixelSize);

            PlayerManager.SetLockInput(true);
            currentFrame = 0;

            SetAnimationState("move",direction);
            facingDirection = direction;

            tileCoords = coords;
            GD.Print("Moving to ", tileCoords);
            return true;
        }
        return false;
    }

    public virtual bool CanMove(Vector2I coords, string direction)
    {
        // Do whatever logic here for now return true;
        return true;
    }

    public bool MoveLeft()
    {
        Vector2I coords = new Vector2I(tileCoords.X, tileCoords.Y);
        coords.X -= 1;
        return MoveTo(coords, "left");
    }

    public bool MoveRight()
    {
        Vector2I coords = new Vector2I(tileCoords.X, tileCoords.Y);
        coords.X += 1;
        return MoveTo(coords, "right");
    }

    public bool MoveDown()
    {
        Vector2I coords = new Vector2I(tileCoords.X, tileCoords.Y);
        coords.Y += 1;
        return MoveTo(coords, "down");
    }

    public bool MoveUp()
    {
        Vector2I coords = new Vector2I(tileCoords.X, tileCoords.Y);
        coords.Y -= 1;
        return MoveTo(coords, "up");
    }

    public void OverrideTileCoords(Vector2I coords)
    {
        tileCoords = coords;
        Position = new Vector2(coords.X * cPixelSize, coords.Y * cPixelSize);
        desiredPosition = Position;
        originalPosition = Position;
    }

    virtual public void SetAnimationState(string action, string direction)
    {
        // default do nothing?
    }
}
