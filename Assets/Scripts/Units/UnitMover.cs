using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitMover : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;
    private bool isMoving = false;

    public IEnumerator MoveAlongPath(List<Vector3Int> path)
    {
        if (isMoving) yield break;
        isMoving = true;

        foreach (Vector3Int tile in path)
        {
            Vector3 targetPos = TileManager.Instance.GetUnitPosFromTilePos(tile);
            yield return StartCoroutine(MoveTo(targetPos));
        }

        isMoving = false;
    }

    private IEnumerator MoveTo(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _moveSpeed * Time.deltaTime);
            // yield return new WaitForSeconds(0.005f); // Small delay to allow for smooth movement
            yield return null;
        }
        transform.position = targetPos;
    }
}
