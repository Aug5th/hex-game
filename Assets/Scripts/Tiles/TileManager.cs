using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Vector3 = UnityEngine.Vector3;

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
                _tiles[pos] = new TileData(_tileMap.GetTile<SO_CustomTile>(pos).Type, pos);
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
        Vector3Int unitPos = unit.CurrentTile.CellPos;
        Debug.Log($"Cell pos: {unitPos}");

        // Highlight surrounding tiles within move range
        Queue<(Vector3Int pos, int cost)> queue = new Queue<(Vector3Int, int)>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        queue.Enqueue((unitPos, 0));
        visited.Add(unitPos);

        while (queue.Count > 0)
        {
            var (currentPos, cost) = queue.Dequeue();
            
            if (cost > unit.MoveRange)
                continue;
                
            HighlightTile(currentPos);

            foreach (var neighbor in GetNeighbors(currentPos, unit.CurrentTile.TileType))
            {
                if (!visited.Contains(neighbor))
                {
                    var newCost = cost + 1;
                    if (newCost <= unit.MoveRange && _tiles[neighbor].IsEmpty)
                    {
                        queue.Enqueue((neighbor, newCost));
                        visited.Add(neighbor);
                    }
                }
            }
        }
    }

    private void HighlightTile(Vector3Int cellPos)
    {
        if (!_highlightTilemap.HasTile(cellPos))
        {
            _highlightTilemap.SetTile(cellPos, _highlightTile);
            highlightCells.Add(cellPos);
        }
    }
    
    private List<Vector3Int> GetNeighbors(Vector3Int pos, TileType moveableTileType)
    {
        // Vector3Int[] directions = new Vector3Int[]
        // {
        //     new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
        //     new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0),
        //     new Vector3Int(1, -1, 0), new Vector3Int(-1, 1, 0)

        //     /*
        //     new Vector3Int(1, -1, 0),
        //     new Vector3Int(1, 0, -1),
        //     new Vector3Int(0, 1, -1),
        //     new Vector3Int(-1, 1, 0),
        //     new Vector3Int(-1, 0, 1),
        //     new Vector3Int(0, -1, 1)
        //     */
        // };

        // flat-top hex offset (even-q layout)
        Vector3Int[] directionsEven = new Vector3Int[]
        {
            new Vector3Int(+1, 0, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(-1, -1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(-1, +1, 0),
            new Vector3Int(0, +1, 0),
        };

        Vector3Int[] directionsOdd = new Vector3Int[]
        {
            new Vector3Int(+1, 0, 0),
            new Vector3Int(+1, -1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, +1, 0),
            new Vector3Int(+1, +1, 0),
        };

        Vector3Int[] directions = pos.y % 2 == 0 ? directionsEven : directionsOdd;

        List<Vector3Int> neighbors = new List<Vector3Int>();
        foreach (var dir in directions)
        {
            Vector3Int neighborPos = pos + dir;
            if (_tiles.ContainsKey(neighborPos) && !_tiles[neighborPos].Unit && _tiles[neighborPos].TileType == moveableTileType)
            {
                neighbors.Add(neighborPos);
            }
        }
        return neighbors;
    }
}
