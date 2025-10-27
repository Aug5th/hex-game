using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class Unit : MonoBehaviour , IDamageable
{
    [SerializeField] private TileData _currentTile;
    public TileData CurrentTile => _currentTile;
    [SerializeField] private int _moveRange = 3;
    public int MoveRange => _moveRange;

    public int Health { get; private set; } = 100;

    public UnityAction OnDeath { get; set; }

    public void TakeDamage(int amount, GameObject source)
    {
        Health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage from {source.name}. Remaining health: {Health}");
        if (Health <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }

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
