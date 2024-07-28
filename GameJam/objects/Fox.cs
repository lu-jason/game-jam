using Godot;
using System;

public partial class Fox : Player
{
    [Export]
    PackedScene FoxIceScene { get; set; }

    //[Signal]
    //public delegate void OnFlameTileEventHandler(Vector2I affectedTile, string layerName);

    // Maybe simpler to have a separate signal for applying flames to an object
    // Somthing like
    //[Signal]
    //public delegate void OnFlameObjectEventHandler(int objectID);

    public override void UseAbility(Vector2I affectedTile)
    {
        // TODO Clean this up so there isn't so much terrible code dupe.

        // See if anything on the tile is flammable or something like that
        // Call a callback if it is. 
        // we should object and layers and whatever but just manually doing it for now.
        TileData lightData = levelViewer.GetTileData("lights", affectedTile);
        if (lightData != null)
        {
            var iceable = lightData.GetCustomData("Iceable");
            if (iceable.AsBool())
            {
                levelViewer.ApplyIceToTile(affectedTile, "lights");
            }
        }


        TileData floorData = levelViewer.GetTileData("floors", affectedTile);
        if (floorData != null)
        {
            var iceable = floorData.GetCustomData("Iceable");
            if (iceable.AsBool())
            {
                levelViewer.ApplyIceToTile(affectedTile, "floors");
            }
        }
    }

    public override AnimatedSprite2D GetAbilityAnimation()
    {
        return FoxIceScene.Instantiate<AnimatedSprite2D>();
    }
}
