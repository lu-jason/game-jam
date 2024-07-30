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
    public override bool WallMoveSpecific(TileData wallData)
    {
        if (wallData == null) return false;
        
        
        // Allow movement on small walls as salamander
        return wallData.GetCustomData("Small").AsBool();
    }
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

        TileData floorData = levelViewer.GetTileData("floors", affectedTile);
        if (floorData != null)
        {
            var flammable = floorData.GetCustomData("Flammable");
            if (flammable.AsBool())
            {
                // Could do this with signals
                // TODO
                levelViewer.ApplyFlameToTile(affectedTile, "floors");
            }
        }

    }

    public override AnimatedSprite2D GetAbilityAnimation()
    {
        return SalamanderFireScene.Instantiate<AnimatedSprite2D>();
    }
}
