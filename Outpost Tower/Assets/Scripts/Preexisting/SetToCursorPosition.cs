using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetToCursorPosition : MonoBehaviour
{
    [SerializeField] private float posZ;
    [SerializeField] private float distanceFromCamera;
    [SerializeField] private float mouseX;
    [SerializeField] private float mouseY;
    [SerializeField] private Vector3 mouseWorldPos;

    // Start is called before the first frame update
    void Start()
    {   
        // Lock Cursor to Screen (redundant)
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameActive)
        {
            // Set the position to the World Position of the cursor
            transform.position = GetMousePosition();
        }
        else
        {
            transform.position = new Vector3(3, 3, 0);
        }
    }

    // Returns In-World Transform position of cursor on the Player's Z plane
    Vector3 GetMousePosition()
    {
        // Get cursor position in pixels and clamp to screen
        mouseX = Mathf.Clamp(Input.mousePosition.x, 0, Screen.width);
        mouseY = Mathf.Clamp(Input.mousePosition.y, 0, Screen.height);

        // Get Z axis position of player and distance from camera
        posZ = transform.position.z;
        distanceFromCamera = posZ - Camera.main.transform.position.z;

        // Turn mouse position into world position
        mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseX, mouseY, distanceFromCamera));

        // Return world position
        return mouseWorldPos;
    }
}
