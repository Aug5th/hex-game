using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Custom Tile", menuName = "Tiles/Custom Tile")]
public class SO_CustomTile : Tile
{
    public TileType Type;
}

public enum TileType
{
    Ground,
    Grass,
    Water,
    Sand,
    None
}


