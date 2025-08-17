using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    public float dragSpeed = 2;
    private Vector3 dragOrigin;

    void Update()
    {
        // Check if the right mouse button is pressed
        if (Input.GetMouseButtonDown(1))
        {
            // Record the initial mouse position
            dragOrigin = Input.mousePosition;
            return;
        }

        // Check if the right mouse button is held down
        if (!Input.GetMouseButton(1)) return;

        // Calculate the difference in position
        Vector3 difference = Input.mousePosition - dragOrigin;

        // Move the camera
        Vector3 move = new Vector3(-difference.x * dragSpeed * Time.deltaTime, -difference.y * dragSpeed * Time.deltaTime, 0);
        transform.Translate(move, Space.World);

        // Update the drag origin to the current mouse position
        dragOrigin = Input.mousePosition;
    }
}
