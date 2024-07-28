using Godot;
using System;

public partial class Salamander : Player
{
    [Export]
    PackedScene SalamanderFireScene { get; set; }

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
            var flammable = lightData.GetCustomData("Flammable");
            if (flammable.AsBool())
            {
                // Could do this with signals
                levelViewer.ApplyFlameToTile(affectedTile, "lights");
            }
        }

    }

    public override AnimatedSprite2D GetAbilityAnimation()
    {
        return SalamanderFireScene.Instantiate<AnimatedSprite2D>();
    }
}
