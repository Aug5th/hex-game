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

    public TileData(Vector3Int cellPos)
    {
        _cellPos = cellPos;
        _unit = null;
    }

    public bool IsEmpty => _unit == null;
}
