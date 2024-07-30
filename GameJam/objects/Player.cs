using System;
using Godot;

public partial class Player : GameObject
{

    public override bool CanMove(Vector2I coords, string direction)
    {
        // Later we could hold a reference to these so we don't need to look them up with this "hardcodey" way
        LevelViewer levelViewer =
            GetNode<LevelViewer>("/root/Main/LevelViewer");
        TileData floorData = levelViewer.GetTileData("floors", coords);
        TileData wallData = levelViewer.GetTileData("walls", coords);

        // For now adding shadows check here as well
        LightingManager lightingManager =
            GetNode<LightingManager>("/root/Main/LevelViewer/LightingManager");
        TileMap shadowTileMap =
            lightingManager.GetNode<TileMap>("ShadowTileMap");
        TileData shadowData = shadowTileMap.GetCellTileData(0, coords);

        if (levelViewer.IsObject(coords))
        {
            GD.Print("Object is here");
            GameObject obj = levelViewer.ObjectAtPosition(coords);

            // if the object is solid, we cannot move.
            if (obj.GetGroups().Contains("Solid")) return false;

            // if the object is movable, handle.
            if (obj.GetGroups().Contains("Movable"))
            {
                var moved = levelViewer.MoveObject(coords, direction);
                if (moved)
                {
                    GD.Print("Object moved");
                }
                else
                {
                    GD.Print("Object not moved");
                    return false;
                }
            }
        }

        // If the walls and shadows are empty we can move there
        if ((wallData == null) && (shadowData == null))
        {
            bool IsEnd = floorData.GetCustomData("RoomEnd").AsBool();
            if (IsEnd)
            {
                sb.EmitSignal(SignalBus.SignalName.OnLevelEnd);
                return false;
            }
            // Check if the floor is good
            bool IsBlocked = floorData.GetCustomData("Blocked").AsBool();
            if (IsBlocked)
            {
                return false;
            }

            return true;
            // TODO - readd hole logic
        }

        // Do whatever logic here for now return true;
        return false;
    }
    public override void SetAnimationState(string action, string direction)
    {
        AnimatedSprite2D animSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        if (animSprite != null)
        {
            // Construct animation name
            string animationName = action + "_" + direction;
            // GD.Print("Setting animation to: ", animationName);

            // This is gonna spam the debugger when we don't have animations set up
            // Turning off for now
            animSprite.Play(animationName);

        }
    }

    public virtual void UseAbility(Vector2I affectedTile)
    {
        GD.Print("This should be overidden");
    }

    public virtual AnimatedSprite2D GetAbilityAnimation()
    {
        GD.Print("This should be overidden");
        return null;
    }
}
