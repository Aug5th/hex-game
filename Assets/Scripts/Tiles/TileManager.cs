using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : Singleton<TileManager>
{
    [SerializeField] private Tilemap _tileMap;
    [SerializeField] private Tilemap _highlightTilemap;
    [SerializeField] private Unit _unitPrefab;

    private List<Vector3Int> highlightCells = new List<Vector3Int>();
    [SerializeField] private Tile _highlightTile;

    private Dictionary<Vector3Int, TileData> _tiles = new Dictionary<Vector3Int, TileData>();
    private Unit _selectedUnit;


    void Start()
    {
        InitializeTiles();
    }

    private void InitializeTiles()
    {
        BoundsInt bounds = _tileMap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (_tileMap.HasTile(pos))
            {
                _tiles[pos] = new TileData(pos);
            }
        }

        //Spawn a test unit at the center of the tilemap
        Vector3Int centerPos = new Vector3Int(0, 0, 0);
        if (_tiles.ContainsKey(centerPos))
        {
            Unit testUnit = Instantiate(_unitPrefab);
            testUnit.PlaceOnTile(_tiles[centerPos], _tileMap);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0; // Ensure we are working in 2D space
            Vector3Int cellPos = _tileMap.WorldToCell(mouseWorldPos);
            
            if (_tiles.ContainsKey(cellPos))
            {
                Debug.Log($"Tile clicked at {cellPos}");
                OnTileClicked(_tiles[cellPos]);
            }
        }
    }

    private void OnTileClicked(TileData tileData)
    {
        if (tileData.Unit != null)
        {
            _selectedUnit = tileData.Unit;
            HighlightMoveRange(_selectedUnit);
        }
        else if (_selectedUnit != null && highlightCells.Contains(tileData.CellPos))
        {
            _selectedUnit.PlaceOnTile(tileData, _tileMap);
            ClearHighlights();
            _selectedUnit = null;
        }
        else
        {
            ClearHighlights();
            _selectedUnit = null;
        }
    }

    private void ClearHighlights()
    {
        foreach (var pos in highlightCells)
        {
            _highlightTilemap.SetTile(pos, null);
        }
        highlightCells.Clear();
    }

    private void HighlightMoveRange(Unit unit)
    {
        ClearHighlights();
        foreach (var kvp in _tiles)
        {
            int dist = HexDistance(unit.CurrentTile.CellPos, kvp.Key);
            if (dist <= unit.MoveRange && kvp.Value.Unit == null)
            {
                highlightCells.Add(kvp.Key);
                _highlightTilemap.SetTile(kvp.Key, _highlightTile);
            }
        }
    }
    
    int HexDistance(Vector3Int a, Vector3Int b)
    {
        Vector3Int ac = OffsetToCube(a);
        Vector3Int bc = OffsetToCube(b);

        return (Mathf.Abs(ac.x - bc.x) + Mathf.Abs(ac.y - bc.y) + Mathf.Abs(ac.z - bc.z)) / 2;
    }

    Vector3Int OffsetToCube(Vector3Int hex)
    {
        int x = hex.x - (hex.y >> 1); // flat-top odd-r
        int z = hex.y;
        int y = -x - z;
        return new Vector3Int(x, y, z);
    }
}
