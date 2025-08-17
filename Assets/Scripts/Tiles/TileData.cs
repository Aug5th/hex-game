using UnityEngine;

public class TileData
{
    [SerializeField] private Vector3Int _cellPos;
    public Vector3Int CellPos => _cellPos;

    [SerializeField] private Unit _unit;
    public Unit Unit
    {
        get => _unit;
        set => _unit = value;
    }

    private TileType _tileType;
    public TileType TileType
    {
        get => _tileType;
        set => _tileType = value;
    }
    public TileData(TileType tileType, Vector3Int cellPos)
    {
        _tileType = tileType;
        _cellPos = cellPos;
        _unit = null;  
    }

    public bool IsEmpty => _unit == null;
}
