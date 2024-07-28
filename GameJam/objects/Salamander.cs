using Godot;
using System;

public partial class Salamander : Player
{
    [Export]
    PackedScene SalamanderFireScene { get; set; }
    public override void UseAbility(Vector2I affectedTile)
    {

        // See if anything on the tile is flammable or something like that
        // Call a callback if it is. 
    }

    public override AnimatedSprite2D GetAbilityAnimation()
    {
        return SalamanderFireScene.Instantiate<AnimatedSprite2D>();
    }
}
