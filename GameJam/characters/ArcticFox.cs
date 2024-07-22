using Godot;
using System;

public partial class ArcticFox : CharacterClass
{
    public override void Move(string direction, TileSet.CellNeighbor neighbour){
		GD.Print("Move Fox ", direction);
	}

}
