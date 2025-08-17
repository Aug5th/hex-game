using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Unit : MonoBehaviour
{
    [SerializeField] private TileData _currentTile;
    public TileData CurrentTile => _currentTile;
    [SerializeField] private int _moveRange = 3;
    public int MoveRange => _moveRange;
    private TileType _moveableTileType = TileType.Water; // Example tile type that the unit can move on

    public void PlaceOnTile(TileData tile, Tilemap tilemap)
    {
        if (_currentTile != null)
        {
            _currentTile.Unit = null; // Remove unit from previous tile
        }

        _currentTile = tile;
        _currentTile.Unit = this;
    }

    public bool CheckMovableTile(TileType tileType)
    {
        return _moveableTileType == tileType;
    }
}
