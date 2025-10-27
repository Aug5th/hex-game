using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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

    public UnityAction<TileData> OnTileClickedEvent;

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
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0; // Ensure we are working in 2D space
            Vector3Int cellPos = _tileMap.WorldToCell(mouseWorldPos);

            if (_tiles.ContainsKey(cellPos))
            {
                OnTileClickedEvent?.Invoke(_tiles[cellPos]);
            }
        }
    }

    public void HandleMoveUnit(Unit unit, TileData tileData)
    {
        List<Vector3Int> path = HexGridPathfinder.FindPath(_tiles, unit.CurrentTile.CellPos, tileData.CellPos);
        if (path.Count > 0)
        {
            StartCoroutine(unit.GetComponent<UnitMover>().MoveAlongPath(path, _tileMap));
        }
        unit.PlaceOnTile(tileData, _tileMap);
        ClearHighlights();
    }

    public void ClearHighlights()
    {
        foreach (var pos in highlightCells)
        {
            _highlightTilemap.SetTile(pos, null);
        }
        highlightCells.Clear();
    }

    public void HighlightMoveRange(Vector3Int unitPos , int unitMoveRange , TileType moveableTileType = TileType.Grass)
    {
        ClearHighlights();

        // Highlight surrounding tiles within move range
        Queue<(Vector3Int pos, int cost)> queue = new Queue<(Vector3Int, int)>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        queue.Enqueue((unitPos, 0));
        visited.Add(unitPos);

        while (queue.Count > 0)
        {
            var (currentPos, cost) = queue.Dequeue();

            if (cost > 0)
            {
                HighlightTile(currentPos);
            }

            foreach (var neighbor in HexGridPathfinder.GetNeighbors(_tiles, currentPos, moveableTileType))
            {
                if (!visited.Contains(neighbor))
                {
                    var newCost = cost + 1;
                    if (newCost <= unitMoveRange && _tiles[neighbor].IsEmpty)
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
}
