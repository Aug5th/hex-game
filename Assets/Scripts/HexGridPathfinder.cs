using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridPathfinder
{
    private static readonly Vector3Int[] directionsEven = new Vector3Int[]
    {
        new Vector3Int(+1, 0, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(-1, +1, 0),
        new Vector3Int(0, +1, 0),
    };

    private static readonly Vector3Int[] directionsOdd = new Vector3Int[]
    {
        new Vector3Int(+1, 0, 0),
        new Vector3Int(+1, -1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, +1, 0),
        new Vector3Int(+1, +1, 0),
    };

    public static List<Vector3Int> GetNeighbors(Dictionary<Vector3Int, TileData> tiles, Vector3Int pos, TileType moveableTileType)
    {
        Vector3Int[] directions = pos.y % 2 == 0 ? directionsEven : directionsOdd;

        List<Vector3Int> neighbors = new List<Vector3Int>();
        foreach (var dir in directions)
        {
            Vector3Int neighborPos = pos + dir;
            if (tiles.ContainsKey(neighborPos) && !tiles[neighborPos].Unit && tiles[neighborPos].TileType == moveableTileType)
            {
                neighbors.Add(neighborPos);
            }
        }
        return neighbors;
    }

    public static List<Vector3Int> FindPath(Dictionary<Vector3Int, TileData> tiles, Vector3Int start, Vector3Int goal)
    {
        Queue<Vector3Int> frontier = new Queue<Vector3Int>();
        frontier.Enqueue(start);

        Dictionary<Vector3Int, Vector3Int?> cameFrom = new Dictionary<Vector3Int, Vector3Int?>();
        cameFrom[start] = null;

        while (frontier.Count > 0)
        {
            Vector3Int current = frontier.Dequeue();

            if (current == goal)
                break;

            Vector3Int[] directions = (current.y % 2 == 0) ? directionsEven : directionsOdd;

            foreach (var dir in directions)
            {
                Vector3Int next = current + dir;

                // check valid , obstacle
                if (!tiles.ContainsKey(next))
                    continue;

                if (!cameFrom.ContainsKey(next) && tiles[next].TileType == tiles[start].TileType && !tiles[next].Unit)
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        // if can't reach goal
        if (!cameFrom.ContainsKey(goal))
            return new List<Vector3Int>();

        // Reconstruct path
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int? currentTile = goal;

        while (currentTile != null)
        {
            path.Add(currentTile.Value);
            currentTile = cameFrom[currentTile.Value];
        }

        path.Reverse(); // from start -> goal
        return path;
    }
}
