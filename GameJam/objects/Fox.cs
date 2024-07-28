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

        // See if anything on the tile is flammable or something like that
        // Call a callback if it is. 
        // we should object and layers and whatever but just manually doing it for now.
        TileData lightData = levelViewer.GetTileData("lights", affectedTile);
        if (lightData != null)
        {
            var iceable = lightData.GetCustomData("Iceable");
            if (iceable.AsBool())
            {
                // Could do this with signals
                // TODO
                //levelViewer.ApplyIceToTile(affectedTile, "lights");
            }
        }
    }

    public override AnimatedSprite2D GetAbilityAnimation()
    {
        return FoxIceScene.Instantiate<AnimatedSprite2D>();
    }
}
